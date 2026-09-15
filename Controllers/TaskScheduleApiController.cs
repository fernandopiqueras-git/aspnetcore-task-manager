using AspNetCoreTaskManager.Data;
using AspNetCoreTaskManager.Dtos;
using AspNetCoreTaskManager.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AspNetCoreTaskManager.Controllers;

[ApiController]
[Route("api/tasks")]
public class TaskScheduleApiController(AppDbContext database) : ControllerBase
{
    [HttpGet]
    public ActionResult<IReadOnlyCollection<TaskCalendarDto>> Get(DateTime start, DateTime end)
    {
        if (end <= start)
            return BadRequest(new { error = "El final del intervalo debe ser posterior al inicio." });

        var tasks = database.Tasks
            .AsNoTracking()
            .Include(item => item.Project)
            .Where(item => item.StartAt.HasValue &&
                item.StartAt.Value < end &&
                (item.EndAt ?? item.StartAt.Value) >= start)
            .OrderBy(item => item.StartAt)
            .AsEnumerable()
            .Select(ToCalendarDto)
            .ToArray();

        return Ok(tasks);
    }

    [HttpGet("list")]
    public ActionResult<IReadOnlyCollection<TaskListDto>> GetList()
    {
        var tasks = database.Tasks
            .AsNoTracking()
            .Include(item => item.Project)
            .OrderBy(item => item.Status)
            .ThenBy(item => item.StartAt)
            .ThenBy(item => item.Title)
            .AsEnumerable()
            .Select(ToListDto)
            .ToArray();

        return Ok(tasks);
    }

    [HttpGet("{id:int}")]
    public ActionResult<TaskCalendarDto> GetById(int id)
    {
        var item = database.Tasks.AsNoTracking().Include(task => task.Project).FirstOrDefault(task => task.Id == id);
        if (item?.StartAt is null)
            return NotFound();

        return Ok(ToCalendarDto(item));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult<TaskCalendarDto> Create(TaskScheduleRequest request)
    {
        ValidateProject(request.ProjectId);
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        var item = new TaskItem();
        Apply(request, item);
        database.Tasks.Add(item);
        database.SaveChanges();
        database.Entry(item).Reference(task => task.Project).Load();
        return CreatedAtAction(nameof(GetById), new { id = item.Id }, ToCalendarDto(item));
    }

    [HttpPut("{id:int}")]
    [ValidateAntiForgeryToken]
    public ActionResult<TaskCalendarDto> Update(int id, TaskScheduleRequest request)
    {
        var item = database.Tasks.Include(task => task.Project).FirstOrDefault(task => task.Id == id);
        if (item is null)
            return NotFound();

        ValidateProject(request.ProjectId);
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        Apply(request, item);
        database.SaveChanges();
        if (item.ProjectId.HasValue)
            database.Entry(item).Reference(task => task.Project).Load();
        else
            item.Project = null;
        return Ok(ToCalendarDto(item));
    }

    private void ValidateProject(int? projectId)
    {
        if (projectId.HasValue && !database.Projects.Any(project => project.Id == projectId))
            ModelState.AddModelError(nameof(TaskScheduleRequest.ProjectId), "El proyecto seleccionado no existe.");
    }

    private static void Apply(TaskScheduleRequest request, TaskItem item)
    {
        item.Title = request.Title.Trim();
        item.StartAt = request.Start;
        item.EndAt = request.End;
        item.DueDate = (request.End ?? request.Start)?.Date;
        item.Status = request.Status;
        item.Priority = request.Priority;
        item.ProjectId = request.ProjectId;
        item.AssignedTo = string.IsNullOrWhiteSpace(request.AssignedTo) ? null : request.AssignedTo.Trim();
    }

    private static TaskCalendarDto ToCalendarDto(TaskItem item)
    {
        return new TaskCalendarDto(
            item.Id,
            item.Title,
            item.StartAt!.Value,
            item.EndAt,
            item.Status,
            item.Priority,
            item.ProjectId,
            item.Project?.Name,
            item.AssignedTo);
    }

    private static TaskListDto ToListDto(TaskItem item)
    {
        return new TaskListDto(
            item.Id,
            item.Title,
            item.StartAt,
            item.EndAt,
            item.Status,
            item.Priority,
            item.ProjectId,
            item.Project?.Name,
            item.AssignedTo);
    }
}
