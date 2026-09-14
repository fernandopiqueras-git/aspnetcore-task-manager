using AspNetCoreTaskManager.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AspNetCoreTaskManager.Migrations;

[DbContext(typeof(AppDbContext))]
[Migration("20260914214500_AddTaskOrganization")]
public class AddTaskOrganization : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(name: "AssignedTo", table: "Tasks", type: "nvarchar(100)", maxLength: 100, nullable: true);
        migrationBuilder.AddColumn<int>(name: "ProjectId", table: "Tasks", type: "int", nullable: true);

        migrationBuilder.CreateTable(
            name: "Projects",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false).Annotation("SqlServer:Identity", "1, 1"),
                Name = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false)
            },
            constraints: table => table.PrimaryKey("PK_Projects", item => item.Id));

        migrationBuilder.CreateTable(
            name: "Tags",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false).Annotation("SqlServer:Identity", "1, 1"),
                Name = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false)
            },
            constraints: table => table.PrimaryKey("PK_Tags", item => item.Id));

        migrationBuilder.CreateTable(
            name: "Comments",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false).Annotation("SqlServer:Identity", "1, 1"),
                TaskItemId = table.Column<int>(type: "int", nullable: false),
                Text = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Comments", item => item.Id);
                table.ForeignKey("FK_Comments_Tasks_TaskItemId", item => item.TaskItemId, "Tasks", "Id", onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "TaskTags",
            columns: table => new
            {
                TaskItemId = table.Column<int>(type: "int", nullable: false),
                TagId = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_TaskTags", item => new { item.TaskItemId, item.TagId });
                table.ForeignKey("FK_TaskTags_Tags_TagId", item => item.TagId, "Tags", "Id", onDelete: ReferentialAction.Cascade);
                table.ForeignKey("FK_TaskTags_Tasks_TaskItemId", item => item.TaskItemId, "Tasks", "Id", onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(name: "IX_Tasks_ProjectId", table: "Tasks", column: "ProjectId");
        migrationBuilder.CreateIndex(name: "IX_Projects_Name", table: "Projects", column: "Name", unique: true);
        migrationBuilder.CreateIndex(name: "IX_Tags_Name", table: "Tags", column: "Name", unique: true);
        migrationBuilder.CreateIndex(name: "IX_Comments_TaskItemId", table: "Comments", column: "TaskItemId");
        migrationBuilder.CreateIndex(name: "IX_TaskTags_TagId", table: "TaskTags", column: "TagId");
        migrationBuilder.AddForeignKey("FK_Tasks_Projects_ProjectId", "Tasks", "ProjectId", "Projects", principalColumn: "Id", onDelete: ReferentialAction.SetNull);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey("FK_Tasks_Projects_ProjectId", "Tasks");
        migrationBuilder.DropTable("Comments");
        migrationBuilder.DropTable("TaskTags");
        migrationBuilder.DropTable("Tags");
        migrationBuilder.DropTable("Projects");
        migrationBuilder.DropIndex("IX_Tasks_ProjectId", "Tasks");
        migrationBuilder.DropColumn("AssignedTo", "Tasks");
        migrationBuilder.DropColumn("ProjectId", "Tasks");
    }
}
