using Asreyion.Core.Features.Database.DbContexts;
using Asreyion.Core.Features.Navigation.Data;
using Asreyion.Core.Features.Navigation.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Asreyion.Core.Features.Navigation.Services;

public class NavigationService(DataDbContext dbContext) : INavigationService
{
    public async Task<IReadOnlyList<NavigationMenu>> GetNavigationMenusAsync()
        => await dbContext.NavigationMenus.ToListAsync();

    public async Task<NavigationMenu?> GetNavigationMenuAsync(int id)
        => await dbContext.NavigationMenus.FindAsync(id);

    public async Task<NavigationMenu?> GetNavigationMenuByNameAsync(string name)
    => await dbContext.NavigationMenus.FirstOrDefaultAsync(m => m.Name == name);

    public async Task<bool> AddRootItemAsync(int menuId, NavigationMenuItem item)
    {
        NavigationMenu? menu = await dbContext.NavigationMenus.FindAsync(menuId);
        if (menu is null)
        {
            return false;
        }

        _ = await dbContext.NavigationMenuItems.AddAsync(item);
        _ = await dbContext.SaveChangesAsync();

        menu.Items ??= [];
        menu.Items.Add(item.Id);

        _ = dbContext.NavigationMenus.Update(menu);
        _ = await dbContext.SaveChangesAsync();

        return true;
    }

    public async Task<bool> AddRootItemsAsync(int menuId, IEnumerable<NavigationMenuItem> items)
    {
        NavigationMenu? menu = await dbContext.NavigationMenus.FindAsync(menuId);
        if (menu is null || items is null || !items.Any())
        {
            return false;
        }

        await dbContext.NavigationMenuItems.AddRangeAsync(items);
        _ = await dbContext.SaveChangesAsync();

        menu.Items ??= [];
        menu.Items.AddRange(items.Select(i => i.Id));

        _ = dbContext.NavigationMenus.Update(menu);
        _ = await dbContext.SaveChangesAsync();

        return true;
    }

    public async Task SaveNavigationMenuAsync(NavigationMenu menu)
    {
        _ = dbContext.NavigationMenus.Update(menu);
        _ = await dbContext.SaveChangesAsync();
    }

    public async Task<bool> DeleteNavigationMenuAsync(int id)
    {
        NavigationMenu? menu = await dbContext.NavigationMenus.FindAsync(id);

        if (menu is not null)
        {
            // Recursive Cascade: Find and delete all items explicitly tied to this menu
            await this.DeleteItemsRecursivelyAsync(menu.Items);

            _ = dbContext.NavigationMenus.Remove(menu);
            _ = await dbContext.SaveChangesAsync();

            return true;
        }

        return false;
    }

    private async Task DeleteItemsRecursivelyAsync(List<int> itemIds)
    {
        if (itemIds is null || itemIds.Count == 0)
        {
            return;
        }

        List<NavigationMenuItem> itemsToDelete = await dbContext.NavigationMenuItems
            .Where(x => itemIds.Contains(x.Id))
            .ToListAsync();

        foreach (NavigationMenuItem item in itemsToDelete)
        {
            if (item.Children.Count > 0)
            {
                await this.DeleteItemsRecursivelyAsync(item.Children);
            }

            _ = dbContext.NavigationMenuItems.Remove(item);
        }
    }

    public async Task<IReadOnlyList<NavigationMenuItem>> GetNavigationMenuItemsAsync()
        => await dbContext.NavigationMenuItems.ToListAsync();

    public async Task<IReadOnlyList<NavigationMenuItem>> GetNavigationMenuItemsAsync(NavigationMenu navigationMenu)
        => await dbContext.NavigationMenuItems
            .Where(x => navigationMenu.Items.Contains(x.Id))
            .OrderByDescending(x => x.Order)
            .ToListAsync();

    public async Task<IReadOnlyList<NavigationMenuItem>> GetNavigationMenuItemsAsync(NavigationMenuItem navigationMenuItem)
        => await dbContext.NavigationMenuItems
            .Where(x => navigationMenuItem.Children.Contains(x.Id))
            .OrderByDescending(x => x.Order)
            .ToListAsync();

    public async Task<NavigationMenuItem?> GetNavigationMenuItemAsync(int id)
        => await dbContext.NavigationMenuItems.FindAsync(id);

    public async Task<bool> AddChildItemAsync(int parentItemId, NavigationMenuItem childItem)
    {
        NavigationMenuItem? parent = await dbContext.NavigationMenuItems.FindAsync(parentItemId);
        if (parent is null)
        {
            return false;
        }

        _ = await dbContext.NavigationMenuItems.AddAsync(childItem);
        _ = await dbContext.SaveChangesAsync();

        parent.Children ??= [];
        parent.Children.Add(childItem.Id);

        _ = dbContext.NavigationMenuItems.Update(parent);
        _ = await dbContext.SaveChangesAsync();

        return true;
    }

    public async Task<bool> AddChildrenItemsAsync(int parentItemId, IEnumerable<NavigationMenuItem> childItems)
    {
        NavigationMenuItem? parent = await dbContext.NavigationMenuItems.FindAsync(parentItemId);
        if (parent is null || childItems is null || !childItems.Any())
        {
            return false;
        }

        await dbContext.NavigationMenuItems.AddRangeAsync(childItems);
        _ = await dbContext.SaveChangesAsync();

        parent.Children ??= [];
        parent.Children.AddRange(childItems.Select(c => c.Id));

        _ = dbContext.NavigationMenuItems.Update(parent);
        _ = await dbContext.SaveChangesAsync();

        return true;
    }

    public async Task SaveNavigationMenuItemAsync(NavigationMenuItem item)
    {
        _ = dbContext.NavigationMenuItems.Update(item);
        _ = await dbContext.SaveChangesAsync();
    }

    public async Task<bool> DeleteNavigationMenuItemAsync(int id)
    {
        NavigationMenuItem? itemToDelete = await dbContext.NavigationMenuItems.FindAsync(id);
        if (itemToDelete is null)
        {
            return false;
        }

        List<int> childrenToPromote = itemToDelete.Children;

        // RULE 1: Move children up to the root level of any parent menus using this item
        List<NavigationMenu> parentMenus = await dbContext.NavigationMenus
            .Where(m => m.Items.Contains(id))
            .ToListAsync();

        foreach (NavigationMenu menu in parentMenus)
        {
            _ = menu.Items.Remove(id);
            // Clean out duplicates just in case
            menu.Items.AddRange(childrenToPromote);
            menu.Items = [.. menu.Items.Distinct()];
        }

        // RULE 2: Move children up to the parent item level if this item is nested
        List<NavigationMenuItem> parentItems = await dbContext.NavigationMenuItems
            .Where(m => m.Children.Contains(id))
            .ToListAsync();

        foreach (NavigationMenuItem parentItem in parentItems)
        {
            _ = parentItem.Children.Remove(id);
            parentItem.Children.AddRange(childrenToPromote);
            parentItem.Children = [.. parentItem.Children.Distinct()];
        }

        // FIX: Perform the structural table removal LAST right before pushing to MariaDB
        _ = dbContext.NavigationMenuItems.Remove(itemToDelete);
        _ = await dbContext.SaveChangesAsync();

        return true;
    }

    public async Task<List<NavigationMenuItem>> GetAllMenuItemsAsync()
        => await dbContext.NavigationMenuItems.ToListAsync();
}