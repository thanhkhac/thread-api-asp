using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Identity.Entities;
using Identity.Repositories;
using Identity.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace Identity.Services
{
    public interface IAuthenticationService
    {
        public ServiceResult SignUp(SignUpVm input, out IdentityResult result);
        public ServiceResult SignIn(SignInVm input, out TokenVm? result);
    }

    public class AuthenticationService(
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        IConfiguration configuration,
        IRefreshTokenRepository refreshTokenRepository,
        IUserRepository userRepository
    ) : IAuthenticationService
    {

        public ServiceResult SignUp(SignUpVm input, out IdentityResult result)
        {
            var user = new User
            {
                UserName = input.UserName,
                Email = input.Email,
            };
            result = userManager.CreateAsync(user, input.Password).Result;
            if (!result.Succeeded) return ServiceResult.Ok("Create account successfully");
            return ServiceResult.Error(result.Errors.First().Description);
        }
        public ServiceResult SignIn(SignInVm input, out TokenVm? result)
        {
            result = null;
            var signInResult = signInManager.PasswordSignInAsync(input.UserName, input.Password, true, true).Result;
            if (signInResult.IsLockedOut) return ServiceResult.Ok("Your account has been locked");
            if (!signInResult.Succeeded) return ServiceResult.Ok("Sign in failed");
            var user = userManager.FindByEmailAsync(input.UserName).Result;
            if (user == null) return ServiceResult.Error("Your account isn't existed");
            result = GenerateToken(user);
            return ServiceResult.Ok("Đăng nhập thành công");
        }


        private TokenVm GenerateToken(User user)
        {
            //Chuyển secret key thành byte để mã hóa
            var secretKeyBytes = Encoding.UTF8.GetBytes(configuration["JwtSettings:SecretKey"]!);
            //Tạo thông tin JWT (Json Web Token)
            var tokenDescription = new SecurityTokenDescriptor
            {
                //Các claim để client có lấy ra thông tin (nếu cần)
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(nameof(user.Id), user.Id),
                    new Claim(nameof(user.UserName), user.UserName ?? throw new InvalidOperationException()),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                }),
                Expires = DateTime.UtcNow.AddMinutes(6),
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
                UserId = user.Id,
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
    }
}