using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Dynamic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using thread_api_asp.Configurations;
using thread_api_asp.ViewModels;

namespace thread_api_asp.Services
{
    public interface IAuthenticationService
    {
        public string GetAuthenticationResult(UserLoginVm input, out object? result);
    }

    public class AuthenticationService(IUserService userService, IOptionsMonitor<JwtSettings> jwtSettings) : IAuthenticationService
    {
        public string GetAuthenticationResult(UserLoginVm input, out object? result)
        {
            result = new ExpandoObject();
            userService.GetUserByUsernameAndPassword(input, out object? output);
            if (output == null) return "Đăng nhập không thành công";
            if (output is UserVm vm) //Câu lệnh kiểm tra xem output có phải là instance 
                result = GenerateToken(vm);
            return string.Empty;
        }

        #region Helper

        private TokenVm GenerateToken(UserVm userVm)
        {
            //Chuyển secret key thành byte để mã hóa
            var secretKeyBytes = Encoding.UTF8.GetBytes(jwtSettings.CurrentValue.SecretKey);
            var tokenDescription = new SecurityTokenDescriptor
            {
                //Các claim để client có lấy ra thông tin (nếu cần)
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(nameof(userVm.Id), userVm.Id),
                    new Claim(nameof(userVm.Username), userVm.Username),
                }),
                Expires = DateTime.UtcNow.AddYears(5),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKeyBytes), SecurityAlgorithms.HmacSha512Signature)
            };
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var token = jwtTokenHandler.CreateToken(tokenDescription);
            var accessToken =  jwtTokenHandler.WriteToken(token);
            
            return new TokenVm
            {
                AccessToken = accessToken,
                RefreshToken = GenerateRefreshToken ()
            };
        }
        
        private string GenerateRefreshToken ()
        {
            var random = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(random);
                return Convert.ToBase64String(random);
            }
        }

        #endregion

    }
}