using thread_api_asp.Configurations;
using thread_api_asp.Entities;

namespace thread_api_asp
{
    public class Program
    {
        public static void Main(string[] args)
        {
        
            var builder = WebApplication.CreateBuilder(args);
            //SERVICE
            builder.Services.AddControllers();
            builder.Services.AddSwaggerGen(); // Add the Swagger generator to the services collection in Program.cs:
            builder.Services.AddEndpointsApiExplorer();
            //SERVICE-ENTITY_SERVICE
            builder.Services.MyConfigureService();
            builder.Services.MyConfigureJwt(builder.Configuration);
            builder.Services.MyConfigureSwaggerAuthentication();

            //Commons
            builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
            //BUILD
            var app = builder.Build();
            // Enable the middleware for serving the generated JSON document and the Swagger UI

            app.UseRouting();
            app.UseSwagger();
            app.UseSwaggerUI();
            app.MapControllers();
            app.UseAuthentication(); //Authentication phải luôn đứng trước Authorization
            app.UseAuthorization();
            app.UseHttpsRedirection();
            app.MapIdentityApi<User>();
            //RUN
            app.Run();
        }
    }
}
