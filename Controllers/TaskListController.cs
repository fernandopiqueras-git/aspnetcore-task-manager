using AspNetCoreTaskManager.Data;
using AspNetCoreTaskManager.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AspNetCoreTaskManager.Controllers;

public class TaskListController(AppDbContext database) : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        var model = new CalendarViewModel
        {
            Projects = database.Projects.AsNoTracking().OrderBy(project => project.Name).ToArray(),
            ResponsiblePeople = database.Tasks.AsNoTracking()
                .Where(task => task.AssignedTo != null && task.AssignedTo != string.Empty)
                .Select(task => task.AssignedTo!)
                .Distinct()
                .OrderBy(name => name)
                .ToArray()
        };

        return View(model);
    }
}
