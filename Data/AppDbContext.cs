using AspNetCoreTaskManager.Models;
using Microsoft.EntityFrameworkCore;

namespace AspNetCoreTaskManager.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<TaskItem> Tasks => Set<TaskItem>();
}
