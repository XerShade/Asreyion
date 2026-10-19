using Asreyion.Core.Navigation.Data;

namespace Asreyion.Core.Navigation.Services.Interfaces;

/// <summary>
/// Reads and maintains navigation menus and their hierarchical items.
/// </summary>
public interface INavigationService
{
    /// <summary>
    /// Adds one item beneath a parent item.
    /// </summary>
    /// <param name="parentItemId">The parent item's identifier.</param>
    /// <param name="childItem">The item to add.</param>
    /// <returns><see langword="true"/> when the parent exists and the item is saved.</returns>
    Task<bool> AddChildItemAsync(int parentItemId, NavigationMenuItem childItem);
    /// <summary>
    /// Adds multiple items beneath a parent item.
    /// </summary>
    /// <param name="parentItemId">The parent item's identifier.</param>
    /// <param name="childItems">The items to add.</param>
    /// <returns><see langword="true"/> when the parent exists and the items are saved.</returns>
    Task<bool> AddChildrenItemsAsync(int parentItemId, IEnumerable<NavigationMenuItem> childItems);
    /// <summary>
    /// Adds one root item to a menu.
    /// </summary>
    /// <param name="menuId">The menu's identifier.</param>
    /// <param name="item">The item to add.</param>
    /// <returns><see langword="true"/> when the menu exists and the item is saved.</returns>
    Task<bool> AddRootItemAsync(int menuId, NavigationMenuItem item);
    /// <summary>
    /// Adds multiple root items to a menu.
    /// </summary>
    /// <param name="menuId">The menu's identifier.</param>
    /// <param name="items">The items to add.</param>
    /// <returns><see langword="true"/> when the menu exists and the items are saved.</returns>
    Task<bool> AddRootItemsAsync(int menuId, IEnumerable<NavigationMenuItem> items);
    /// <summary>
    /// Deletes a menu and the root items associated with it.
    /// </summary>
    /// <param name="id">The menu's identifier.</param>
    /// <returns><see langword="true"/> when the menu existed and was deleted.</returns>
    Task<bool> DeleteNavigationMenuAsync(int id);
    /// <summary>
    /// Deletes an item and promotes its children to its former parent.
    /// </summary>
    /// <param name="id">The item's identifier.</param>
    /// <returns><see langword="true"/> when the item existed and was deleted.</returns>
    Task<bool> DeleteNavigationMenuItemAsync(int id);
    /// <summary>
    /// Finds a menu by its identifier.
    /// </summary>
    /// <param name="id">The menu's identifier.</param>
    /// <returns>The matching menu, or <see langword="null"/> when it does not exist.</returns>
    Task<NavigationMenu?> GetNavigationMenuAsync(int id);
    /// <summary>
    /// Finds an item by its identifier, including its children.
    /// </summary>
    /// <param name="id">The item's identifier.</param>
    /// <returns>The matching item, or <see langword="null"/> when it does not exist.</returns>
    Task<NavigationMenuItem?> GetNavigationMenuItemAsync(int id);
    /// <summary>
    /// Gets every navigation item.
    /// </summary>
    /// <returns>All persisted navigation items.</returns>
    Task<IReadOnlyList<NavigationMenuItem>> GetNavigationMenuItemsAsync();
    /// <summary>
    /// Gets the root items assigned to a menu.
    /// </summary>
    /// <param name="navigationMenu">The menu whose items should be loaded.</param>
    /// <returns>The menu's root items in display order.</returns>
    Task<IReadOnlyList<NavigationMenuItem>> GetNavigationMenuItemsAsync(NavigationMenu navigationMenu);
    /// <summary>
    /// Gets the direct children of an item.
    /// </summary>
    /// <param name="navigationMenuItem">The parent item.</param>
    /// <returns>The item's children in display order.</returns>
    Task<IReadOnlyList<NavigationMenuItem>> GetNavigationMenuItemsAsync(NavigationMenuItem navigationMenuItem);
    /// <summary>
    /// Gets all navigation menus.
    /// </summary>
    /// <returns>The persisted menus.</returns>
    Task<IReadOnlyList<NavigationMenu>> GetNavigationMenusAsync();
    /// <summary>
    /// Saves changes to a navigation menu.
    /// </summary>
    /// <param name="menu">The menu to save.</param>
    Task SaveNavigationMenuAsync(NavigationMenu menu);
    /// <summary>
    /// Saves changes to a navigation item.
    /// </summary>
    /// <param name="item">The item to save.</param>
    Task SaveNavigationMenuItemAsync(NavigationMenuItem item);
    /// <summary>
    /// Finds a menu by name.
    /// </summary>
    /// <param name="name">The menu name to find.</param>
    /// <returns>The matching menu, or <see langword="null"/> when it does not exist.</returns>
    Task<NavigationMenu?> GetNavigationMenuByNameAsync(string name);
    /// <summary>
    /// Gets all navigation items with their direct children included.
    /// </summary>
    /// <returns>All persisted items and their direct child collections.</returns>
    Task<List<NavigationMenuItem>> GetAllMenuItemsAsync();
}
