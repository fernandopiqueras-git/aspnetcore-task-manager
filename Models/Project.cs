using System.ComponentModel.DataAnnotations;

namespace AspNetCoreTaskManager.Models;

public class Project
{
    public int Id { get; set; }

    [Required]
    [StringLength(80)]
    public string Name { get; set; } = string.Empty;

    public ICollection<TaskItem> Tasks { get; set; } = [];
}
