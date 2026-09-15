using System.ComponentModel.DataAnnotations;

namespace AspNetCoreTaskManager.Models;

public class TaskItem
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El título es obligatorio.")]
    [StringLength(100, ErrorMessage = "El título no puede superar los 100 caracteres.")]
    public string Title { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "La descripción no puede superar los 500 caracteres.")]
    public string? Description { get; set; }

    public TaskPriority Priority { get; set; } = TaskPriority.Medium;
    public WorkStatus Status { get; set; } = WorkStatus.Pending;

    [DataType(DataType.Date)]
    [FutureOrToday]
    public DateTime? DueDate { get; set; }

    public DateTime? StartAt { get; set; }
    public DateTime? EndAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public int? ProjectId { get; set; }
    public Project? Project { get; set; }

    [StringLength(100)]
    public string? AssignedTo { get; set; }

    public ICollection<TaskTag> TaskTags { get; set; } = [];
    public ICollection<TaskComment> Comments { get; set; } = [];
}
