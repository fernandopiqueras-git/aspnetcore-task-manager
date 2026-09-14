using AspNetCoreTaskManager.Models;
using AspNetCoreTaskManager.Repositories;

namespace AspNetCoreTaskManager.Tests;

public class InMemoryTaskRepositoryTests
{
    [Fact]
    public void Add_AssignsIdAndStoresTask()
    {
        var repository = new InMemoryTaskRepository();
        var result = repository.Add(new TaskItem { Title = "Primera" });

        Assert.Equal(1, result.Id);
        Assert.Equal("Primera", repository.GetById(result.Id)?.Title);
    }

    [Fact]
    public void GetAll_ReturnsCopies()
    {
        var repository = new InMemoryTaskRepository();
        repository.Add(new TaskItem { Title = "Original" });

        repository.GetAll().Single().Title = "Modificada";

        Assert.Equal("Original", repository.GetAll().Single().Title);
    }

    [Fact]
    public void Update_ExistingTask_ReturnsTrueAndPreservesCreationDate()
    {
        var repository = new InMemoryTaskRepository();
        var original = repository.Add(new TaskItem { Title = "Original" });
        original.Title = "Actualizada";

        var result = repository.Update(original);

        Assert.True(result);
        Assert.Equal("Actualizada", repository.GetById(original.Id)?.Title);
        Assert.Equal(original.CreatedAt, repository.GetById(original.Id)?.CreatedAt);
    }

    [Fact]
    public void Update_MissingTask_ReturnsFalse()
    {
        var repository = new InMemoryTaskRepository();
        Assert.False(repository.Update(new TaskItem { Id = 99, Title = "No existe" }));
    }

    [Fact]
    public void Delete_ExistingTask_RemovesIt()
    {
        var repository = new InMemoryTaskRepository();
        var task = repository.Add(new TaskItem { Title = "Eliminar" });

        Assert.True(repository.Delete(task.Id));
        Assert.Null(repository.GetById(task.Id));
    }

    [Fact]
    public void Delete_MissingTask_ReturnsFalse()
    {
        Assert.False(new InMemoryTaskRepository().Delete(99));
    }
}
