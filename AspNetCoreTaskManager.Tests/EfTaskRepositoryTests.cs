using AspNetCoreTaskManager.Data;
using AspNetCoreTaskManager.Models;
using AspNetCoreTaskManager.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AspNetCoreTaskManager.Tests;

public class EfTaskRepositoryTests
{
    [Fact]
    public void Add_AssignsIdAndPersistsTask()
    {
        using var database = CreateDatabase();
        var repository = new EfTaskRepository(database);

        var result = repository.Add(new TaskItem { Title = "Primera" });

        Assert.True(result.Id > 0);
        Assert.Equal("Primera", database.Tasks.AsNoTracking().Single().Title);
    }

    [Fact]
    public void GetAll_ReturnsPersistedTasks()
    {
        using var database = CreateDatabase();
        database.Tasks.AddRange(new TaskItem { Title = "Una" }, new TaskItem { Title = "Dos" });
        database.SaveChanges();

        var result = new EfTaskRepository(database).GetAll();

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public void GetById_ReturnsRequestedTask()
    {
        using var database = CreateDatabase();
        var stored = new TaskItem { Title = "Encontrada" };
        database.Tasks.Add(stored);
        database.SaveChanges();

        var result = new EfTaskRepository(database).GetById(stored.Id);

        Assert.Equal("Encontrada", result?.Title);
    }

    [Fact]
    public void Update_ExistingTask_PersistsChangesAndCreationDate()
    {
        using var database = CreateDatabase();
        var repository = new EfTaskRepository(database);
        var stored = repository.Add(new TaskItem { Title = "Original" });
        var createdAt = stored.CreatedAt;
        stored.Title = "Actualizada";
        stored.CreatedAt = DateTime.MinValue;

        var result = repository.Update(stored);

        Assert.True(result);
        var updated = database.Tasks.AsNoTracking().Single();
        Assert.Equal("Actualizada", updated.Title);
        Assert.Equal(createdAt, updated.CreatedAt);
    }

    [Fact]
    public void Update_MissingTask_ReturnsFalse()
    {
        using var database = CreateDatabase();
        Assert.False(new EfTaskRepository(database).Update(new TaskItem { Id = 99, Title = "No existe" }));
    }

    [Fact]
    public void Delete_ExistingTask_RemovesIt()
    {
        using var database = CreateDatabase();
        var repository = new EfTaskRepository(database);
        var stored = repository.Add(new TaskItem { Title = "Eliminar" });

        Assert.True(repository.Delete(stored.Id));
        Assert.Empty(database.Tasks);
    }

    [Fact]
    public void Delete_MissingTask_ReturnsFalse()
    {
        using var database = CreateDatabase();
        Assert.False(new EfTaskRepository(database).Delete(99));
    }

    private static AppDbContext CreateDatabase()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }
}
