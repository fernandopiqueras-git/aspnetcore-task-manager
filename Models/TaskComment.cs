using System.ComponentModel.DataAnnotations;

namespace AspNetCoreTaskManager.Models;

public class TaskComment
{
    public int Id { get; set; }
    public int TaskItemId { get; set; }
    public TaskItem TaskItem { get; set; } = null!;

    [Required]
    [StringLength(500)]
    public string Text { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
