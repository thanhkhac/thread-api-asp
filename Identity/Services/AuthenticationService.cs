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
        public ServiceResult Login(LoginVm input, out TokenVm? result);
        public ServiceResult RefreshToken(RefreshTokenVm input, out TokenVm? output);
    }

    public class AuthenticationService(
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        RoleManager<IdentityRole> roleManager,
        IConfiguration configuration,
        IRefreshTokenRepository refreshTokenRepository
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
            return ServiceResult.Error(result.Errors.FirstOrDefault()?.Description);
        }
        
        public ServiceResult Login(LoginVm input, out TokenVm? result)
        {
            result = null;
            var signInResult = signInManager.PasswordSignInAsync(input.UserName, input.Password, true, true).Result;
            if (signInResult.IsLockedOut) return ServiceResult.Ok("Your account has been locked");
            if (signInResult.IsLockedOut) return ServiceResult.Ok("Your account is locked");
            if (!signInResult.Succeeded) return ServiceResult.Ok("Sign in failed");
            var user = userManager.FindByEmailAsync(input.UserName).Result;
            if (user == null) user = userManager.FindByNameAsync(input.UserName).Result;
            if (user == null) return ServiceResult.Error("Your account isn't existed");
            result = GenerateToken(user);
            return ServiceResult.Ok("Đăng nhập thành công");
        }

        // public ServiceResult RefreshToken(RefreshTokenVm input, out TokenVm? output)
        //     {
        //         output = null;
        //         var jwtTokenHandler = new JwtSecurityTokenHandler();
        //         var secretKeyBytes = Encoding.UTF8.GetBytes(configuration["JwtSettings:SecretKey"]!);
        //         var nonValidateLifeTime = new TokenValidationParameters
        //         {
        //             ValidateIssuer = false,
        //             ValidateAudience = false,
        //             ValidateIssuerSigningKey = true,
        //             IssuerSigningKey = new SymmetricSecurityKey(secretKeyBytes),
        //             ClockSkew = TimeSpan.Zero,
        //         };
        //         var refreshTk = refreshTokenRepository.GetRefreshToken(input.RefreshToken);
        //         if (refreshTk == null) { return ServiceResult.Error("Refresh token không tồn tại"); }
        //         try
        //         {
        //             //Check 1: Kiểm tra access token có đúng định dạng và đã hết hạn chưa, nếu có thì sẽ throw lỗi
        //             jwtTokenHandler.ValidateToken(input.AccessToken, nonValidateLifeTime, out var validatedAccessToken);
        //             //Check 2: Kiểm tra thuật toán có khớp hay không
        //             if (validatedAccessToken is JwtSecurityToken token)
        //             {
        //                 if (!token.Header.Alg.Equals(SecurityAlgorithms.HmacSha512, StringComparison.InvariantCultureIgnoreCase))
        //                     ServiceResult.Error("Không đúng thuật toán");
        //                 if (!refreshTk.JwtId.Equals(token.Id)) ServiceResult.Error("JWT ID của Access token không khớp");
        //             }
        //         }
        //         catch (SecurityTokenExpiredException)
        //         {
        //             if(refreshTk.UserId == null) return ServiceResult.Error("Your account doesn't exist");
        //             if (refreshTk.IsRevoked == true) return ServiceResult.Error("Refresh token was revoked");
        //             if (refreshTk.IsUsed == true) return ServiceResult.Error("Refresh token was used");
        //             if (refreshTk.ExpiredAt < DateTime.Now) return ServiceResult.Error("Refresh token was expried");
        //             //Renew token:
        //             refreshTk.IsRevoked = true;
        //             refreshTk.IsUsed = true;
        //             refreshTokenRepository.Update(refreshTk);
        //             var user = userManager.FindByIdAsync(refreshTk.UserId).Result;
        //             if (user == null) return ServiceResult.Error("User doesn;t exist");
        //             output = GenerateToken(user);
        //             return ServiceResult.Ok("Refresh successfully");
        //         }
        //         catch (Exception e)
        //         {
        //             return ServiceResult.Error("Error");
        //         }
        //         return ServiceResult.Error("Your access token hasn't expired");
        //     }

        public ServiceResult RefreshToken(RefreshTokenVm input, out TokenVm? output)
        {
            output = null;
            var refreshTk = refreshTokenRepository.GetRefreshToken(input.RefreshToken);
            if(refreshTk == null) return ServiceResult.Error("Access token doesn't exist");
            if (refreshTk.UserId == null) return ServiceResult.Error("Your account doesn't exist");
            if (refreshTk.IsRevoked == true) return ServiceResult.Error("Refresh token was revoked");
            if (refreshTk.IsUsed == true) return ServiceResult.Error("Refresh token was used");
            if (refreshTk.ExpiredAt < DateTime.Now) return ServiceResult.Error("Refresh token was expried");
            //Renew token:
            refreshTk.IsUsed = true;
            refreshTokenRepository.Update(refreshTk);
            var user = userManager.FindByIdAsync(refreshTk.UserId).Result;
            if (user == null) return ServiceResult.Error("User doesn't exist");
            output = GenerateToken(user);
            return ServiceResult.Ok("Refresh successfully");
        }


        private TokenVm GenerateToken(User user)
        {
            //Chuyển secret key thành byte để mã hóa
            var secretKeyBytes = Encoding.UTF8.GetBytes(configuration["JwtSettings:SecretKey"]!);
            //Tạo thông tin JWT (Json Web Token)
            var userClaims = userManager.GetClaimsAsync(user).Result;
            var roles = userManager.GetRolesAsync(user).Result;

            // Lấy các claims từ roles
            var roleClaims = new List<Claim>();
            foreach (var roleName in roles)
            {
                var role = roleManager.FindByNameAsync(roleName).Result;
                if (role != null)
                {
                    var roleClaimsList = roleManager.GetClaimsAsync(role).Result;
                    roleClaims.AddRange(roleClaimsList);
                }
            }
            var tokenDescription = new SecurityTokenDescriptor
            {
                //Các claim để client có lấy ra thông tin (nếu cần)
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(nameof(user.Id), user.Id),
                    new Claim(nameof(user.UserName), user.UserName ?? throw new InvalidOperationException()),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                }.Union(userClaims).Union(roleClaims)),
                Expires = DateTime.UtcNow.AddSeconds(60),
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
                RefreshToken = refreshToken,
                Expired = 60
            };
        }

        private string GenerateRefreshToken()
        {
            //Khởi tạo một mảng byte với 32 ô
            var random = new byte[32];
            //Khởi tạo một biến "Tạo số ngẫu nhiên"
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(random); //Điền vào các ô của array bằng các byte ngẫu nhiên
            //Trả về chuỗi random được mã hóa về base 64
            return Convert.ToBase64String(random) + Guid.NewGuid();
        }
    }
}