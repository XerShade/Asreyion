using Asreyion.Core.Database.DbContexts;
using Asreyion.Modules.Blog.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Asreyion.Modules.Blog.Controllers;

/// <summary>
/// Displays the published blog index.
/// </summary>
[Area("Blog")]
public class BlogController(DataDbContext dbContext) : Controller
{
    /// <summary>
    /// Lists posts with their authors, categories, and tags.
    /// </summary>
    /// <param name="cancellationToken">Cancels the database query.</param>
    /// <returns>The blog index view.</returns>
    [HttpGet("Blog")]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        List<BlogPost> posts = await dbContext.Set<BlogPost>()
            .AsNoTracking()
            .Include(p => p.Author)
            .Include(p => p.Categories)
            .Include(p => p.Tags)
            .OrderByDescending(p => p.Created)
            .ToListAsync(cancellationToken);

        return this.View(posts);
    }
}
