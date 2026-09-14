using AspNetCoreTaskManager.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;

namespace AspNetCoreTaskManager.Migrations;

[DbContext(typeof(AppDbContext))]
public class AppDbContextModelSnapshot : ModelSnapshot
{
    protected override void BuildModel(ModelBuilder modelBuilder)
    {
        modelBuilder.HasAnnotation("ProductVersion", "8.0.11").HasAnnotation("Relational:MaxIdentifierLength", 128);
        SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

        modelBuilder.Entity("AspNetCoreTaskManager.Models.Project", entity =>
        {
            entity.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("int").UseIdentityColumn();
            entity.Property<string>("Name").IsRequired().HasMaxLength(80).HasColumnType("nvarchar(80)");
            entity.HasKey("Id");
            entity.HasIndex("Name").IsUnique();
            entity.ToTable("Projects");
        });

        modelBuilder.Entity("AspNetCoreTaskManager.Models.Tag", entity =>
        {
            entity.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("int").UseIdentityColumn();
            entity.Property<string>("Name").IsRequired().HasMaxLength(40).HasColumnType("nvarchar(40)");
            entity.HasKey("Id");
            entity.HasIndex("Name").IsUnique();
            entity.ToTable("Tags");
        });

        modelBuilder.Entity("AspNetCoreTaskManager.Models.TaskComment", entity =>
        {
            entity.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("int").UseIdentityColumn();
            entity.Property<DateTime>("CreatedAt").HasColumnType("datetime2");
            entity.Property<int>("TaskItemId").HasColumnType("int");
            entity.Property<string>("Text").IsRequired().HasMaxLength(500).HasColumnType("nvarchar(500)");
            entity.HasKey("Id");
            entity.HasIndex("TaskItemId");
            entity.ToTable("Comments");
        });

        modelBuilder.Entity("AspNetCoreTaskManager.Models.TaskItem", entity =>
        {
            entity.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("int").UseIdentityColumn();
            entity.Property<string>("AssignedTo").HasMaxLength(100).HasColumnType("nvarchar(100)");
            entity.Property<DateTime>("CreatedAt").HasColumnType("datetime2");
            entity.Property<string>("Description").HasMaxLength(500).HasColumnType("nvarchar(500)");
            entity.Property<DateTime?>("DueDate").HasColumnType("datetime2");
            entity.Property<int>("Priority").HasColumnType("int");
            entity.Property<int?>("ProjectId").HasColumnType("int");
            entity.Property<int>("Status").HasColumnType("int");
            entity.Property<string>("Title").IsRequired().HasMaxLength(100).HasColumnType("nvarchar(100)");
            entity.HasKey("Id");
            entity.HasIndex("ProjectId");
            entity.ToTable("Tasks");
        });

        modelBuilder.Entity("AspNetCoreTaskManager.Models.TaskTag", entity =>
        {
            entity.Property<int>("TaskItemId").HasColumnType("int");
            entity.Property<int>("TagId").HasColumnType("int");
            entity.HasKey("TaskItemId", "TagId");
            entity.HasIndex("TagId");
            entity.ToTable("TaskTags");
        });

        modelBuilder.Entity("AspNetCoreTaskManager.Models.TaskComment", entity =>
            entity.HasOne("AspNetCoreTaskManager.Models.TaskItem", "TaskItem").WithMany("Comments").HasForeignKey("TaskItemId").OnDelete(DeleteBehavior.Cascade).IsRequired());
        modelBuilder.Entity("AspNetCoreTaskManager.Models.TaskItem", entity =>
            entity.HasOne("AspNetCoreTaskManager.Models.Project", "Project").WithMany("Tasks").HasForeignKey("ProjectId").OnDelete(DeleteBehavior.SetNull));
        modelBuilder.Entity("AspNetCoreTaskManager.Models.TaskTag", entity =>
        {
            entity.HasOne("AspNetCoreTaskManager.Models.Tag", "Tag").WithMany("TaskTags").HasForeignKey("TagId").OnDelete(DeleteBehavior.Cascade).IsRequired();
            entity.HasOne("AspNetCoreTaskManager.Models.TaskItem", "TaskItem").WithMany("TaskTags").HasForeignKey("TaskItemId").OnDelete(DeleteBehavior.Cascade).IsRequired();
        });
    }
}
