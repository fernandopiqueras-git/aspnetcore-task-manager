using AspNetCoreTaskManager.Models;
using AspNetCoreTaskManager.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreTaskManager.Controllers;

public class TasksController(ITaskRepository repository) : Controller
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

        ViewBag.Status = status;
        ViewBag.Priority = priority;
        ViewBag.Search = search;
        return View(tasks.OrderBy(item => item.Status).ThenByDescending(item => item.Priority).ThenBy(item => item.DueDate).ToArray());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(TaskItem item)
    {
        if (!ModelState.IsValid)
        {
            ViewData["OpenCreateDialog"] = true;
            return View("Index", repository.GetAll());
        }

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
        return item is null ? NotFound() : View(item);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, TaskItem item)
    {
        if (id != item.Id)
            return BadRequest();
        if (repository.GetById(id) is null)
            return NotFound();
        if (!ModelState.IsValid)
            return View(item);

        repository.Update(item);
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
        if (!repository.Delete(id))
            return NotFound();

        return RedirectToAction(nameof(Index));
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
}
