using AspNetCoreTaskManager.Models;

namespace AspNetCoreTaskManager.Repositories;

public interface ITaskRepository
{
    IReadOnlyCollection<TaskItem> GetAll();
    TaskItem? GetById(int id);
    TaskItem Add(TaskItem item);
    bool Update(TaskItem item);
    bool Delete(int id);
}
