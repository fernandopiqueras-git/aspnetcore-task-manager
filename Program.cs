using AspNetCoreTaskManager.Data;
using AspNetCoreTaskManager.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<ITaskRepository, EfTaskRepository>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var database = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    database.Database.Migrate();
}

app.UseStaticFiles();
app.UseRouting();
app.MapControllerRoute("default", "{controller=Tasks}/{action=Index}/{id?}");
app.Run();

public partial class Program;
