using System.Configuration;
using CinemeOnlineWeb;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<DBOnlineCinemaContext>(option => option.UseSqlServer(
    builder.Configuration.GetConnectionString("OnlineCinemaDB")));

var app = builder.Build();


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

app.UseRouting();

app.UseAuthorization();

app.Run();