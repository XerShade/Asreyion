using Asreyion.Core.Features.Navigation.Data;

namespace Asreyion.Core.Features.Navigation.Services.Interfaces;

public interface INavigationService
{
    Task<bool> AddChildItemAsync(int parentItemId, NavigationMenuItem childItem);
    Task<bool> AddChildrenItemsAsync(int parentItemId, IEnumerable<NavigationMenuItem> childItems);
    Task<bool> AddRootItemAsync(int menuId, NavigationMenuItem item);
    Task<bool> AddRootItemsAsync(int menuId, IEnumerable<NavigationMenuItem> items);
    Task<bool> DeleteNavigationMenuAsync(int id);
    Task<bool> DeleteNavigationMenuItemAsync(int id);
    Task<NavigationMenu?> GetNavigationMenuAsync(int id);
    Task<NavigationMenuItem?> GetNavigationMenuItemAsync(int id);
    Task<IReadOnlyList<NavigationMenuItem>> GetNavigationMenuItemsAsync();
    Task<IReadOnlyList<NavigationMenuItem>> GetNavigationMenuItemsAsync(NavigationMenu navigationMenu);
    Task<IReadOnlyList<NavigationMenuItem>> GetNavigationMenuItemsAsync(NavigationMenuItem navigationMenuItem);
    Task<IReadOnlyList<NavigationMenu>> GetNavigationMenusAsync();
    Task SaveNavigationMenuAsync(NavigationMenu menu);
    Task SaveNavigationMenuItemAsync(NavigationMenuItem item);
    Task<NavigationMenu?> GetNavigationMenuByNameAsync(string name);
    Task<List<NavigationMenuItem>> GetAllMenuItemsAsync();
}