using Asreyion.Core.Features.Navigation.Data;
using Asreyion.Core.Features.Navigation.Models;
using Asreyion.Core.Features.Navigation.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Asreyion.Core.Features.Navigation.Components;

public class NavigationView(INavigationService navService) : ViewComponent
{
    private readonly string ViewPath = "/Features/Navigation/Views/Components/NavigationView.cshtml";

    public async Task<IViewComponentResult> InvokeAsync(string menuName = "Primary")
    {
        NavigationMenu? menu = await navService.GetNavigationMenuByNameAsync(menuName);
        if (menu == null || menu.Items == null || menu.Items.Count == 0)
        {
            return this.View(this.ViewPath, new List<NavigationTreeViewModel>());
        }

        List<NavigationMenuItem> allItemsFromDb = await navService.GetAllMenuItemsAsync();

        List<NavigationMenuItem> rootItems = [.. allItemsFromDb
            .Where(x => menu.Items.Contains(x.Id) && x.ParentId == null)
            .OrderBy(x => x.Order)];

        List<NavigationTreeViewModel> rootNodes = [];
        foreach (NavigationMenuItem? rootItem in rootItems)
        {
            rootNodes.Add(this.BuildTreeRecursively(rootItem));
        }

        return this.View(this.ViewPath, rootNodes.OrderBy(x => x.Order).ToList());
    }

    private NavigationTreeViewModel BuildTreeRecursively(Data.NavigationMenuItem currentItem)
    {
        NavigationTreeViewModel node = new()
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

        if (currentItem.Children != null && currentItem.Children.Count > 0)
        {
            foreach (NavigationMenuItem childItem in currentItem.Children)
            {
                node.Children.Add(this.BuildTreeRecursively(childItem));
            }
            node.Children = node.Children.OrderBy(x => x.Order).ToList();
        }

        return node;
    }
}
