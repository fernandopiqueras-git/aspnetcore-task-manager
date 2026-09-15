using AspNetCoreTaskManager.Controllers;
using AspNetCoreTaskManager.Data;
using AspNetCoreTaskManager.Models;
using AspNetCoreTaskManager.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AspNetCoreTaskManager.Tests;

public class CalendarControllerTests
{
    [Fact]
    public void Index_ReturnsProjectsAndDistinctResponsiblePeople()
    {
        using var database = CreateDatabase();
        database.Projects.AddRange(new Project { Name = "Segundo" }, new Project { Name = "Primero" });
        database.Tasks.AddRange(
            new TaskItem { Title = "Uno", AssignedTo = "Fernando" },
            new TaskItem { Title = "Dos", AssignedTo = "Fernando" },
            new TaskItem { Title = "Tres", AssignedTo = "Ana" });
        database.SaveChanges();

        var result = new CalendarController(database).Index();

        var view = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<CalendarViewModel>(view.Model);
        Assert.Equal(["Primero", "Segundo"], model.Projects.Select(project => project.Name));
        Assert.Equal(["Ana", "Fernando"], model.ResponsiblePeople);
    }

    private static AppDbContext CreateDatabase()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }
}
