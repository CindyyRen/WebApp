using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Scalar.AspNetCore;
using WebApp.Database;
using WebApp.DTOs.Habits;
using WebApp.Entities;
using WebApp.Extensions;
using WebApp.middleware;
using WebApp.Services.Sorting;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(options =>
{
    options.ReturnHttpNotAcceptable = true;
})
.AddNewtonsoftJson()
.AddXmlSerializerFormatters();//XmlSerializer 把你的对象（比如 DTO）转成 XML 字符串返回给客户端

builder.Services.AddValidatorsFromAssemblyContaining<Program>();

//AddProblemDetails() 是 ASP.NET Core 8+ 的新统一错误处理机制。
//当应用返回错误（400、404、500 等）时，会返回符合 RFC 7807 的 application/problem+json。
builder.Services.AddProblemDetails(options =>
{
    options.CustomizeProblemDetails = context =>
    {
        context.ProblemDetails.Extensions.TryAdd("requestId", context.HttpContext.TraceIdentifier);
    };
});

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

builder.Services.AddExceptionHandler<ValidationExceptionHandler>();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddTransient<SortMappingProvider>();
builder.Services.AddSingleton<ISortMappingDefinition, SortMappingDefinition<HabitDto, Habit>>(_ =>
    HabitMappings.SortMapping);
WebApplication app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
    await app.ApplyMigrationsAsync();
}

app.UseHttpsRedirection();

app.UseAuthorization();
app.UseExceptionHandler();
app.MapControllers();

await app.RunAsync();
