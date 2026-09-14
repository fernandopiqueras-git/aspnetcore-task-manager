using AspNetCoreTaskManager.Controllers;
using AspNetCoreTaskManager.Data;
using AspNetCoreTaskManager.Models;
using AspNetCoreTaskManager.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AspNetCoreTaskManager.Tests;

public class TasksControllerTests
{
    [Fact]
    public void Index_FiltersByStatusPriorityAndSearch()
    {
        using var database = CreateDatabase();
        var repository = new EfTaskRepository(database);
        repository.Add(new TaskItem { Title = "Preparar API", Status = WorkStatus.InProgress, Priority = TaskPriority.High });
        repository.Add(new TaskItem { Title = "Documentar", Status = WorkStatus.Pending, Priority = TaskPriority.Low });
        var controller = new TasksController(repository);

        var result = Assert.IsType<ViewResult>(controller.Index(WorkStatus.InProgress, TaskPriority.High, "API"));
        var model = Assert.IsAssignableFrom<IReadOnlyCollection<TaskItem>>(result.Model);

        Assert.Single(model);
        Assert.Equal("Preparar API", model.Single().Title);
    }

    [Fact]
    public void Create_ValidTask_PersistsItAndRedirects()
    {
        using var database = CreateDatabase();
        var controller = new TasksController(new EfTaskRepository(database));

        var result = controller.Create(new TaskItem { Title = "Nueva" });

        Assert.IsType<RedirectToActionResult>(result);
        Assert.Single(database.Tasks);
    }

    [Fact]
    public void Create_InvalidTask_DoesNotPersistIt()
    {
        using var database = CreateDatabase();
        var controller = new TasksController(new EfTaskRepository(database));
        controller.ModelState.AddModelError("Title", "Obligatorio");

        var result = controller.Create(new TaskItem());

        Assert.IsType<ViewResult>(result);
        Assert.Empty(database.Tasks);
    }

    [Fact]
    public void ChangeStatus_ExistingTask_PersistsNewStatus()
    {
        using var database = CreateDatabase();
        var repository = new EfTaskRepository(database);
        var task = repository.Add(new TaskItem { Title = "Cambiar" });
        var controller = new TasksController(repository);

        var result = controller.ChangeStatus(task.Id, WorkStatus.Completed);

        Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(WorkStatus.Completed, database.Tasks.AsNoTracking().Single().Status);
    }

    [Fact]
    public void ChangeStatus_MissingTask_ReturnsNotFound()
    {
        using var database = CreateDatabase();
        var controller = new TasksController(new EfTaskRepository(database));
        Assert.IsType<NotFoundResult>(controller.ChangeStatus(99, WorkStatus.Completed));
    }

    [Fact]
    public void Delete_ExistingTask_RemovesIt()
    {
        using var database = CreateDatabase();
        var repository = new EfTaskRepository(database);
        var task = repository.Add(new TaskItem { Title = "Eliminar" });
        var controller = new TasksController(repository);

        var result = controller.Delete(task.Id);

        Assert.IsType<RedirectToActionResult>(result);
        Assert.Empty(database.Tasks);
    }

    [Fact]
    public void Delete_MissingTask_ReturnsNotFound()
    {
        using var database = CreateDatabase();
        var controller = new TasksController(new EfTaskRepository(database));
        Assert.IsType<NotFoundResult>(controller.Delete(99));
    }

    private static AppDbContext CreateDatabase()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }
}
