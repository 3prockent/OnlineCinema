using CinemeOnlineWeb.Models;
using CinemeOnlineWeb;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<DBOnlineCinemaContext>(option => option.UseSqlServer(
    builder.Configuration.GetConnectionString("OnlineCinemaDB")));

builder.Services.AddDbContext<IdentityContext>(option => option.UseSqlServer(
    builder.Configuration.GetConnectionString("IdentityDB")));

builder.Services.AddIdentity<User, IdentityRole>(opt =>
{
    opt.Password.RequireNonAlphanumeric = false;
    opt.Password.RequireUppercase = false;
}).AddEntityFrameworkStores<IdentityContext>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var userManager = services.GetRequiredService<UserManager<User>>();
        var rolesManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        await RoleInitializer.InitializeAsync(userManager, rolesManager);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database." + DateTime.Now.ToString());
    }
}


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Films}/{action=Index}/{filmId?}");


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseAuthentication();    

app.UseRouting();

app.UseAuthorization();

app.Run();