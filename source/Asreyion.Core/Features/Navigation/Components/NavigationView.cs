using Asreyion.Core.Features.Navigation.Models;
using Asreyion.Core.Features.Navigation.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Asreyion.Core.Features.Navigation.Components;

public class NavigationView(INavigationService navService) : ViewComponent
{
    private readonly string ViewPath = "/Features/Navigation/Views/Components/NavigationView.cshtml";

    public async Task<IViewComponentResult> InvokeAsync(string menuName = "Primary")
    {
        var menu = await navService.GetNavigationMenuByNameAsync(menuName);
        if (menu == null || menu.Items == null || menu.Items.Count == 0)
        {
            return View(this.ViewPath, new List<NavigationTreeViewModel>());
        }

        // Single DB round-trip fetch optimization: Load all map nodes at once into RAM memory
        var allItemsFromDb = await navService.GetAllMenuItemsAsync();
        var itemCache = allItemsFromDb.ToDictionary(x => x.Id);

        // Build hierarchical node projection
        var rootNodes = new List<NavigationTreeViewModel>();
        foreach (var id in menu.Items)
        {
            if (itemCache.TryGetValue(id, out var rootItem))
            {
                rootNodes.Add(BuildTreeRecursively(rootItem, itemCache));
            }
        }

        return View(this.ViewPath, rootNodes.OrderBy(x => x.Order).ToList());
    }

    private NavigationTreeViewModel BuildTreeRecursively(
        Data.NavigationMenuItem currentItem,
        Dictionary<int, Data.NavigationMenuItem> cache)
    {
        var node = new NavigationTreeViewModel
        {
            Id = currentItem.Id,
            Label = currentItem.Label,
            Area = currentItem.Area,
            Controller = currentItem.Controller,
            Action = currentItem.Action,
            Icon = currentItem.Icon,
            Order = currentItem.Order,
            ItemType = currentItem.ItemType,
            RouteValues = currentItem.RouteValues ?? []
        };

        if (currentItem.Children != null)
        {
            foreach (var childId in currentItem.Children)
            {
                if (cache.TryGetValue(childId, out var childItem))
                {
                    node.Children.Add(BuildTreeRecursively(childItem, cache));
                }
            }
            node.Children = node.Children.OrderBy(x => x.Order).ToList();
        }

        return node;
    }
}
