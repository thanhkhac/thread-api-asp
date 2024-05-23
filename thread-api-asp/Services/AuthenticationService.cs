using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Dynamic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using thread_api_asp.Commons;
using thread_api_asp.Configurations;
using thread_api_asp.Entities;
using thread_api_asp.ViewModels;

namespace thread_api_asp.Services
{
    public interface IAuthenticationService
    {
        public string Login(UserLoginVm input, out TokenVm? result);
        public string RenewToken(TokenVm input, out TokenVm? newToken);
    }

    public class AuthenticationService(
        ThreadsContext context,
        IUserService userService,
        IOptionsMonitor<JwtSettings> jwtSettings) : IAuthenticationService
    {
        public string Login(UserLoginVm input, out TokenVm? result)
        {
            result = null;
            userService.GetUserByUsernameAndPassword(input, out object? output);
            if (output == null) return "Đăng nhập không thành công";
            return GenerateToken((UserVm)output, out result);
        }

        public string RenewToken(TokenVm input, out TokenVm? newToken)
        {
            newToken = null;
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
            var refreshTk = GetRefreshToken(input.RefreshToken);
            if (refreshTk == null) { return "Refresh token không tồn tại"; }
            try
            {
                //Check 1: Kiểm tra access token có đúng định dạng và đã hết hạn chưa, nếu có thì sẽ throw lỗi
                var tokenInVerification = jwtTokenHandler.ValidateToken(input.AccessToken, nonValidateLifeTime, out var validatedAccessToken);
                //Check 2: Kiểm tra thuật toán có khớp hay không
                if (validatedAccessToken is JwtSecurityToken token)
                {
                    if (!token.Header.Alg.Equals(SecurityAlgorithms.HmacSha512, StringComparison.InvariantCultureIgnoreCase))
                        return "Không đúng thuật toán";
                    if (!refreshTk.JwtId.Equals(token.Id)) return "JWT ID của Access token không khớp";
                }
            }
            catch (SecurityTokenExpiredException)
            {
                if (refreshTk.IsRevoked == true) return "Refresh token đã bị thu hồi";
                if (refreshTk.IsUsed == true) return "Refresh token đã được sử dụng";
                if (refreshTk.ExpiredAt < DateTime.Now) return "Refresh token đã hết hạn";
                //Renew token:
                refreshTk.IsRevoked = true;
                refreshTk.IsUsed = true;
                context.RefreshTokens.Update(refreshTk);
                var user = context.Users.SingleOrDefault(x => x.Id == refreshTk.UserId);
                if (user != null)
                {
                    GenerateToken(new UserVm { Id = user.Id, Username = user.Username }, out newToken); 
                    return string.Empty;
                }
            }
            catch (Exception e) { return ("Sai định dạng token"); }
            return "Token chưa hết hạn";
        }

        private RefreshToken? GetRefreshToken(string? accessToken)
        {
            var result = (
                from atk in context.RefreshTokens
                where
                    atk.Token == accessToken
                select atk).FirstOrDefault();
            return result;
        }

        #region Helper

        private string GenerateToken(UserVm userVm, out TokenVm? output)
        {
            output = null;
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
            context.RefreshTokens.Add(refreshTokenEntity);
            string msg = DbHelper.SaveChangeHandleError(context);
            if (msg.Length > 0) { return msg; }
            //Trả về thông tin token
            output = new TokenVm
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
            return string.Empty;
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