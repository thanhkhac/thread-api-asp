using Identity.Configurations;
using Identity.Entities;
using Microsoft.AspNetCore.Identity;

namespace Identity;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        //SERVICE
        builder.Services.ConfigSwagger(); // SWAGGER
        builder.Services.AddEndpointsApiExplorer(); 
        builder.Services.AddControllers();
        builder.Services.AddAuthorization();
        builder.Services.ConfigDb(builder.Configuration); //SERVICES
        builder.Services.ConfigIdentity(); // IDENTITY
        builder.Services.ConfigAuthentication(builder.Configuration);
        builder.Services.AddServices();
        //BUILD
        var app = builder.Build();
        app.UseRouting();
        app.UseSwagger();
        app.UseSwaggerUI();
        app.MapControllers();
        app.UseAuthentication(); //Authentication phải luôn đứng trước Authorization
        app.UseAuthorization();
        app.UseHttpsRedirection();
        //RUN
        app.Run();
    }
}