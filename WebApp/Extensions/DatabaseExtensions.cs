using WebApp.Database;
using Microsoft.EntityFrameworkCore;

namespace WebApp.Extensions;

public static class DatabaseExtensions
{
    //async 方法必须返回 Task 或 Task<T>
    //this WebApplication app：扩展方法，扩展 WebApplication 类型
    //this WebApplication app 不是 new
    //this 告诉编译器：这是一个 扩展方法
    //“挂在 WebApplication 类型上，用它的实例来调用这个方法”
    public static async Task ApplyMigrationsAsync(this WebApplication app)
    {
        using IServiceScope scope = app.Services.CreateScope();
        await using ApplicationDbContext dbContext =
            scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        try
        {
            await dbContext.Database.MigrateAsync();
            app.Logger.LogInformation("Database migrations applied successfully.");
        }
        catch (Exception e)
        {
            app.Logger.LogError(e, "An error occurred while applying database migrations.");
            throw;
        }
    }
}
