using Asreyion.Core.Data;
using Asreyion.Core.Database.DbContexts;
using Asreyion.Modules.SimpleContent.Services;
using Asreyion.Modules.SimpleContent.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

WebApplicationBuilder builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    // Set the content root path to the directory of the executing assembly.
    ContentRootPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!,

    // Set the web root path to the directory of the executing assembly.
    WebRootPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!, "wwwroot"),

    // Set the command line arguments.
    Args = args,

    // Set the environment name.
    EnvironmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production",

    // Set the application name.
    ApplicationName = Assembly.GetExecutingAssembly().GetName().Name ?? "Asreyion"
});

// Add services to the container.
builder.Services.AddDbContext<AuthenticationDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("Authentication"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("Authentication"))
    ));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;
})
    .AddEntityFrameworkStores<AuthenticationDbContext>()
    .AddDefaultTokenProviders();

builder.Services
    .AddAuthentication()
    .AddGoogle(options =>
    {
        options.ClientId =
            builder.Configuration["Authentication:Google:ClientId"]!;

        options.ClientSecret =
            builder.Configuration["Authentication:Google:ClientSecret"]!;
    });

builder.Services.AddControllersWithViews();

builder.Services.AddTransient<IContentProvider, MarkdownContentProvider>();

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    _ = app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    _ = app.UseHsts();
}

bool enableHttps = builder.Configuration.GetValue<bool>("ENABLE_HTTPS");

if (enableHttps)
{
    _ = app.UseHttpsRedirection();
}

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllers();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();