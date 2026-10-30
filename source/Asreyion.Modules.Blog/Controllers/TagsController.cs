using Asreyion.Core.Database.DbContexts;
using Asreyion.Modules.Blog.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Asreyion.Modules.Blog.Controllers;

/// <summary>
/// Displays posts associated with a blog tag.
/// </summary>
[Area("Blog")]
public class TagsController(DataDbContext dbContext) : Controller
{
    /// <summary>
    /// Finds a tag by name and displays its posts.
    /// </summary>
    /// <param name="slug">The tag name from the route.</param>
    /// <param name="cancellationToken">Cancels the database query.</param>
    /// <returns>The tagged post list, or a not-found response.</returns>
    [HttpGet("blog/tag/{slug}")]
    public async Task<IActionResult> Index(string slug, CancellationToken cancellationToken)
    {
        try
        {
            BlogTag? tag = await dbContext.Set<BlogTag>()
            .AsNoTracking()
            .Include(t => t.Posts)
            .FirstOrDefaultAsync(
                t => t.Name == slug,
                cancellationToken);

            if (tag is null)
            {
                return this.NotFound();
            }

            List<BlogPost> posts = [.. tag.Posts.OrderByDescending(p => p.Created)];

            this.ViewBag.Tag = tag;

            return this.View(posts);
        }
        catch
        {
            return this.StatusCode(500);
        }
    }
}
