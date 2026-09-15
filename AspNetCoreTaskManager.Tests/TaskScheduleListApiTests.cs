using AspNetCoreTaskManager.Controllers;
using AspNetCoreTaskManager.Data;
using AspNetCoreTaskManager.Dtos;
using AspNetCoreTaskManager.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AspNetCoreTaskManager.Tests;

public class TaskScheduleListApiTests
{
    [Fact]
    public void GetList_ReturnsScheduledAndUnscheduledTasksWithoutExposingEntities()
    {
        using var database = CreateDatabase();
        var project = new Project { Name = "Web" };
        database.Projects.Add(project);
        database.Tasks.AddRange(
            new TaskItem { Title = "Sin fecha", Project = project },
            new TaskItem { Title = "Con fecha", StartAt = new DateTime(2026, 9, 15, 9, 0, 0) });
        database.SaveChanges();

        var result = new TaskScheduleApiController(database).GetList();

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var tasks = Assert.IsAssignableFrom<IReadOnlyCollection<TaskListDto>>(ok.Value);
        Assert.Equal(2, tasks.Count);
        var unscheduled = Assert.Single(tasks, task => task.Title == "Sin fecha");
        Assert.Null(unscheduled.Start);
        Assert.Equal("Web", unscheduled.ProjectName);
    }

    private static AppDbContext CreateDatabase()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }
}
