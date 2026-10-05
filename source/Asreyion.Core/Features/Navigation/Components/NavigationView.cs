using Asreyion.Core.Features.Hooks.Interfaces;
using Asreyion.Core.Features.Navigation.Builder;
using Asreyion.Core.Features.Navigation.Data;
using Asreyion.Core.Features.Navigation.Hooks;
using Asreyion.Core.Features.Navigation.Models;
using Asreyion.Core.Features.Navigation.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Asreyion.Core.Features.Navigation.Components;

public class NavigationView(INavigationService navService, IHookEngine hookEngine) : ViewComponent
{
    private readonly string ViewPath = "/Features/Navigation/Views/Components/NavigationView.cshtml";

    public async Task<IViewComponentResult> InvokeAsync(string menuName = "Primary")
    {
        // 1. Build code-driven navigation from registered IOnBuildNavigationHook subscribers
        NavigationBuilder builder = new(menuName);
        await hookEngine.ExecuteAsync<IOnBuildNavigationHook>(h => h.BuildNavigationAsync(builder));

        // 2. Fetch database menu items (if any exist)
        NavigationMenu? dbMenu = await navService.GetNavigationMenuByNameAsync(menuName);
        List<NavigationMenuItem> allDbItems = [];
        if (dbMenu != null && dbMenu.Items != null && dbMenu.Items.Count > 0)
        {
            List<NavigationMenuItem> itemsFromDb = await navService.GetAllMenuItemsAsync();
            allDbItems = itemsFromDb.Where(x => dbMenu.Items.Contains(x.Id)).ToList();
        }

        // 3. Merge code-driven items with DB items (deduplicating parents/children by label)
        List<NavigationTreeViewModel> mergedNodes = MergeNavigation(builder.Items, allDbItems);

        return this.View(this.ViewPath, mergedNodes.OrderBy(x => x.Order).ToList());
    }

    private static List<NavigationTreeViewModel> MergeNavigation(
        IReadOnlyList<NavigationItemDefinition> codeItems,
        List<NavigationMenuItem> dbItems)
    {
        List<NavigationTreeViewModel> result = [];

        // Add code-driven root items
        foreach (NavigationItemDefinition codeItem in codeItems)
        {
            NavigationTreeViewModel node = MapCodeToViewModel(codeItem);
            result.Add(node);
        }

        // Merge DB root items
        List<NavigationMenuItem> rootDbItems = dbItems.Where(x => x.ParentId == null).ToList();
        foreach (NavigationMenuItem dbItem in rootDbItems)
        {
            NavigationTreeViewModel? existing = result
                .FirstOrDefault(x => string.Equals(x.Label, dbItem.Label, StringComparison.OrdinalIgnoreCase));

            if (existing is null)
            {
                result.Add(MapDbToViewModel(dbItem));
            }
            else
            {
                // Override metadata from DB if provided
                if (!string.IsNullOrEmpty(dbItem.Icon)) existing.Icon = dbItem.Icon;
                if (dbItem.Order != 0) existing.Order = dbItem.Order;
                if (!string.IsNullOrEmpty(dbItem.Controller)) existing.Controller = dbItem.Controller;
                if (!string.IsNullOrEmpty(dbItem.Action)) existing.Action = dbItem.Action;
                if (!string.IsNullOrEmpty(dbItem.Area)) existing.Area = dbItem.Area;

                // Merge children from DB
                if (dbItem.Children != null && dbItem.Children.Count > 0)
                {
                    foreach (NavigationMenuItem dbChild in dbItem.Children)
                    {
                        NavigationTreeViewModel? existingChild = existing.Children
                            .FirstOrDefault(c => string.Equals(c.Label, dbChild.Label, StringComparison.OrdinalIgnoreCase));

                        if (existingChild is null)
                        {
                            existing.Children.Add(MapDbToViewModel(dbChild));
                        }
                    }
                }
            }
        }

        // Order nodes and children recursively
        foreach (NavigationTreeViewModel node in result)
        {
            node.Children = node.Children.OrderBy(c => c.Order).ToList();
        }

        return result.OrderBy(x => x.Order).ToList();
    }

    private static NavigationTreeViewModel MapCodeToViewModel(NavigationItemDefinition codeItem)
    {
        NavigationTreeViewModel vm = new()
        {
            Id = 0,
            Label = codeItem.Label,
            Area = codeItem.Area,
            Controller = codeItem.Controller,
            Action = codeItem.Action,
            Icon = codeItem.Icon,
            Order = codeItem.Order,
            ItemType = codeItem.ItemType,
            RouteValues = new Dictionary<string, string>(codeItem.RouteValues)
        };

        foreach (NavigationItemDefinition child in codeItem.Children)
        {
            vm.Children.Add(MapCodeToViewModel(child));
        }

        return vm;
    }

    private static NavigationTreeViewModel MapDbToViewModel(NavigationMenuItem dbItem)
    {
        NavigationTreeViewModel vm = new()
        {
            Id = dbItem.Id,
            Label = dbItem.Label,
            Area = dbItem.Area,
            Controller = dbItem.Controller,
            Action = dbItem.Action,
            Icon = dbItem.Icon,
            Order = dbItem.Order,
            ItemType = dbItem.ItemType,
            RouteValues = dbItem.RouteValues ?? []
        };

        if (dbItem.Children != null && dbItem.Children.Count > 0)
        {
            foreach (NavigationMenuItem child in dbItem.Children)
            {
                vm.Children.Add(MapDbToViewModel(child));
            }
        }

        return vm;
    }
}

