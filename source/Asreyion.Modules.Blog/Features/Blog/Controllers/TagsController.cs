using Asreyion.Core.Features.Database.DbContexts;
using Asreyion.Modules.Blog.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Asreyion.Modules.Blog.Controllers;

[Area("Blog")]
public class TagsController(DataDbContext dbContext) : Controller
{
    [HttpGet("Blog/Tags/{slug}")]
    public async Task<IActionResult> Index(string slug, CancellationToken cancellationToken)
    {
        try
        {
            BlogTag? tag = await dbContext.BlogTags
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
            return this.RedirectToAction("Index");
        }
    }
}