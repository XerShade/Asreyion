namespace Asreyion.Core.Features.Navigation.Data;

/// <summary>
/// A persisted navigation link or structural item, optionally nested under another item.
/// </summary>
public class NavigationMenuItem
{
    /// <summary>
    /// Gets or sets the item's database identifier.
    /// </summary>
    public int Id { get; set; }
    /// <summary>
    /// Gets or sets the item's position among its siblings.
    /// </summary>
    public int Order { get; set; } = 0;

    /// <summary>
    /// Gets or sets the label shown in the menu.
    /// </summary>
    public string Label { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the CSS icon name used by the menu.
    /// </summary>
    public string Icon { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the MVC area for link items.
    /// </summary>
    public string Area { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the MVC controller for link items.
    /// </summary>
    public string Controller { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the MVC action for link items.
    /// </summary>
    public string Action { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the item kind, such as a link, header, or divider.
    /// </summary>
    public string ItemType { get; set; } = "Link";

    /// <summary>
    /// Gets or sets additional MVC route values for the link.
    /// </summary>
    public Dictionary<string, string> RouteValues { get; set; } = [];

    /// <summary>
    /// Gets or sets the parent item's identifier, when nested.
    /// </summary>
    public int? ParentId { get; set; }
    /// <summary>
    /// Gets or sets the parent navigation item.
    /// </summary>
    public NavigationMenuItem? Parent { get; set; }

    /// <summary>
    /// Gets or sets the items nested below this item.
    /// </summary>
    public ICollection<NavigationMenuItem> Children { get; set; } = [];
}
