using AspNetCoreTaskManager.Controllers;
using AspNetCoreTaskManager.Models;
using AspNetCoreTaskManager.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreTaskManager.Tests;

public class TasksControllerTests
{
    [Fact]
    public void Index_FiltersByStatusPriorityAndSearch()
    {
        var repository = new InMemoryTaskRepository();
        repository.Add(new TaskItem { Title = "Preparar API", Status = WorkStatus.InProgress, Priority = TaskPriority.High });
        repository.Add(new TaskItem { Title = "Documentar", Status = WorkStatus.Pending, Priority = TaskPriority.Low });
        var controller = new TasksController(repository);

        var result = Assert.IsType<ViewResult>(controller.Index(WorkStatus.InProgress, TaskPriority.High, "API"));
        var model = Assert.IsAssignableFrom<IReadOnlyCollection<TaskItem>>(result.Model);

        Assert.Single(model);
        Assert.Equal("Preparar API", model.Single().Title);
    }

    [Fact]
    public void Create_ValidTask_AddsItAndRedirects()
    {
        var repository = new InMemoryTaskRepository();
        var controller = new TasksController(repository);

        var result = controller.Create(new TaskItem { Title = "Nueva" });

        Assert.IsType<RedirectToActionResult>(result);
        Assert.Single(repository.GetAll());
    }

    [Fact]
    public void Create_InvalidTask_DoesNotAddIt()
    {
        var repository = new InMemoryTaskRepository();
        var controller = new TasksController(repository);
        controller.ModelState.AddModelError("Title", "Obligatorio");

        var result = controller.Create(new TaskItem());

        Assert.IsType<ViewResult>(result);
        Assert.Empty(repository.GetAll());
    }

    [Fact]
    public void ChangeStatus_ExistingTask_UpdatesIt()
    {
        var repository = new InMemoryTaskRepository();
        var task = repository.Add(new TaskItem { Title = "Cambiar" });
        var controller = new TasksController(repository);

        var result = controller.ChangeStatus(task.Id, WorkStatus.Completed);

        Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(WorkStatus.Completed, repository.GetById(task.Id)?.Status);
    }

    [Fact]
    public void ChangeStatus_MissingTask_ReturnsNotFound()
    {
        var controller = new TasksController(new InMemoryTaskRepository());
        Assert.IsType<NotFoundResult>(controller.ChangeStatus(99, WorkStatus.Completed));
    }

    [Fact]
    public void Delete_ExistingTask_RemovesIt()
    {
        var repository = new InMemoryTaskRepository();
        var task = repository.Add(new TaskItem { Title = "Eliminar" });
        var controller = new TasksController(repository);

        var result = controller.Delete(task.Id);

        Assert.IsType<RedirectToActionResult>(result);
        Assert.Empty(repository.GetAll());
    }

    [Fact]
    public void Delete_MissingTask_ReturnsNotFound()
    {
        var controller = new TasksController(new InMemoryTaskRepository());
        Assert.IsType<NotFoundResult>(controller.Delete(99));
    }
}
