using Asreyion.Core.Features.Database.DbContexts;
using Asreyion.Modules.Blog.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Asreyion.Modules.Blog.Controllers;

[Area("Blog")]
public class BlogController(DataDbContext dbContext) : Controller
{
    [HttpGet("Blog")]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        try
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
        catch
        {
            return this.StatusCode(500);
        }
    }
}