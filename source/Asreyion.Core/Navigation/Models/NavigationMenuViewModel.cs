namespace Asreyion.Core.Navigation.Models;

/// <summary>
/// A named menu and its ordered tree of renderable navigation items.
/// </summary>
public sealed class NavigationMenuViewModel
{
    public string Name { get; init; } = string.Empty;

    public string HtmlId { get; init; } = string.Empty;

    public IReadOnlyList<NavigationTreeViewModel> Items { get; init; } = [];
}
