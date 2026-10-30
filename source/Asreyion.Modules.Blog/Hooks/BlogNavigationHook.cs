using Asreyion.Core.Database.DbContexts;
using Asreyion.Core.Hooks.Attributes;
using Asreyion.Core.Navigation.Builder;
using Asreyion.Core.Navigation.Hooks;
using Asreyion.Modules.Blog.Data;
using Microsoft.EntityFrameworkCore;

namespace Asreyion.Modules.Blog.Hooks;

/// <summary>
/// Registers blog posts and database-backed category links in the primary menu.
/// </summary>
[HookName("Blog Navigation")]
[HookDescription("Adds blog links and nested categories to the primary navigation menu.")]
public sealed class BlogNavigationHook(DataDbContext dbContext) : IOnBuildNavigationHook
{
    /// <inheritdoc />
    public async Task BuildNavigationAsync(INavigationBuilder builder)
    {
        if (!string.Equals(builder.MenuName, "Primary", StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        NavigationItemDefinition blogMenu = builder.AddOrGet("Blog", item => item
            .WithRoute("Blog", "Index", "Blog")
            .WithIcon("newspaper")
            .WithOrder(20));

        _ = blogMenu.AddOrGetChild("All Posts", item => item
            .WithRoute("Blog", "Index", "Blog")
            .WithIcon("list")
            .WithOrder(10));

        _ = blogMenu.AddOrGetChild(string.Empty, item => item
            .WithItemType("Divider")
            .WithOrder(20));

        _ = blogMenu.AddOrGetChild("Categories", item => item
            .WithItemType("Header")
            .WithOrder(30));

        List<BlogCategory> categories = await dbContext.Set<BlogCategory>()
            .AsNoTracking()
            .OrderBy(category => category.Name)
            .ToListAsync();

        Dictionary<int, List<BlogCategory>> categoriesByParent = categories
            .Where(category => category.ParentId.HasValue)
            .GroupBy(category => category.ParentId!.Value)
            .ToDictionary(group => group.Key, group => group.ToList());

        List<BlogCategory> rootCategories = categories
            .Where(category => !category.ParentId.HasValue)
            .ToList();

        AddCategoryItems(blogMenu, rootCategories, categoriesByParent, string.Empty, 40);
    }

    private static void AddCategoryItems(
        NavigationItemDefinition parentMenuItem,
        IEnumerable<BlogCategory> categories,
        IReadOnlyDictionary<int, List<BlogCategory>> categoriesByParent,
        string parentSlugPath,
        int startingOrder)
    {
        int order = startingOrder;

        foreach (BlogCategory category in categories)
        {
            if (string.IsNullOrWhiteSpace(category.Slug))
            {
                continue;
            }

            string categorySlugPath = string.IsNullOrEmpty(parentSlugPath)
                ? category.Slug
                : $"{parentSlugPath}/{category.Slug}";

            NavigationItemDefinition categoryMenuItem = parentMenuItem.AddOrGetChild(category.Name, item => item
                .WithRoute("Categories", "Index", "Blog")
                .WithRouteValue("slug", categorySlugPath)
                .WithIcon("folder")
                .WithOrder(order));

            if (categoriesByParent.TryGetValue(category.Id, out List<BlogCategory>? childCategories))
            {
                AddCategoryItems(categoryMenuItem, childCategories, categoriesByParent, categorySlugPath, 10);
            }

            order += 10;
        }
    }
}
