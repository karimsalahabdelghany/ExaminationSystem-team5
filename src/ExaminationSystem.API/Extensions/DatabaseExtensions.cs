using ExaminationSystem.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ExaminationSystem.API.Extensions;

public static class DatabaseExtensions
{
    public static WebApplication ApplyDatabaseMigrations(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        ApplicationContext? dbContext = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
        dbContext.Database.Migrate();

        return app;
    }
}
