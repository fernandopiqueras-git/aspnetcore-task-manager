using AspNetCoreTaskManager.Controllers;
using AspNetCoreTaskManager.Data;
using AspNetCoreTaskManager.Models;
using AspNetCoreTaskManager.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AspNetCoreTaskManager.Tests;

public class TaskOrganizationTests
{
    [Fact]
    public void AddProject_WithValidName_PersistsProject()
    {
        using var database = CreateDatabase();
        var controller = Controller(database);

        var result = controller.AddProject("Portal web");

        Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Portal web", database.Projects.Single().Name);
    }

    [Fact]
    public void AddProject_WithDuplicateName_DoesNotDuplicateIt()
    {
        using var database = CreateDatabase();
        database.Projects.Add(new Project { Name = "Portal web" });
        database.SaveChanges();

        Controller(database).AddProject("Portal web");

        Assert.Single(database.Projects);
    }

    [Fact]
    public void AddTag_WithValidName_PersistsTag()
    {
        using var database = CreateDatabase();

        Controller(database).AddTag("backend");

        Assert.Equal("backend", database.Tags.Single().Name);
    }

    [Fact]
    public void Create_WithProjectResponsibleAndTags_PersistsOrganization()
    {
        using var database = CreateDatabase();
        var project = new Project { Name = "ERP" };
        var tag = new Tag { Name = "urgente" };
        database.AddRange(project, tag);
        database.SaveChanges();
        var item = new TaskItem { Title = "Preparar módulo", ProjectId = project.Id, AssignedTo = "Fernando" };

        Controller(database).Create(item, [tag.Id]);

        var stored = database.Tasks
            .AsNoTracking()
            .Include(task => task.Project)
            .Include(task => task.TaskTags).ThenInclude(taskTag => taskTag.Tag)
            .Single();
        Assert.Equal("ERP", stored.Project?.Name);
        Assert.Equal("Fernando", stored.AssignedTo);
        Assert.Equal("urgente", stored.TaskTags.Single().Tag.Name);
    }

    [Fact]
    public void Create_WithMissingProject_ReturnsViewWithoutPersisting()
    {
        using var database = CreateDatabase();
        var result = Controller(database).Create(new TaskItem { Title = "Tarea", ProjectId = 99 }, null);

        Assert.IsType<ViewResult>(result);
        Assert.Empty(database.Tasks);
    }

    [Fact]
    public void Edit_ReplacesAssignedProjectAndTags()
    {
        using var database = CreateDatabase();
        var firstProject = new Project { Name = "Uno" };
        var secondProject = new Project { Name = "Dos" };
        var firstTag = new Tag { Name = "frontend" };
        var secondTag = new Tag { Name = "backend" };
        database.AddRange(firstProject, secondProject, firstTag, secondTag);
        database.SaveChanges();
        var controller = Controller(database);
        var item = new TaskItem { Title = "Tarea", ProjectId = firstProject.Id };
        controller.Create(item, [firstTag.Id]);
        item.ProjectId = secondProject.Id;
        item.AssignedTo = "Ana";

        controller.Edit(item.Id, item, [secondTag.Id]);

        var stored = database.Tasks.AsNoTracking().Include(task => task.TaskTags).Single();
        Assert.Equal(secondProject.Id, stored.ProjectId);
        Assert.Equal("Ana", stored.AssignedTo);
        Assert.Equal(secondTag.Id, stored.TaskTags.Single().TagId);
    }

    [Fact]
    public void AddComment_ToExistingTask_PersistsComment()
    {
        using var database = CreateDatabase();
        var repository = new EfTaskRepository(database);
        var item = repository.Add(new TaskItem { Title = "Tarea" });

        var result = new TasksController(repository, database).AddComment(item.Id, "Primer comentario");

        Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Primer comentario", database.Comments.Single().Text);
    }

    [Fact]
    public void AddComment_ToMissingTask_ReturnsNotFound()
    {
        using var database = CreateDatabase();
        Assert.IsType<NotFoundResult>(Controller(database).AddComment(99, "Comentario"));
    }

    private static TasksController Controller(AppDbContext database)
    {
        return new TasksController(new EfTaskRepository(database), database);
    }

    private static AppDbContext CreateDatabase()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }
}
