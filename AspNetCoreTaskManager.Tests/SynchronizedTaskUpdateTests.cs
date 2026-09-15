using AspNetCoreTaskManager.Data;
using AspNetCoreTaskManager.Models;
using AspNetCoreTaskManager.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AspNetCoreTaskManager.Tests;

public class SynchronizedTaskUpdateTests
{
    [Fact]
    public void BoardUpdate_PreservesCalendarDates()
    {
        using var database = CreateDatabase();
        var start = new DateTime(2026, 9, 15, 9, 0, 0);
        var end = start.AddHours(2);
        var stored = new TaskItem { Title = "Anterior", StartAt = start, EndAt = end };
        database.Tasks.Add(stored);
        database.SaveChanges();
        var repository = new EfTaskRepository(database);

        repository.Update(new TaskItem { Id = stored.Id, Title = "Actualizada", Status = WorkStatus.InProgress });

        var updated = database.Tasks.Single();
        Assert.Equal("Actualizada", updated.Title);
        Assert.Equal(start, updated.StartAt);
        Assert.Equal(end, updated.EndAt);
    }

    private static AppDbContext CreateDatabase()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }
}
