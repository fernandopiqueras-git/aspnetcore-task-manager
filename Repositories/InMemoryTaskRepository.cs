using AspNetCoreTaskManager.Models;

namespace AspNetCoreTaskManager.Repositories;

public class InMemoryTaskRepository : ITaskRepository
{
    private readonly List<TaskItem> items = [];
    private int nextId = 1;
    private readonly object syncRoot = new();

    public IReadOnlyCollection<TaskItem> GetAll()
    {
        lock (syncRoot)
            return items.Select(Clone).ToArray();
    }

    public TaskItem? GetById(int id)
    {
        lock (syncRoot)
            return items.Where(item => item.Id == id).Select(Clone).FirstOrDefault();
    }

    public TaskItem Add(TaskItem item)
    {
        lock (syncRoot)
        {
            var stored = Clone(item);
            stored.Id = nextId++;
            stored.CreatedAt = DateTime.UtcNow;
            items.Add(stored);
            return Clone(stored);
        }
    }

    public bool Update(TaskItem item)
    {
        lock (syncRoot)
        {
            var index = items.FindIndex(existing => existing.Id == item.Id);
            if (index < 0)
                return false;

            var updated = Clone(item);
            updated.CreatedAt = items[index].CreatedAt;
            items[index] = updated;
            return true;
        }
    }

    public bool Delete(int id)
    {
        lock (syncRoot)
            return items.RemoveAll(item => item.Id == id) > 0;
    }

    private static TaskItem Clone(TaskItem item)
    {
        return new TaskItem
        {
            Id = item.Id,
            Title = item.Title,
            Description = item.Description,
            Priority = item.Priority,
            Status = item.Status,
            DueDate = item.DueDate,
            CreatedAt = item.CreatedAt
        };
    }
}
