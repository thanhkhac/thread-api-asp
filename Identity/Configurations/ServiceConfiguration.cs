using System.Configuration;
using System.Text;
using Identity.Entities;
using Identity.Repositories;
using Identity.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace Identity.Configurations
{
    public static class ServiceConfiguration
    {
        public static void AddServices(this IServiceCollection services)
        {
            services.AddScoped<SignInManager<User>>();
            services.AddScoped<UserManager<User>>();
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        }
        public static void ConfigDb(this IServiceCollection services, IConfiguration configuration)
        {
            //Cấu hình DBContext
            services.AddDbContext<MyIdentityDbContext>(options =>
                options.UseMySQL(configuration.GetConnectionString("DefaultConnection")!));
        }

        public static void ConfigIdentity(this IServiceCollection services)
        {
            services.AddIdentity<User, IdentityRole>(option =>
                {
                    option.Password.RequireUppercase = false;
                    option.Password.RequireNonAlphanumeric = false;
                    option.Password.RequireDigit = false;
                    option.Password.RequiredLength = 6;
                    option.Password.RequireLowercase = false;
                }
            ).AddEntityFrameworkStores<MyIdentityDbContext>().AddDefaultTokenProviders();
        }

        public static void ConfigAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var secretKey = configuration["JwtSettings:SecretKey"];
            services.AddAuthentication(option =>
            {
                option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                option.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(
                option =>
                {
                    option.SaveToken = true;
                    option.RequireHttpsMetadata = false;
                    option.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey ?? throw new InvalidOperationException())),
                        ClockSkew = TimeSpan.Zero //Độ lệch thời gian cho token
                    };
                }
            );
            // 
            // services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(
            //     option =>
            //     {
            //         option.TokenValidationParameters = new TokenValidationParameters
            //         {
            //             ValidateIssuer = false, 
            //             ValidateAudience = false,
            //             ValidateIssuerSigningKey = true,
            //             IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey ?? throw new InvalidOperationException())),
            //             ClockSkew = TimeSpan.Zero //Độ lệch thời gian cho token
            //         };
            //     }
            // );
        }

        public static void ConfigSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(option =>
            {
                option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });
                option.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] { }
                    }
                });
            });
        }


    }
}