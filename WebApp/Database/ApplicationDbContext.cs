using System.Reflection;
using Microsoft.EntityFrameworkCore;
using WebApp.Entities;

namespace WebApp.Database;

public sealed class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options):DbContext(options)
//sealed 表示 这个类不能被再继承
{
    public DbSet<Habit> Habits { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<HabitTag> HabitTag { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // 配置表、字段、关系、默认值等
        modelBuilder.HasDefaultSchema(Schemas.Application);
        // EF Core 提供的一个方法，可以扫描指定程序集（Assembly）里所有实现了 IEntityTypeConfiguration<T> 的配置类
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);


    }


}
