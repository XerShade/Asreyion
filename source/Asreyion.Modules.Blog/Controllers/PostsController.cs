using Asreyion.Core.Database.DbContexts;
using Asreyion.Modules.Blog.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Asreyion.Modules.Blog.Controllers;

/// <summary>
/// Displays an individual published blog post.
/// </summary>
[Area("Blog")]
public class PostsController(DataDbContext dbContext) : Controller
{
    /// <summary>
    /// Finds a post by its URL slug and displays it.
    /// </summary>
    /// <param name="slug">The post's URL slug.</param>
    /// <param name="cancellationToken">Cancels the database query.</param>
    /// <returns>The post view, or a not-found response.</returns>
    [HttpGet("Blog/Post/{slug}")]
    public async Task<IActionResult> Index(string slug, CancellationToken cancellationToken)
    {
        try
        {
            BlogPost? post = await dbContext.Set<BlogPost>()
            .AsNoTracking()
            .Include(p => p.Author)
            .Include(p => p.Categories)
            .Include(p => p.Tags)
            .FirstOrDefaultAsync(
                p => p.Slug == slug,
                cancellationToken);

            return post is null ? this.NotFound() : this.View(post);
        }
        catch
        {
            return this.StatusCode(500);
        }
    }
}
