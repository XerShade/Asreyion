using Asreyion.Core.Database.DbContexts;
using Asreyion.Modules.Blog.Data;
using Asreyion.Modules.Blog.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Asreyion.Modules.Blog.Controllers;

/// <summary>
/// Displays posts assigned to a blog category.
/// </summary>
[Area("Blog")]
public class CategoriesController(
    DataDbContext dbContext,
    BlogPostUrlService postUrlService,
    ILogger<CategoriesController> logger) : Controller
{
    /// <summary>
    /// Finds a category by its hierarchical URL path and displays its posts.
    /// </summary>
    /// <param name="slug">The category slugs, including parent category slugs.</param>
    /// <param name="cancellationToken">Cancels the database query.</param>
    /// <returns>The category's post list, or a not-found response.</returns>
    [HttpGet("blog/category/{**slug}")]
    public async Task<IActionResult> Index(string slug, CancellationToken cancellationToken)
    {
        string[] requestedPath = ParseCategoryPath(slug);

        if (requestedPath.Length == 0)
        {
            return this.NotFound();
        }

        try
        {
            int? categoryId = await FindCategoryIdByPathAsync(requestedPath, cancellationToken);

            if (!categoryId.HasValue)
            {
                return this.NotFound();
            }

            BlogCategory? category = await dbContext.Set<BlogCategory>()
                .AsNoTracking()
                .AsSplitQuery()
                .Include(item => item.Posts)
                    .ThenInclude(post => post.Author)
                .Include(item => item.Posts)
                    .ThenInclude(post => post.Tags)
                .FirstOrDefaultAsync(item => item.Id == categoryId.Value, cancellationToken);

            if (category is null)
            {
                return this.NotFound();
            }

            List<BlogPost> posts = [.. category.Posts.OrderByDescending(post => post.Created)];
            await PopulatePostCategoriesAsync(posts, cancellationToken);

            IReadOnlyDictionary<int, string> categoryPaths = await postUrlService.GetCategoryPathsAsync(cancellationToken);

            this.ViewBag.Category = category;
            this.ViewData["CategoryPaths"] = categoryPaths;
            this.ViewData["PostUrlPaths"] = postUrlService.BuildPostPaths(posts, categoryPaths);

            return this.View(posts);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Failed to load blog category at path {CategoryPath}.", slug);

            return this.StatusCode(500);
        }
    }

    private async Task PopulatePostCategoriesAsync(
        IReadOnlyCollection<BlogPost> posts,
        CancellationToken cancellationToken)
    {
        if (posts.Count == 0)
        {
            return;
        }

        int[] postIds = posts.Select(post => post.Id).ToArray();
        List<BlogPost> postsWithCategories = await dbContext.Set<BlogPost>()
            .AsNoTracking()
            .Where(post => postIds.Contains(post.Id))
            .Include(post => post.Categories)
            .ToListAsync(cancellationToken);

        Dictionary<int, List<BlogCategory>> categoriesByPostId = postsWithCategories
            .ToDictionary(post => post.Id, post => post.Categories);

        foreach (BlogPost post in posts)
        {
            post.Categories = categoriesByPostId.TryGetValue(post.Id, out List<BlogCategory>? categories)
                ? categories
                : [];
        }
    }

    private static string[] ParseCategoryPath(string? slug)
    {
        if (string.IsNullOrWhiteSpace(slug))
        {
            return [];
        }

        return slug.Split('/', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    }

    private async Task<int?> FindCategoryIdByPathAsync(string[] requestedPath, CancellationToken cancellationToken)
    {
        List<CategoryRouteNode> categories = await dbContext.Set<BlogCategory>()
            .AsNoTracking()
            .Select(category => new CategoryRouteNode(category.Id, category.Slug, category.ParentId))
            .ToListAsync(cancellationToken);

        Dictionary<int, CategoryRouteNode> categoriesById = categories.ToDictionary(category => category.Id);
        CategoryRouteNode? targetCategory = categories.FirstOrDefault(category =>
            string.Equals(category.Slug, requestedPath[^1], StringComparison.OrdinalIgnoreCase));

        if (targetCategory is null)
        {
            return null;
        }

        List<string> categoryPath = BuildCategoryPath(targetCategory, categoriesById);
        bool matchesFullPath = requestedPath.SequenceEqual(categoryPath, StringComparer.OrdinalIgnoreCase);
        bool matchesLegacySlugOnly = requestedPath.Length == 1
            && string.Equals(requestedPath[0], targetCategory.Slug, StringComparison.OrdinalIgnoreCase);

        if (!matchesFullPath && !matchesLegacySlugOnly)
        {
            return null;
        }

        return targetCategory.Id;
    }

    private static List<string> BuildCategoryPath(
        CategoryRouteNode category,
        IReadOnlyDictionary<int, CategoryRouteNode> categoriesById)
    {
        List<string> path = [];
        HashSet<int> visitedCategoryIds = [];
        CategoryRouteNode? currentCategory = category;

        while (currentCategory is not null)
        {
            if (!visitedCategoryIds.Add(currentCategory.Id))
            {
                return [];
            }

            path.Add(currentCategory.Slug);

            if (!currentCategory.ParentId.HasValue)
            {
                break;
            }

            if (!categoriesById.TryGetValue(currentCategory.ParentId.Value, out CategoryRouteNode? parentCategory))
            {
                return [];
            }

            currentCategory = parentCategory;
        }

        path.Reverse();

        return path;
    }

    private sealed record CategoryRouteNode(int Id, string Slug, int? ParentId);
}
