using WebApp.Entities;

namespace WebApp.DTOs.Habits;

public sealed record HabitsCollectionDto
{
    public List<HabitDto> Data { get; init; }
}
