using Asreyion.Core.Database.DbContexts;
using Asreyion.Modules.Blog.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Asreyion.Modules.Blog.Controllers;

/// <summary>
/// Displays posts assigned to a blog category.
/// </summary>
[Area("Blog")]
public class CategoriesController(DataDbContext dbContext) : Controller
{
    /// <summary>
    /// Finds a category by its URL slug and displays its posts.
    /// </summary>
    /// <param name="slug">The category's URL slug.</param>
    /// <param name="cancellationToken">Cancels the database query.</param>
    /// <returns>The category's post list, or a not-found response.</returns>
    [HttpGet("Blog/Category/{slug}")]
    public async Task<IActionResult> Index(string slug, CancellationToken cancellationToken)
    {
        try
        {
            BlogCategory? category = await dbContext.Set<BlogCategory>()
            .AsNoTracking()
            .Include(c => c.Posts)
                .ThenInclude(p => p.Author)
            .Include(c => c.Posts)
                .ThenInclude(p => p.Tags)
            .FirstOrDefaultAsync(
                c => c.Slug == slug,
                cancellationToken);

            if (category is null)
            {
                return this.NotFound();
            }

            List<BlogPost> posts = [.. category.Posts.OrderByDescending(p => p.Created)];

            this.ViewBag.Category = category;

            return this.View(posts);
        }
        catch
        {
            return this.StatusCode(500);
        }
    }
}
