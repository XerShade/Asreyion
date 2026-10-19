namespace Asreyion.Core.Features.Navigation.Models;

/// <summary>
/// A navigation item prepared for rendering in a nested menu.
/// </summary>
public class NavigationTreeViewModel
{
    /// <summary>
    /// Gets or sets the persisted item's identifier, or zero for a code-defined item.
    /// </summary>
    public int Id { get; set; }
    /// <summary>
    /// Gets or sets the text displayed for the item.
    /// </summary>
    public string Label { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the MVC area for the item's link.
    /// </summary>
    public string Area { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the MVC controller for the item's link.
    /// </summary>
    public string Controller { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the MVC action for the item's link.
    /// </summary>
    public string Action { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the CSS icon name displayed with the item.
    /// </summary>
    public string Icon { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the item's display order.
    /// </summary>
    public int Order { get; set; }
    /// <summary>
    /// Gets or sets the item kind, such as a link, header, or divider.
    /// </summary>
    public string ItemType { get; set; } = "Link";
    /// <summary>
    /// Gets or sets additional MVC route values for the link.
    /// </summary>
    public Dictionary<string, string> RouteValues { get; set; } = [];
    /// <summary>
    /// Gets or sets the child items in the navigation tree.
    /// </summary>
    public List<NavigationTreeViewModel> Children { get; set; } = [];
}
