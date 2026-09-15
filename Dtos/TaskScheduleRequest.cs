using AspNetCoreTaskManager.Models;
using System.ComponentModel.DataAnnotations;

namespace AspNetCoreTaskManager.Dtos;

public class TaskScheduleRequest : IValidatableObject
{
    [Required(ErrorMessage = "El título es obligatorio.")]
    [StringLength(100)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public DateTime? Start { get; set; }

    public DateTime? End { get; set; }

    [EnumDataType(typeof(WorkStatus))]
    public WorkStatus Status { get; set; }

    [EnumDataType(typeof(TaskPriority))]
    public TaskPriority Priority { get; set; } = TaskPriority.Medium;

    public int? ProjectId { get; set; }

    [StringLength(100)]
    public string? AssignedTo { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Start.HasValue && End.HasValue && End <= Start)
            yield return new ValidationResult("La fecha final debe ser posterior a la inicial.", [nameof(End)]);
    }
}
