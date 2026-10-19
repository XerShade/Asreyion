namespace Asreyion.Core.Features.Navigation.Builder;

/// <summary>
/// Defines a code-driven navigation menu item definition.
/// </summary>
public class NavigationItemDefinition
{
    /// <summary>
    /// Display label for the navigation item.
    /// </summary>
    public string Label { get; set; }

    /// <summary>
    /// ASP.NET Core Area name.
    /// </summary>
    public string Area { get; set; } = string.Empty;

    /// <summary>
    /// Controller name.
    /// </summary>
    public string Controller { get; set; } = string.Empty;

    /// <summary>
    /// Action name.
    /// </summary>
    public string Action { get; set; } = string.Empty;

    /// <summary>
    /// FontAwesome or CSS icon name.
    /// </summary>
    public string Icon { get; set; } = string.Empty;

    /// <summary>
    /// Order index for sorting.
    /// </summary>
    public int Order { get; set; } = 100;

    /// <summary>
    /// Type of item: "Link", "Dropdown", "Header", "Divider".
    /// </summary>
    public string ItemType { get; set; } = "Link";

    /// <summary>
    /// Additional route values.
    /// </summary>
    public Dictionary<string, string> RouteValues { get; set; } = [];

    /// <summary>
    /// Child navigation items under this parent.
    /// </summary>
    public List<NavigationItemDefinition> Children { get; } = [];

    /// <summary>
    /// Creates a navigation item with the supplied display label.
    /// </summary>
    /// <param name="label">The label shown for the item.</param>
    public NavigationItemDefinition(string label)
    {
        this.Label = label;
    }

    /// <summary>
    /// Adds or retrieves a child navigation item by label.
    /// Reuses an existing child if one with the same label already exists.
    /// </summary>
    public NavigationItemDefinition AddOrGetChild(string label, Action<NavigationItemDefinition>? configure = null)
    {
        NavigationItemDefinition? existing = this.Children
            .FirstOrDefault(c => string.Equals(c.Label, label, StringComparison.OrdinalIgnoreCase));

        if (existing is null)
        {
            existing = new NavigationItemDefinition(label);
            this.Children.Add(existing);
        }

        configure?.Invoke(existing);
        return existing;
    }

    /// <summary>
    /// Sets the MVC destination for this item.
    /// </summary>
    /// <param name="controller">The destination controller.</param>
    /// <param name="action">The destination action.</param>
    /// <param name="area">The destination area, if any.</param>
    /// <returns>This item, for fluent configuration.</returns>
    public NavigationItemDefinition WithRoute(string controller, string action = "Index", string area = "")
    {
        this.Controller = controller;
        this.Action = action;
        this.Area = area;
        return this;
    }

    /// <summary>
    /// Sets the icon shown beside this item.
    /// </summary>
    /// <param name="icon">The CSS or icon-library name.</param>
    /// <returns>This item, for fluent configuration.</returns>
    public NavigationItemDefinition WithIcon(string icon)
    {
        this.Icon = icon;
        return this;
    }

    /// <summary>
    /// Sets the structural kind of this item.
    /// </summary>
    /// <param name="itemType">The kind, such as a link, header, or divider.</param>
    /// <returns>This item, for fluent configuration.</returns>
    public NavigationItemDefinition WithItemType(string itemType)
    {
        this.ItemType = itemType;
        return this;
    }

    /// <summary>
    /// Sets this item's position relative to its siblings.
    /// </summary>
    /// <param name="order">The sort order.</param>
    /// <returns>This item, for fluent configuration.</returns>
    public NavigationItemDefinition WithOrder(int order)
    {
        this.Order = order;
        return this;
    }

    /// <summary>
    /// Adds or replaces a route value for the item's link.
    /// </summary>
    /// <param name="key">The route value name.</param>
    /// <param name="value">The route value.</param>
    /// <returns>This item, for fluent configuration.</returns>
    public NavigationItemDefinition WithRouteValue(string key, string value)
    {
        this.RouteValues[key] = value;
        return this;
    }
}

