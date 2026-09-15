using AspNetCoreTaskManager.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AspNetCoreTaskManager.Migrations;

[DbContext(typeof(AppDbContext))]
[Migration("20260915103000_AddTaskSchedule")]
public class AddTaskSchedule : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<DateTime>(name: "StartAt", table: "Tasks", type: "datetime2", nullable: true);
        migrationBuilder.AddColumn<DateTime>(name: "EndAt", table: "Tasks", type: "datetime2", nullable: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(name: "StartAt", table: "Tasks");
        migrationBuilder.DropColumn(name: "EndAt", table: "Tasks");
    }
}
