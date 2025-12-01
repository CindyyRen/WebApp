using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApp.Entities;

namespace WebApp.Database.Configurations;

public sealed class HabitConfiguration : IEntityTypeConfiguration<Habit>
{
    //Configure是IEntityTypeConfiguration里写好的方法。
    public void Configure(EntityTypeBuilder<Habit> builder)
    {
        builder.HasKey(h => h.Id);

        builder.Property(h => h.Id).HasMaxLength(500);

        builder.Property(h => h.Name).HasMaxLength(100);

        builder.Property(h => h.Description).HasMaxLength(500);

        builder.OwnsOne(h => h.Frequency);
        builder.OwnsOne(h => h.Target, targetBuilder =>
        {
            targetBuilder.Property(t => t.Unit).HasMaxLength(100);
        });
        builder.OwnsOne(h => h.Milestone);

        // Seed 数据 (注意：枚举转 int，owned 类型扁平化)
      
    }
}
