using Asreyion.Core.Features.Database.DbContexts;
using Asreyion.Core.Theme.Models;
using Asreyion.Modules.Blog.Features.Blog.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Asreyion.Content.StaticContent.Content.Controllers;

public class HomeController(DataDbContext dbContext) : Controller
{
    public IActionResult Index()
    {
        try
        {
            List<BlogPost> posts = [.. dbContext.BlogPosts
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

    public IActionResult Privacy()
        => this.View();

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
        => this.View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? this.HttpContext.TraceIdentifier });
}
