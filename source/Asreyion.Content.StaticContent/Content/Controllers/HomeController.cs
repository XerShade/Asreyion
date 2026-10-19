using Asreyion.Core.Database.DbContexts;
using Asreyion.Server.Models;
using Asreyion.Modules.Blog.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Asreyion.Content.StaticContent.Content.Controllers;

/// <summary>
/// Handles the public home, privacy, and error pages.
/// </summary>
public class HomeController(DataDbContext dbContext) : Controller
{
    /// <summary>
    /// Displays the most recent blog posts on the home page.
    /// </summary>
    /// <returns>The home page view.</returns>
    public IActionResult Index()
    {
        try
        {
            List<BlogPost> posts = [.. dbContext.Set<BlogPost>()
                .AsNoTracking()
                .Include(p => p.Author)
                .Include(p => p.Categories)
                .Include(p => p.Tags)
                .OrderByDescending(p => p.Created)
                .Take(5)];

            return this.View(posts);
        }
        catch
        {
            return this.RedirectToAction("Index");
        }
    }

    /// <summary>
    /// Displays the site's privacy information.
    /// </summary>
    /// <returns>The privacy view.</returns>
    public IActionResult Privacy()
        => this.View();

    /// <summary>
    /// Displays the error page with the current request identifier.
    /// </summary>
    /// <returns>The error view.</returns>
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
        => this.View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? this.HttpContext.TraceIdentifier });
}
