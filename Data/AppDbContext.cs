using AspNetCoreTaskManager.Models;
using Microsoft.EntityFrameworkCore;

namespace AspNetCoreTaskManager.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<TaskItem> Tasks => Set<TaskItem>();
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<Tag> Tags => Set<Tag>();
    public DbSet<TaskTag> TaskTags => Set<TaskTag>();
    public DbSet<TaskComment> Comments => Set<TaskComment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TaskTag>().HasKey(item => new { item.TaskItemId, item.TagId });
        modelBuilder.Entity<Project>().HasIndex(item => item.Name).IsUnique();
        modelBuilder.Entity<Tag>().HasIndex(item => item.Name).IsUnique();
        modelBuilder.Entity<TaskItem>()
            .HasOne(item => item.Project)
            .WithMany(project => project.Tasks)
            .HasForeignKey(item => item.ProjectId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
