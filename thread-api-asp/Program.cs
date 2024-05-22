using thread_api_asp.Configurations;

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
            builder.Services.MyConfigureJWT(builder.Configuration);
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
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseHttpsRedirection();

            //RUN
            app.Run();
        }
    }
}
