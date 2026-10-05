using Asreyion.Core.Features.Database.DbContexts;
using Asreyion.Core.Features.Navigation.Builder;
using Asreyion.Core.Features.Navigation.Hooks;
using Asreyion.Modules.Blog.Data;
using Microsoft.EntityFrameworkCore;

namespace Asreyion.Modules.Blog.Hooks;

/// <summary>
/// Registers blog navigation items dynamically into navigation menus.
/// </summary>
public class BlogNavigationHook(DataDbContext dbContext) : IOnBuildNavigationHook
{
    public async Task BuildNavigationAsync(INavigationBuilder builder)
    {
        if (!string.Equals(builder.MenuName, "Primary", StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        // Add or get "Blog" root parent item
        NavigationItemDefinition blogParent = builder.AddOrGet("Blog", item => item
            .WithRoute("Blog", "Index", "Blog")
            .WithIcon("newspaper")
            .WithOrder(20));

        // Add "All Posts" link under Blog
        _ = blogParent.AddOrGetChild("All Posts", child => child
            .WithRoute("Blog", "Index", "Blog")
            .WithIcon("list")
            .WithOrder(10));

        _ = blogParent.AddOrGetChild("", child => child
            .WithItemType("Divider")
            .WithOrder(20));

        _ = blogParent.AddOrGetChild("Categories", child => child
            .WithItemType("Header")
            .WithOrder(20));

        // Dynamically add all Blog Categories from the database as children under "Blog"!
        List<BlogCategory> categories = await dbContext.Set<BlogCategory>()
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .ToListAsync();

        foreach (BlogCategory category in categories)
        {
            _ = blogParent.AddOrGetChild(category.Name, child => child
                .WithRoute("Categories", "Index", "Blog")
                .WithRouteValue("slug", category.Slug)
                .WithIcon("folder")
                .WithOrder(30));
        }
    }
}

