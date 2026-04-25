using ExaminationSystem.API.Middleware;
using ExaminationSystem.Application.Interfaces;
using ExaminationSystem.Domain.Entities;
using ExaminationSystem.Persistence.Repositories;
using ExaminationSystem.Persistence.Services;
using Microsoft.AspNetCore.Identity;
using Scalar.AspNetCore;
using Serilog;
namespace ExaminationSystem.API;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Host.UseSerilog((context, loggerConfiguration) =>
            loggerConfiguration.ReadFrom.Configuration(context.Configuration));

        builder.Services.AddApiDependencies(builder.Configuration);
        builder.Services.AddApplication();
        builder.Services.AddPersistence(builder.Configuration);
        // builder.Services.AddPersistence(builder.Configuration);
        builder.Services.AddMemoryCache();


        var app = builder.Build();
        app.UseSerilogRequestLogging();
        app.UseMiddleware<ExceptionHandlingMiddleware>();

        app.ApplyDatabaseMigrations();


        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.MapScalarApiReference(options =>
            {
                options
                    .WithTitle("Examination System")
                    .WithTheme(ScalarTheme.Purple)
                    .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
            });
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRateLimiter();

        app.UseAuthentication();
        app.UseAuthorization();


        app.MapControllers();

        app.Run();
    }
}
