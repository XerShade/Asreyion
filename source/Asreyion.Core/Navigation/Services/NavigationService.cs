using Asreyion.Core.Database.DbContexts;
using Asreyion.Core.Navigation.Data;
using Asreyion.Core.Navigation.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Asreyion.Core.Navigation.Services;

/// <summary>
/// Persists navigation menus and provides their items for rendering and administration.
/// </summary>
public class NavigationService(DataDbContext dbContext) : INavigationService
{
    /// <inheritdoc />
    public async Task<IReadOnlyList<NavigationMenu>> GetNavigationMenusAsync()
        => await dbContext.Set<NavigationMenu>().AsNoTracking().ToListAsync();

    /// <inheritdoc />
    public async Task<NavigationMenu?> GetNavigationMenuAsync(int id)
        => await dbContext.Set<NavigationMenu>().FindAsync(id);

    /// <inheritdoc />
    public async Task<NavigationMenu?> GetNavigationMenuByNameAsync(string name)
        => await dbContext.Set<NavigationMenu>().AsNoTracking().FirstOrDefaultAsync(m => m.Name == name);

    /// <inheritdoc />
    public async Task<bool> AddRootItemAsync(int menuId, NavigationMenuItem item)
    {
        NavigationMenu? menu = await dbContext.Set<NavigationMenu>().FindAsync(menuId);
        if (menu is null)
        {
            return false;
        }

        // Top level items have no parent item
        item.ParentId = null;

        _ = await dbContext.Set<NavigationMenuItem>().AddAsync(item);
        _ = await dbContext.SaveChangesAsync();

        menu.Items ??= [];
        menu.Items.Add(item.Id);

        _ = dbContext.Set<NavigationMenu>().Update(menu);
        _ = await dbContext.SaveChangesAsync();

        return true;
    }

    /// <inheritdoc />
    public async Task<bool> AddRootItemsAsync(int menuId, IEnumerable<NavigationMenuItem> items)
    {
        NavigationMenu? menu = await dbContext.Set<NavigationMenu>().FindAsync(menuId);
        if (menu is null || items is null || !items.Any())
        {
            return false;
        }

        foreach (NavigationMenuItem item in items)
        {
            item.ParentId = null;
        }

        await dbContext.Set<NavigationMenuItem>().AddRangeAsync(items);
        _ = await dbContext.SaveChangesAsync();

        menu.Items ??= [];
        menu.Items.AddRange(items.Select(i => i.Id));

        _ = dbContext.Set<NavigationMenu>().Update(menu);
        _ = await dbContext.SaveChangesAsync();

        return true;
    }

    /// <inheritdoc />
    public async Task SaveNavigationMenuAsync(NavigationMenu menu)
    {
        _ = dbContext.Set<NavigationMenu>().Update(menu);
        _ = await dbContext.SaveChangesAsync();
    }

    /// <inheritdoc />
    public async Task<bool> DeleteNavigationMenuAsync(int id)
    {
        NavigationMenu? menu = await dbContext.Set<NavigationMenu>().FindAsync(id);
        if (menu is null)
        {
            return false;
        }

        List<NavigationMenuItem> itemsToDelete = await dbContext.Set<NavigationMenuItem>()
            .Where(x => menu.Items.Contains(x.Id))
            .ToListAsync();

        if (itemsToDelete.Count > 0)
        {
            dbContext.Set<NavigationMenuItem>().RemoveRange(itemsToDelete);
        }

        _ = dbContext.Set<NavigationMenu>().Remove(menu);
        _ = await dbContext.SaveChangesAsync();

        return true;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<NavigationMenuItem>> GetNavigationMenuItemsAsync()
        => await dbContext.Set<NavigationMenuItem>().AsNoTracking().ToListAsync();

    /// <inheritdoc />
    public async Task<IReadOnlyList<NavigationMenuItem>> GetNavigationMenuItemsAsync(NavigationMenu navigationMenu)
        => await dbContext.Set<NavigationMenuItem>()
            .AsNoTracking()
            .Include(x => x.Children)
            .Where(x => navigationMenu.Items.Contains(x.Id))
            .OrderByDescending(x => x.Order)
            .ToListAsync();

    /// <inheritdoc />
    public async Task<IReadOnlyList<NavigationMenuItem>> GetNavigationMenuItemsAsync(NavigationMenuItem navigationMenuItem)
        => await dbContext.Set<NavigationMenuItem>()
            .AsNoTracking()
            .Include(x => x.Children)
            .Where(x => x.ParentId == navigationMenuItem.Id) // Direct relational search
            .OrderByDescending(x => x.Order)
            .ToListAsync();

    /// <inheritdoc />
    public async Task<NavigationMenuItem?> GetNavigationMenuItemAsync(int id)
        => await dbContext.Set<NavigationMenuItem>()
            .Include(x => x.Children)
            .FirstOrDefaultAsync(x => x.Id == id);

    /// <inheritdoc />
    public async Task<bool> AddChildItemAsync(int parentItemId, NavigationMenuItem childItem)
    {
        NavigationMenuItem? parent = await dbContext.Set<NavigationMenuItem>().FindAsync(parentItemId);
        if (parent is null)
        {
            return false;
        }

        childItem.ParentId = parentItemId;

        _ = await dbContext.Set<NavigationMenuItem>().AddAsync(childItem);
        _ = await dbContext.SaveChangesAsync();

        return true;
    }

    /// <inheritdoc />
    public async Task<bool> AddChildrenItemsAsync(int parentItemId, IEnumerable<NavigationMenuItem> childItems)
    {
        NavigationMenuItem? parent = await dbContext.Set<NavigationMenuItem>().FindAsync(parentItemId);
        if (parent is null || childItems is null || !childItems.Any())
        {
            return false;
        }

        foreach (NavigationMenuItem childItem in childItems)
        {
            childItem.ParentId = parentItemId;
        }

        await dbContext.Set<NavigationMenuItem>().AddRangeAsync(childItems);
        _ = await dbContext.SaveChangesAsync();

        return true;
    }

    /// <inheritdoc />
    public async Task SaveNavigationMenuItemAsync(NavigationMenuItem item)
    {
        _ = dbContext.Set<NavigationMenuItem>().Update(item);
        _ = await dbContext.SaveChangesAsync();
    }

    /// <inheritdoc />
    public async Task<bool> DeleteNavigationMenuItemAsync(int id)
    {
        NavigationMenuItem? itemToDelete = await dbContext.Set<NavigationMenuItem>()
            .Include(x => x.Children)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (itemToDelete is null)
        {
            return false;
        }

        List<NavigationMenuItem> childrenToPromote = [.. itemToDelete.Children];

        List<NavigationMenu> assignedMenus = await dbContext.Set<NavigationMenu>()
            .Where(m => m.Items.Contains(id))
            .ToListAsync();

        foreach (NavigationMenu menu in assignedMenus)
        {
            // Keep the menu's root list valid when a root item is removed.
            _ = menu.Items.Remove(id);
            menu.Items.AddRange(childrenToPromote.Select(c => c.Id));
            menu.Items = [.. menu.Items.Distinct()];
        }

        foreach (NavigationMenuItem child in childrenToPromote)
        {
            // Preserve the remaining tree by moving children to the deleted item's former parent.
            child.ParentId = itemToDelete.ParentId;
            _ = dbContext.Set<NavigationMenuItem>().Update(child);
        }

        _ = dbContext.Set<NavigationMenuItem>().Remove(itemToDelete);
        _ = await dbContext.SaveChangesAsync();

        return true;
    }

    /// <inheritdoc />
    public async Task<List<NavigationMenuItem>> GetAllMenuItemsAsync()
        => await dbContext.Set<NavigationMenuItem>()
            .AsNoTracking()
            .Include(x => x.Children)
            .ToListAsync();
}
