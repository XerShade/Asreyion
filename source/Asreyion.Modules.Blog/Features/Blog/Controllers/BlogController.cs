using Asreyion.Core.Features.Database.DbContexts;
using Asreyion.Modules.Blog.Features.Blog.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Asreyion.Modules.Blog.Features.Blog.Controllers;

[Area("Blog")]
public class BlogController(DataDbContext dbContext) : Controller
{
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        try
        {
            List<BlogPost> posts = await dbContext.BlogPosts
                .AsNoTracking()
                .Include(p => p.Author)
                .Include(p => p.Categories)
                .Include(p => p.Tags)
                .OrderByDescending(p => p.Created)
                .ToListAsync(cancellationToken);

            return this.View(posts);
        }
        catch
        {
            return this.RedirectToAction("Index");
        }
    }

    [HttpGet("Blog/Post/{slug}")]
    public async Task<IActionResult> Post(string slug, CancellationToken cancellationToken)
    {
        try
        {
            BlogPost? post = await dbContext.BlogPosts
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
            return this.RedirectToAction("Index");
        }
    }
}