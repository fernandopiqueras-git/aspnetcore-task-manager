using AspNetCoreTaskManager.Controllers;
using AspNetCoreTaskManager.Data;
using AspNetCoreTaskManager.Dtos;
using AspNetCoreTaskManager.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AspNetCoreTaskManager.Tests;

public class TaskScheduleApiControllerTests
{
    [Fact]
    public void Get_ReturnsOnlyTasksThatOverlapTheInterval()
    {
        using var database = CreateDatabase();
        database.Tasks.AddRange(
            new TaskItem { Title = "Incluida", StartAt = At(10), EndAt = At(12) },
            new TaskItem { Title = "Fuera", StartAt = At(14), EndAt = At(15) },
            new TaskItem { Title = "Sin programar" });
        database.SaveChanges();

        var result = new TaskScheduleApiController(database).Get(At(11), At(13));

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var tasks = Assert.IsAssignableFrom<IReadOnlyCollection<TaskCalendarDto>>(ok.Value);
        Assert.Single(tasks);
        Assert.Equal("Incluida", tasks.Single().Title);
    }

    [Fact]
    public void Get_RejectsAnInvalidInterval()
    {
        using var database = CreateDatabase();

        var result = new TaskScheduleApiController(database).Get(At(12), At(11));

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public void Create_PersistsTheScheduleAndReturnsADto()
    {
        using var database = CreateDatabase();
        var project = new Project { Name = "Web" };
        database.Projects.Add(project);
        database.SaveChanges();
        var request = new TaskScheduleRequest
        {
            Title = "  Diseñar API  ",
            Start = At(9),
            End = At(11),
            Priority = TaskPriority.High,
            Status = WorkStatus.InProgress,
            ProjectId = project.Id,
            AssignedTo = "  Fernando  "
        };

        var result = new TaskScheduleApiController(database).Create(request);

        var created = Assert.IsType<CreatedAtActionResult>(result.Result);
        var dto = Assert.IsType<TaskCalendarDto>(created.Value);
        var stored = Assert.Single(database.Tasks);
        Assert.Equal("Diseñar API", dto.Title);
        Assert.Equal(project.Id, dto.ProjectId);
        Assert.Equal("Web", dto.ProjectName);
        Assert.Equal("Fernando", stored.AssignedTo);
        Assert.Equal(At(11).Date, stored.DueDate);
    }

    [Fact]
    public void Update_ModifiesAllEditableScheduleFields()
    {
        using var database = CreateDatabase();
        var item = new TaskItem { Title = "Anterior", StartAt = At(8), EndAt = At(9) };
        database.Tasks.Add(item);
        database.SaveChanges();
        var request = new TaskScheduleRequest
        {
            Title = "Nueva",
            Start = At(12),
            End = At(13),
            Priority = TaskPriority.High,
            Status = WorkStatus.Completed,
            AssignedTo = "Ana"
        };

        var result = new TaskScheduleApiController(database).Update(item.Id, request);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var dto = Assert.IsType<TaskCalendarDto>(ok.Value);
        Assert.Equal("Nueva", dto.Title);
        Assert.Equal(At(12), dto.Start);
        Assert.Equal(At(13), dto.End);
        Assert.Equal(TaskPriority.High, dto.Priority);
        Assert.Equal(WorkStatus.Completed, dto.Status);
        Assert.Equal("Ana", dto.AssignedTo);
    }

    [Fact]
    public void Update_ReturnsNotFoundForAnUnknownTask()
    {
        using var database = CreateDatabase();
        var request = new TaskScheduleRequest { Title = "Nueva", Start = At(12) };

        var result = new TaskScheduleApiController(database).Update(999, request);

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public void Create_RejectsAnUnknownProject()
    {
        using var database = CreateDatabase();
        var request = new TaskScheduleRequest { Title = "Nueva", Start = At(12), ProjectId = 999 };

        var result = new TaskScheduleApiController(database).Create(request);

        Assert.IsType<ObjectResult>(result.Result);
        Assert.Empty(database.Tasks);
    }

    private static AppDbContext CreateDatabase()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    private static DateTime At(int hour) => new(2026, 9, 15, hour, 0, 0);
}
