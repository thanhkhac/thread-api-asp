using System.Text;
using Microsoft.OpenApi.Models;

namespace Base.Configurations
{
    public static class ServiceConfiguration
    {
        public static void MyConfigureService(this IServiceCollection services)
        {

            
        }

        public static void MyConfigureSwaggerAuthentication(this IServiceCollection services)
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