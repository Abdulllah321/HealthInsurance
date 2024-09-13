using HealthInsurance.Entities;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Configure DbContext with SQL Server
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

// Add services to the container
builder.Services.AddControllersWithViews();

// Configure authentication with cookie
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Error/AccessDenied"; // Path for access denied errors
    });

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    // Use exception handler for production
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Custom middleware to handle unauthorized access to /admin
app.Use(async (context, next) =>
{
    if (context.Request.Path.StartsWithSegments("/admin") &&
        !context.User.Identity.IsAuthenticated)
    {
        // Set status code to 404 for unauthorized access to /admin
        context.Response.StatusCode = StatusCodes.Status404NotFound;
        await context.Response.WriteAsync("404 Not Found");
        return;
    }
    await next();
});

app.Use(async (context, next) =>
{
    await next();

    if (context.Response.StatusCode == StatusCodes.Status404NotFound)
    {
        context.Request.Path = "/Error/NotFound"; // Path to the NotFound action or view
        await next(); 
    }
});

// Configure routes
app.MapControllerRoute(
    name: "adminRegister",
    pattern: "admin/register",
    defaults: new { controller = "Auth", action = "AdminRegistration" }
);

app.MapControllerRoute(
    name: "adminLogin",
    pattern: "admin/login",
    defaults: new { controller = "Auth", action = "AdminLogin" }
);

app.MapControllerRoute(
    name: "empRegister",
    pattern: "employee/register",
    defaults: new { controller = "Auth", action = "EmpRegistration" }
);

app.MapControllerRoute(
    name: "empLogin",
    pattern: "employee/login",
    defaults: new { controller = "Auth", action = "EmpLogin" }
);

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
