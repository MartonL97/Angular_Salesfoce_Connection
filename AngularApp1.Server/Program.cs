using AngularApp1.Server.Data;
using AngularApp1.Server.Interfaces;
using AngularApp1.Server.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews(); // ? Use MVC with views

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddSwaggerGen();

// Register custom Salesforce service
builder.Services.AddScoped<ISalesforceService, SalesforceService>();

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllerRoute( // ? Map MVC routes
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Remove this if you're not using Angular routing fallback:
app.MapFallbackToFile("/index.html"); // ? Only for SPA fallback. Comment/remove if using pure MVC.

app.Run();
