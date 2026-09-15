using AspNetCoreTaskManager.Models;

namespace AspNetCoreTaskManager.ViewModels;

public class CalendarViewModel
{
    public IReadOnlyCollection<Project> Projects { get; init; } = [];
    public IReadOnlyCollection<string> ResponsiblePeople { get; init; } = [];
}
