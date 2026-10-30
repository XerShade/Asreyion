using Asreyion.Core.Database.DbContexts;
using Asreyion.Modules.Blog.Data;
using Asreyion.Modules.Blog.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Asreyion.Modules.Blog.Controllers;

/// <summary>
/// Displays an individual published blog post.
/// </summary>
[Area("Blog")]
public class PostsController(
    DataDbContext dbContext,
    BlogPostUrlService postUrlService,
    ILogger<PostsController> logger) : Controller
{
    /// <summary>
    /// Finds and displays a post by its category tree and post slug.
    /// </summary>
    /// <param name="path">The category slugs followed by the post slug.</param>
    /// <param name="cancellationToken">Cancels the database query.</param>
    /// <returns>The post view, or a not-found response.</returns>
    [HttpGet("blog/posts/{**path}", Name = "BlogPostCanonical")]
    public async Task<IActionResult> Index(string path, CancellationToken cancellationToken)
    {
        string[] pathSegments = ParsePath(path);

        if (pathSegments.Length == 0)
        {
            return this.NotFound();
        }

        try
        {
            string postSlug = pathSegments[^1];
            string[] categoryPath = pathSegments[..^1];
            BlogPost? post = await GetPostAsync(postSlug, cancellationToken);

            if (post is null)
            {
                return this.NotFound();
            }

            IReadOnlyDictionary<int, string> categoryPaths = await postUrlService.GetCategoryPathsAsync(cancellationToken);

            if (categoryPath.Length == 0 && post.Categories.Count > 0)
            {
                string canonicalPath = postUrlService.BuildPostPath(post, categoryPaths);
                string canonicalUrl = this.Url.RouteUrl("BlogPostCanonical", new { path = canonicalPath })
                    ?? $"/blog/posts/{canonicalPath}";

                return this.RedirectPermanent(canonicalUrl);
            }

            if (!postUrlService.HasMatchingCategoryPath(post, categoryPath, categoryPaths))
            {
                return this.NotFound();
            }

            this.ViewData["CategoryPaths"] = categoryPaths;

            return this.View(post);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Failed to load blog post at path {PostPath}.", path);

            return this.StatusCode(500);
        }
    }

    /// <summary>
    /// Redirects the old post URL to the category-aware canonical URL.
    /// </summary>
    /// <param name="slug">The post's stored slug.</param>
    /// <param name="cancellationToken">Cancels the database query.</param>
    /// <returns>A permanent redirect to the canonical URL, or a not-found response.</returns>
    [HttpGet("blog/post/{slug}")]
    public async Task<IActionResult> Legacy(string slug, CancellationToken cancellationToken)
    {
        try
        {
            BlogPost? post = await GetPostAsync(slug, cancellationToken);

            if (post is null)
            {
                return this.NotFound();
            }

            IReadOnlyDictionary<int, string> categoryPaths = await postUrlService.GetCategoryPathsAsync(cancellationToken);
            string canonicalPath = postUrlService.BuildPostPath(post, categoryPaths);
            string canonicalUrl = this.Url.RouteUrl("BlogPostCanonical", new { path = canonicalPath })
                ?? $"/blog/posts/{canonicalPath}";

            return this.RedirectPermanent(canonicalUrl);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Failed to redirect legacy blog post URL for slug {PostSlug}.", slug);

            return this.StatusCode(500);
        }
    }

    private async Task<BlogPost?> GetPostAsync(string slug, CancellationToken cancellationToken)
    {
        return await dbContext.Set<BlogPost>()
            .AsNoTracking()
            .Include(post => post.Author)
            .Include(post => post.Categories)
            .Include(post => post.Tags)
            .FirstOrDefaultAsync(post => post.Slug == slug, cancellationToken);
    }

    private static string[] ParsePath(string? path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            return [];
        }

        return path.Split('/', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    }
}
