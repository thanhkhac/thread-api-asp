using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using thread_api_asp.Configurations;
using thread_api_asp.Entities;
using thread_api_asp.Errors;
using thread_api_asp.Repository;
using thread_api_asp.ViewModels;

namespace thread_api_asp.Services
{
    public interface IAuthenticationService
    {
        public ServiceResult Login(UserLoginVm input, out TokenVm? tokenVm);
        public ServiceResult RefreshToken(TokenVm input, out TokenVm? output);
    }

    public class AuthenticationService(
        ThreadsContext context,
        IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IOptionsMonitor<JwtSettings> jwtSettings) : IAuthenticationService
    {
        public ServiceResult Login(UserLoginVm input, out TokenVm? tokenVm)
        {
            tokenVm = null;
            if (userRepository.GetUserByUsername(input.UserName) == null) return ServiceResult.Error("Tài khoản không tồn tại");
            var user = userRepository.GetUserByUsernameAndPassword(input);
            if (user == null) return ServiceResult.Error("Tài khoản");
            tokenVm = GenerateToken(new UserVm { Id = user.Id, Username = user.Username });
            return ServiceResult.Ok("Đăng nhập thành công");
        }

        public ServiceResult RefreshToken(TokenVm input, out TokenVm? output)
        {
            output = null;
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var secretKeyBytes = Encoding.UTF8.GetBytes(jwtSettings.CurrentValue.SecretKey);
            var nonValidateLifeTime = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(secretKeyBytes),
                ClockSkew = TimeSpan.Zero,
            };
            var refreshTk = refreshTokenRepository.GetRefreshToken(input.RefreshToken);
            if (refreshTk == null) { return ServiceResult.Error("Refresh token không tồn tại"); }
            try
            {
                //Check 1: Kiểm tra access token có đúng định dạng và đã hết hạn chưa, nếu có thì sẽ throw lỗi
                jwtTokenHandler.ValidateToken(input.AccessToken, nonValidateLifeTime, out var validatedAccessToken);
                //Check 2: Kiểm tra thuật toán có khớp hay không
                if (validatedAccessToken is JwtSecurityToken token)
                {
                    if (!token.Header.Alg.Equals(SecurityAlgorithms.HmacSha512, StringComparison.InvariantCultureIgnoreCase))
                        ServiceResult.Error("Không đúng thuật toán");
                    if (!refreshTk.JwtId.Equals(token.Id)) ServiceResult.Error("JWT ID của Access token không khớp");
                }
            }
            catch (SecurityTokenExpiredException)
            {
                if (refreshTk.IsRevoked == true) return ServiceResult.Error("Refresh token đã bị thu hồi");
                if (refreshTk.IsUsed == true) return ServiceResult.Error("Refresh token đã được sử dụng");
                if (refreshTk.ExpiredAt < DateTime.Now) return ServiceResult.Error("Refresh token đã hết hạn");
                //Renew token:
                refreshTk.IsRevoked = true;
                refreshTk.IsUsed = true;
                refreshTokenRepository.Update(refreshTk);
                var user = context.Users.SingleOrDefault(x => x.Id == refreshTk.UserId);
                if (user == null) return ServiceResult.Error("Người dùng không tồn tại");
                output = GenerateToken(new UserVm { Id = user.Id, Username = user.Username });
                return ServiceResult.Ok("Gia hạn thành công");
            }
            catch (Exception)
            {
                //Log lỗi tại đây
                return ServiceResult.Error(ErrorConstants.CommonError);
            }
            return ServiceResult.Error("Refresh token chưa hết hạn");
        }


        #region Helper

        private TokenVm? GenerateToken(UserVm userVm)
        {
            //Chuyển secret key thành byte để mã hóa
            var secretKeyBytes = Encoding.UTF8.GetBytes(jwtSettings.CurrentValue.SecretKey);
            //Tạo thông tin JWT (Json Web Token)
            var tokenDescription = new SecurityTokenDescriptor
            {
                //Các claim để client có lấy ra thông tin (nếu cần)
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(nameof(userVm.Id), userVm.Id),
                    new Claim(nameof(userVm.Username), userVm.Username),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                }),
                Expires = DateTime.UtcNow.AddSeconds(20),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKeyBytes), SecurityAlgorithms.HmacSha512)
            };
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            //Tạo Access Token
            var token = jwtTokenHandler.CreateToken(tokenDescription);
            var accessToken = jwtTokenHandler.WriteToken(token);
            //Tạo refresh token
            var refreshToken = GenerateRefreshToken();
            //Lưu refresh token vào database 
            var refreshTokenEntity = new RefreshToken
            {
                Id = Guid.NewGuid().ToString(),
                UserId = userVm.Id,
                Token = refreshToken,
                JwtId = token.Id,
                IsRevoked = false,
                IsUsed = false,
                IssuedAt = DateTime.Now,
                ExpiredAt = DateTime.Now.AddHours(1)
            };
            refreshTokenRepository.Add(refreshTokenEntity);
            //Trả về thông tin token
            return new TokenVm
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }

        private string GenerateRefreshToken()
        {
            var random = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(random);
            return Convert.ToBase64String(random);
        }

        #endregion

    }
}