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

        var result = Assert.IsType<ViewResult>(new TasksController(repository, database).Index(WorkStatus.InProgress, TaskPriority.High, "API"));
        var model = Assert.IsAssignableFrom<IReadOnlyCollection<TaskItem>>(result.Model);

        Assert.Single(model);
        Assert.Equal("Preparar API", model.Single().Title);
    }

    [Fact]
    public void Create_ValidTask_PersistsItAndRedirects()
    {
        using var database = CreateDatabase();
        var result = new TasksController(new EfTaskRepository(database), database).Create(new TaskItem { Title = "Nueva" });

        Assert.IsType<RedirectToActionResult>(result);
        Assert.Single(database.Tasks);
    }

    [Fact]
    public void Create_InvalidTask_DoesNotPersistIt()
    {
        using var database = CreateDatabase();
        var controller = new TasksController(new EfTaskRepository(database), database);
        controller.ModelState.AddModelError("Title", "Obligatorio");

        var result = controller.Create(new TaskItem());

        Assert.IsType<ViewResult>(result);
        Assert.Empty(database.Tasks);
    }

    [Fact]
    public void Details_ExistingTask_ReturnsView()
    {
        using var database = CreateDatabase();
        var repository = new EfTaskRepository(database);
        var task = repository.Add(new TaskItem { Title = "Detalle" });

        var result = Assert.IsType<ViewResult>(new TasksController(repository, database).Details(task.Id));

        Assert.Equal(task.Id, Assert.IsType<TaskItem>(result.Model).Id);
    }

    [Fact]
    public void Details_MissingTask_ReturnsNotFound()
    {
        using var database = CreateDatabase();
        Assert.IsType<NotFoundResult>(new TasksController(new EfTaskRepository(database), database).Details(99));
    }

    [Fact]
    public void EditGet_ExistingTask_ReturnsView()
    {
        using var database = CreateDatabase();
        var repository = new EfTaskRepository(database);
        var task = repository.Add(new TaskItem { Title = "Editar" });

        Assert.IsType<ViewResult>(new TasksController(repository, database).Edit(task.Id));
    }

    [Fact]
    public void EditPost_ValidTask_PersistsChanges()
    {
        using var database = CreateDatabase();
        var repository = new EfTaskRepository(database);
        var task = repository.Add(new TaskItem { Title = "Original" });
        task.Title = "Actualizada";

        var result = new TasksController(repository, database).Edit(task.Id, task);

        Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Actualizada", database.Tasks.AsNoTracking().Single().Title);
    }

    [Fact]
    public void EditPost_WithDifferentIds_ReturnsBadRequest()
    {
        using var database = CreateDatabase();
        var controller = new TasksController(new EfTaskRepository(database), database);
        Assert.IsType<BadRequestResult>(controller.Edit(1, new TaskItem { Id = 2, Title = "Error" }));
    }

    [Fact]
    public void EditPost_InvalidTask_ReturnsViewWithoutChanges()
    {
        using var database = CreateDatabase();
        var repository = new EfTaskRepository(database);
        var task = repository.Add(new TaskItem { Title = "Original" });
        var controller = new TasksController(repository, database);
        controller.ModelState.AddModelError("DueDate", "Fecha no válida");
        task.Title = "No guardar";

        var result = controller.Edit(task.Id, task);

        Assert.IsType<ViewResult>(result);
        Assert.Equal("Original", database.Tasks.AsNoTracking().Single().Title);
    }

    [Fact]
    public void ChangeStatus_ExistingTask_PersistsNewStatus()
    {
        using var database = CreateDatabase();
        var repository = new EfTaskRepository(database);
        var task = repository.Add(new TaskItem { Title = "Cambiar" });

        var result = new TasksController(repository, database).ChangeStatus(task.Id, WorkStatus.Completed);

        Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(WorkStatus.Completed, database.Tasks.AsNoTracking().Single().Status);
    }

    [Fact]
    public void DeleteGet_ExistingTask_ReturnsConfirmationView()
    {
        using var database = CreateDatabase();
        var repository = new EfTaskRepository(database);
        var task = repository.Add(new TaskItem { Title = "Eliminar" });

        Assert.IsType<ViewResult>(new TasksController(repository, database).Delete(task.Id));
        Assert.Single(database.Tasks);
    }

    [Fact]
    public void DeleteConfirmed_ExistingTask_RemovesIt()
    {
        using var database = CreateDatabase();
        var repository = new EfTaskRepository(database);
        var task = repository.Add(new TaskItem { Title = "Eliminar" });

        var result = new TasksController(repository, database).DeleteConfirmed(task.Id);

        Assert.IsType<RedirectToActionResult>(result);
        Assert.Empty(database.Tasks);
    }

    [Fact]
    public void MissingTasks_ReturnNotFound()
    {
        using var database = CreateDatabase();
        var controller = new TasksController(new EfTaskRepository(database), database);

        Assert.IsType<NotFoundResult>(controller.Edit(99));
        Assert.IsType<NotFoundResult>(controller.Delete(99));
        Assert.IsType<NotFoundResult>(controller.DeleteConfirmed(99));
        Assert.IsType<NotFoundResult>(controller.ChangeStatus(99, WorkStatus.Completed));
    }

    private static AppDbContext CreateDatabase()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }
}
