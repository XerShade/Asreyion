using Asreyion.Core.Hooks.Engine.Interfaces;
using Asreyion.Core.Navigation.Builder;
using Asreyion.Core.Navigation.Hooks;
using Asreyion.Core.Navigation.Models;
using Microsoft.AspNetCore.Mvc;

namespace Asreyion.Core.Navigation.Components;

/// <summary>
/// Builds and renders a named navigation menu contributed by application hooks.
/// </summary>
public sealed class NavigationView(IHookEngine hookEngine) : ViewComponent
{
    private const string ViewPath = "/Navigation/Views/Components/NavigationView.cshtml";

    /// <summary>
    /// Renders the requested code-defined menu.
    /// </summary>
    /// <param name="menuName">The name of the menu to render.</param>
    /// <returns>The rendered menu component.</returns>
    public async Task<IViewComponentResult> InvokeAsync(string menuName = "Primary")
    {
        if (string.IsNullOrWhiteSpace(menuName))
        {
            throw new ArgumentException("A menu name is required.", nameof(menuName));
        }

        NavigationBuilder builder = new(menuName);
        await hookEngine.ExecuteAsync<IOnBuildNavigationHook>(hook => hook.BuildNavigationAsync(builder));

        List<NavigationTreeViewModel> menuItems = builder.Items
            .OrderBy(item => item.Order)
            .Select(MapNavigationItem)
            .ToList();

        NavigationMenuViewModel viewModel = new()
        {
            Name = menuName,
            HtmlId = CreateHtmlId(menuName),
            Items = menuItems
        };

        return this.View(ViewPath, viewModel);
    }

    private static string CreateHtmlId(string menuName)
    {
        List<char> idCharacters = [];

        foreach (char character in menuName)
        {
            if (char.IsLetterOrDigit(character))
            {
                idCharacters.Add(char.ToLowerInvariant(character));
            }
            else if (idCharacters.Count > 0 && idCharacters[^1] != '-')
            {
                idCharacters.Add('-');
            }
        }

        string menuSlug = new string(idCharacters.ToArray()).Trim('-');

        if (string.IsNullOrEmpty(menuSlug))
        {
            menuSlug = "menu";
        }

        return $"navigation-{menuSlug}";
    }

    private static NavigationTreeViewModel MapNavigationItem(NavigationItemDefinition item)
    {
        NavigationTreeViewModel viewModel = new()
        {
            Label = item.Label,
            Area = item.Area,
            Controller = item.Controller,
            Action = item.Action,
            Icon = item.Icon,
            Order = item.Order,
            ItemType = item.ItemType,
            RouteValues = new Dictionary<string, string>(item.RouteValues),
            Children = item.Children
                .OrderBy(child => child.Order)
                .Select(MapNavigationItem)
                .ToList()
        };

        return viewModel;
    }
}
