using Asreyion.Core.Database.DbContexts;
using Asreyion.Modules.Blog.Data;
using Microsoft.EntityFrameworkCore;

namespace Asreyion.Modules.Blog.Services;

/// <summary>
/// Builds and validates canonical blog URLs using category hierarchy paths.
/// </summary>
public sealed class BlogPostUrlService(DataDbContext dbContext)
{
    public async Task<IReadOnlyDictionary<int, string>> GetCategoryPathsAsync(CancellationToken cancellationToken)
    {
        List<CategoryRouteNode> categories = await dbContext.Set<BlogCategory>()
            .AsNoTracking()
            .Select(category => new CategoryRouteNode(category.Id, category.Slug, category.ParentId))
            .ToListAsync(cancellationToken);

        Dictionary<int, CategoryRouteNode> categoriesById = categories.ToDictionary(category => category.Id);
        Dictionary<int, string> categoryPaths = [];

        foreach (CategoryRouteNode category in categories)
        {
            string categoryPath = BuildCategoryPath(category, categoriesById, []);

            if (!string.IsNullOrWhiteSpace(categoryPath))
            {
                categoryPaths[category.Id] = categoryPath;
            }
        }

        return categoryPaths;
    }

    public string BuildPostPath(BlogPost post, IReadOnlyDictionary<int, string> categoryPaths)
    {
        string? primaryCategoryPath = post.Categories
            .Where(category => categoryPaths.ContainsKey(category.Id))
            .Select(category => categoryPaths[category.Id])
            .OrderBy(path => path, StringComparer.OrdinalIgnoreCase)
            .FirstOrDefault();

        if (string.IsNullOrWhiteSpace(primaryCategoryPath))
        {
            return post.Slug;
        }

        return $"{primaryCategoryPath}/{post.Slug}";
    }

    public IReadOnlyDictionary<int, string> BuildPostPaths(
        IEnumerable<BlogPost> posts,
        IReadOnlyDictionary<int, string> categoryPaths)
    {
        return posts.ToDictionary(post => post.Id, post => this.BuildPostPath(post, categoryPaths));
    }

    public bool HasMatchingCategoryPath(
        BlogPost post,
        string[] requestedCategoryPath,
        IReadOnlyDictionary<int, string> categoryPaths)
    {
        if (requestedCategoryPath.Length == 0)
        {
            return post.Categories.Count == 0;
        }

        string requestedPath = string.Join('/', requestedCategoryPath);

        return post.Categories.Any(category =>
            categoryPaths.TryGetValue(category.Id, out string? categoryPath)
            && string.Equals(categoryPath, requestedPath, StringComparison.OrdinalIgnoreCase));
    }

    private static string BuildCategoryPath(
        CategoryRouteNode category,
        IReadOnlyDictionary<int, CategoryRouteNode> categoriesById,
        HashSet<int> visitedCategoryIds)
    {
        if (!visitedCategoryIds.Add(category.Id) || string.IsNullOrWhiteSpace(category.Slug))
        {
            return string.Empty;
        }

        if (!category.ParentId.HasValue)
        {
            return category.Slug;
        }

        if (!categoriesById.TryGetValue(category.ParentId.Value, out CategoryRouteNode? parentCategory))
        {
            return string.Empty;
        }

        string parentPath = BuildCategoryPath(parentCategory, categoriesById, visitedCategoryIds);

        if (string.IsNullOrWhiteSpace(parentPath))
        {
            return string.Empty;
        }

        return $"{parentPath}/{category.Slug}";
    }

    private sealed record CategoryRouteNode(int Id, string Slug, int? ParentId);
}
