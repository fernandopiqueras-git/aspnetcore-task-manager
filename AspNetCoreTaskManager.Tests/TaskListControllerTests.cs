using AspNetCoreTaskManager.Controllers;
using AspNetCoreTaskManager.Data;
using AspNetCoreTaskManager.Models;
using AspNetCoreTaskManager.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AspNetCoreTaskManager.Tests;

public class TaskListControllerTests
{
    [Fact]
    public void Index_ReturnsProjectsAndResponsiblePeopleForEditing()
    {
        using var database = CreateDatabase();
        database.Projects.Add(new Project { Name = "ERP" });
        database.Tasks.Add(new TaskItem { Title = "Planificar", AssignedTo = "Fernando" });
        database.SaveChanges();

        var result = new TaskListController(database).Index();

        var view = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<CalendarViewModel>(view.Model);
        Assert.Equal("ERP", Assert.Single(model.Projects).Name);
        Assert.Equal("Fernando", Assert.Single(model.ResponsiblePeople));
    }

    private static AppDbContext CreateDatabase()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }
}
