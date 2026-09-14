using System.ComponentModel.DataAnnotations;

namespace AspNetCoreTaskManager.Models;

public class Tag
{
    public int Id { get; set; }

    [Required]
    [StringLength(40)]
    public string Name { get; set; } = string.Empty;

    public ICollection<TaskTag> TaskTags { get; set; } = [];
}
