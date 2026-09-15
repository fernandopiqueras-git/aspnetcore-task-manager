using AspNetCoreTaskManager.Data;
using AspNetCoreTaskManager.Models;
using Microsoft.EntityFrameworkCore;

namespace AspNetCoreTaskManager.Repositories;

public class EfTaskRepository(AppDbContext database) : ITaskRepository
{
    public AppDbContext Database => database;

    public IReadOnlyCollection<TaskItem> GetAll()
    {
        return TaskQuery().ToArray();
    }

    public TaskItem? GetById(int id)
    {
        return TaskQuery().FirstOrDefault(item => item.Id == id);
    }

    public TaskItem Add(TaskItem item)
    {
        item.Id = 0;
        item.CreatedAt = DateTime.UtcNow;
        database.Tasks.Add(item);
        database.SaveChanges();
        database.Entry(item).State = EntityState.Detached;
        return item;
    }

    public bool Update(TaskItem item)
    {
        var stored = database.Tasks.FirstOrDefault(existing => existing.Id == item.Id);
        if (stored is null)
            return false;

        var createdAt = stored.CreatedAt;
        var startAt = stored.StartAt;
        var endAt = stored.EndAt;
        database.Entry(stored).CurrentValues.SetValues(item);
        stored.CreatedAt = createdAt;
        stored.StartAt = startAt;
        stored.EndAt = endAt;
        database.SaveChanges();
        return true;
    }

    public bool Delete(int id)
    {
        var item = database.Tasks.Find(id);
        if (item is null)
            return false;

        database.Tasks.Remove(item);
        database.SaveChanges();
        return true;
    }

    private IQueryable<TaskItem> TaskQuery()
    {
        return database.Tasks
            .AsNoTracking()
            .Include(item => item.Project)
            .Include(item => item.TaskTags).ThenInclude(taskTag => taskTag.Tag)
            .Include(item => item.Comments);
    }
}
