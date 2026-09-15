using AspNetCoreTaskManager.Models;

namespace AspNetCoreTaskManager.Dtos;

public record TaskListDto(
    int Id,
    string Title,
    DateTime? Start,
    DateTime? End,
    WorkStatus Status,
    TaskPriority Priority,
    int? ProjectId,
    string? ProjectName,
    string? AssignedTo);
