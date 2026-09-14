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
        modelBuilder
            .HasAnnotation("ProductVersion", "8.0.11")
            .HasAnnotation("Relational:MaxIdentifierLength", 128);

        SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

        modelBuilder.Entity("AspNetCoreTaskManager.Models.TaskItem", entity =>
        {
            entity.Property<int>("Id")
                .ValueGeneratedOnAdd()
                .HasColumnType("int")
                .UseIdentityColumn();
            entity.Property<DateTime>("CreatedAt").HasColumnType("datetime2");
            entity.Property<string>("Description").HasMaxLength(500).HasColumnType("nvarchar(500)");
            entity.Property<DateTime?>("DueDate").HasColumnType("datetime2");
            entity.Property<int>("Priority").HasColumnType("int");
            entity.Property<int>("Status").HasColumnType("int");
            entity.Property<string>("Title").IsRequired().HasMaxLength(100).HasColumnType("nvarchar(100)");
            entity.HasKey("Id");
            entity.ToTable("Tasks");
        });
    }
}
