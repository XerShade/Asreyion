using Asreyion.Core.Features.Database.DbContexts;
using Asreyion.Modules.Blog.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Asreyion.Modules.Blog.Controllers;

[Area("Blog")]
public class PostsController(DataDbContext dbContext) : Controller
{
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