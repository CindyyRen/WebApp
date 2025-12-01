using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Scalar.AspNetCore;
using WebApp.Database;
using WebApp.Entities;
using WebApp.Extensions;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(options =>
{
    options.ReturnHttpNotAcceptable = true;
})
.AddNewtonsoftJson()
.AddXmlSerializerFormatters();//XmlSerializer 把你的对象（比如 DTO）转成 XML 字符串返回给客户端

builder.Services.AddOpenApi();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("Database"),
        npgsqlOptions => npgsqlOptions.MigrationsHistoryTable(
            HistoryRepository.DefaultTableName,   // "__EFMigrationsHistory"
            Schemas.Application                    // 你的 schema
        )
    )
    .UseSnakeCaseNamingConvention()
);


WebApplication app = builder.Build();
// 自动 seed 数据
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    //await db.Database.MigrateAsync(); // 先确保数据库更新到最新 migration

    if (!await db.Habits.AnyAsync())
    {
        db.Habits.AddRange(
            new Habit
            {
                Id = Guid.NewGuid().ToString(),
                Name = "每日阅读",
                Description = "每天至少阅读30分钟",
                Type = HabitType.Binary,
                Frequency = new Frequency { Type = FrequencyType.Daily, TimesPerPeriod = 1 },
                Target = new Target { Value = 30, Unit = "分钟" },
                Status = HabitStatus.Ongoing,
                IsArchived = false,
                Milestone = new Milestone { Target = 100, Current = 15 },
                CreatedAtUtc = DateTime.UtcNow.AddDays(-15),
                UpdatedAtUtc = DateTime.UtcNow.AddDays(-1),
                LastCompletedAtUtc = DateTime.UtcNow.AddDays(-1)
            },
            new Habit
            {
                Id = Guid.NewGuid().ToString(),
                Name = "健身打卡",
                Description = "每周至少去健身房3次",
                Type = HabitType.Measurable,
                Frequency = new Frequency { Type = FrequencyType.Weekly, TimesPerPeriod = 3 },
                Target = new Target { Value = 3, Unit = "次" },
                Status = HabitStatus.Ongoing,
                IsArchived = false,
                EndDate = new DateOnly(2025, 12, 31),
                Milestone = new Milestone { Target = 150, Current = 42 },
                CreatedAtUtc = DateTime.UtcNow.AddDays(-30),
                UpdatedAtUtc = DateTime.UtcNow.AddDays(-2),
                LastCompletedAtUtc = DateTime.UtcNow.AddDays(-2)
            },
            new Habit
            {
                Id = Guid.NewGuid().ToString(),
                Name = "写作计划",
                Description = "每月完成2篇文章",
                Type = HabitType.Measurable,
                Frequency = new Frequency { Type = FrequencyType.Monthly, TimesPerPeriod = 2 },
                Target = new Target { Value = 2, Unit = "篇" },
                Status = HabitStatus.Completed,
                IsArchived = true,
                EndDate = new DateOnly(2025, 10, 31),
                Milestone = new Milestone { Target = 24, Current = 24 },
                CreatedAtUtc = DateTime.UtcNow.AddDays(-365),
                UpdatedAtUtc = DateTime.UtcNow.AddDays(-60),
                LastCompletedAtUtc = DateTime.UtcNow.AddDays(-60)
            }
        );

        await db.SaveChangesAsync();
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
    await app.ApplyMigrationsAsync();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

await app.RunAsync();
