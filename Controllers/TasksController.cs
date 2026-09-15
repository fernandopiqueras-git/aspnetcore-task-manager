using AspNetCoreTaskManager.Data;
using AspNetCoreTaskManager.Models;
using AspNetCoreTaskManager.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AspNetCoreTaskManager.Controllers;

public class TasksController(ITaskRepository repository, AppDbContext database) : Controller
{
    [HttpGet]
    public IActionResult Index(WorkStatus? status, TaskPriority? priority, string? search)
    {
        var tasks = repository.GetAll().AsEnumerable();
        if (status.HasValue)
            tasks = tasks.Where(item => item.Status == status);
        if (priority.HasValue)
            tasks = tasks.Where(item => item.Priority == priority);
        if (!string.IsNullOrWhiteSpace(search))
            tasks = tasks.Where(item =>
                item.Title.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                (item.Description?.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false));

        PrepareOptions();
        ViewBag.Status = status;
        ViewBag.Priority = priority;
        ViewBag.Search = search;
        return View(tasks.OrderBy(item => item.Status).ThenByDescending(item => item.Priority).ThenBy(item => item.DueDate).ToArray());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(TaskItem item, int[]? tagIds = null)
    {
        ValidateProject(item.ProjectId);
        if (!ModelState.IsValid)
        {
            PrepareOptions(tagIds);
            ViewData["OpenCreateDialog"] = true;
            return View("Index", repository.GetAll());
        }

        SetNewTaskTags(item, tagIds);
        repository.Add(item);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult Details(int id)
    {
        var item = repository.GetById(id);
        return item is null ? NotFound() : View(item);
    }

    [HttpGet]
    public IActionResult Edit(int id)
    {
        var item = repository.GetById(id);
        if (item is null)
            return NotFound();

        PrepareOptions(item.TaskTags.Select(taskTag => taskTag.TagId));
        return View(item);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, TaskItem item, int[]? tagIds = null)
    {
        if (id != item.Id)
            return BadRequest();
        if (repository.GetById(id) is null)
            return NotFound();

        ValidateProject(item.ProjectId);
        if (!ModelState.IsValid)
        {
            PrepareOptions(tagIds);
            return View(item);
        }

        repository.Update(item);
        ReplaceTags(id, tagIds);
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpGet]
    public IActionResult Delete(int id)
    {
        var item = repository.GetById(id);
        return item is null ? NotFound() : View(item);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(int id)
    {
        return repository.Delete(id) ? RedirectToAction(nameof(Index)) : NotFound();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult ChangeStatus(int id, WorkStatus status)
    {
        var item = repository.GetById(id);
        if (item is null)
            return NotFound();

        item.Status = status;
        repository.Update(item);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult AddProject(string? name)
    {
        name = name?.Trim();
        if (string.IsNullOrEmpty(name) || name.Length > 80 || database.Projects.Any(project => project.Name == name))
            return RedirectToAction(nameof(Index));

        database.Projects.Add(new Project { Name = name });
        database.SaveChanges();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult AddTag(string? name)
    {
        name = name?.Trim();
        if (string.IsNullOrEmpty(name) || name.Length > 40 || database.Tags.Any(tag => tag.Name == name))
            return RedirectToAction(nameof(Index));

        database.Tags.Add(new Tag { Name = name });
        database.SaveChanges();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult AddComment(int id, string? text)
    {
        text = text?.Trim();
        if (!database.Tasks.Any(item => item.Id == id))
            return NotFound();
        if (string.IsNullOrEmpty(text) || text.Length > 500)
            return RedirectToAction(nameof(Details), new { id });

        database.Comments.Add(new TaskComment { TaskItemId = id, Text = text });
        database.SaveChanges();
        return RedirectToAction(nameof(Details), new { id });
    }

    private void ValidateProject(int? projectId)
    {
        if (projectId.HasValue && !database.Projects.Any(project => project.Id == projectId))
            ModelState.AddModelError(nameof(TaskItem.ProjectId), "El proyecto seleccionado no existe.");
    }

    private void PrepareOptions(IEnumerable<int>? selectedTagIds = null)
    {
        ViewBag.Projects = database.Projects.AsNoTracking().OrderBy(project => project.Name).ToArray();
        ViewBag.Tags = database.Tags.AsNoTracking().OrderBy(tag => tag.Name).ToArray();
        ViewBag.SelectedTagIds = selectedTagIds?.ToArray() ?? Array.Empty<int>();
    }

    private void SetNewTaskTags(TaskItem item, IEnumerable<int>? tagIds)
    {
        foreach (var tagId in ValidTagIds(tagIds))
            item.TaskTags.Add(new TaskTag { TagId = tagId });
    }

    private void ReplaceTags(int taskId, IEnumerable<int>? tagIds)
    {
        var existing = database.TaskTags.Where(taskTag => taskTag.TaskItemId == taskId);
        database.TaskTags.RemoveRange(existing);
        foreach (var tagId in ValidTagIds(tagIds))
            database.TaskTags.Add(new TaskTag { TaskItemId = taskId, TagId = tagId });
        database.SaveChanges();
    }

    private int[] ValidTagIds(IEnumerable<int>? tagIds)
    {
        var requested = tagIds?.Distinct().ToArray() ?? [];
        return database.Tags.Where(tag => requested.Contains(tag.Id)).Select(tag => tag.Id).ToArray();
    }
}
