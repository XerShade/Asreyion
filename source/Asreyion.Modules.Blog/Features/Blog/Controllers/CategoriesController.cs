using Asreyion.Core.Features.Database.DbContexts;
using Asreyion.Modules.Blog.Features.Blog.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Asreyion.Modules.Blog.Features.Blog.Controllers;

[Area("Blog")]
public class CategoriesController(DataDbContext dbContext) : Controller
{
    [HttpGet("Blog/Categories/{slug}")]
    public async Task<IActionResult> Index(string slug, CancellationToken cancellationToken)
    {
        try
        {
            BlogCategory? category = await dbContext.BlogCategories
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
            return this.RedirectToAction("Index");
        }
    }
}