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
        public string Login(UserLoginVm input, out TokenVm? tokenVm);
        public TokenVm? RefreshToken(TokenVm input);
    }

    public class AuthenticationService(
        ThreadsContext context,
        IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IOptionsMonitor<JwtSettings> jwtSettings) : IAuthenticationService
    {
        public string Login(UserLoginVm input, out TokenVm? tokenVm)
        {
            tokenVm = null;
            var user = userRepository.GetUserByUsernameAndPassword(input);
            if (user == null) return ("Đăng nhập không thành công");
            tokenVm = GenerateToken(new UserVm { Id = user.Id, Username = user.Username });
            return string.Empty;
        }

        public TokenVm? RefreshToken(TokenVm input)
        {
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
            if (refreshTk == null) { throw new MessageException("Refresh token không tồn tại"); }
            try
            {
                //Check 1: Kiểm tra access token có đúng định dạng và đã hết hạn chưa, nếu có thì sẽ throw lỗi
                jwtTokenHandler.ValidateToken(input.AccessToken, nonValidateLifeTime, out var validatedAccessToken);
                //Check 2: Kiểm tra thuật toán có khớp hay không
                if (validatedAccessToken is JwtSecurityToken token)
                {
                    if (!token.Header.Alg.Equals(SecurityAlgorithms.HmacSha512, StringComparison.InvariantCultureIgnoreCase))
                        throw new MessageException("Không đúng thuật toán");
                    if (!refreshTk.JwtId.Equals(token.Id)) throw new MessageException("JWT ID của Access token không khớp");
                }
            }
            catch (SecurityTokenExpiredException)
            {
                if (refreshTk.IsRevoked == true) throw new MessageException("Refresh token đã bị thu hồi");
                if (refreshTk.IsUsed == true) throw new MessageException("Refresh token đã được sử dụng");
                if (refreshTk.ExpiredAt < DateTime.Now) throw new MessageException("Refresh token đã hết hạn");
                //Renew token:
                refreshTk.IsRevoked = true;
                refreshTk.IsUsed = true;
                refreshTokenRepository.Update(refreshTk);
                var user = context.Users.SingleOrDefault(x => x.Id == refreshTk.UserId);
                if (user == null) throw new MessageException("Người dùng không tồn tại");
                return GenerateToken(new UserVm { Id = user.Id, Username = user.Username });
            }
            catch (MessageException) { throw; }
            catch (Exception) { throw new MessageException(ErrorConstants.CommonError); }
            throw new MessageException("Refresh token chưa hết hạn");
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