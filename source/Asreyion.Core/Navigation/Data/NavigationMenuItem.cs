namespace Asreyion.Core.Navigation.Data;

/// <summary>
/// Legacy database record retained to preserve the existing database model.
/// Runtime navigation items are registered through hooks.
/// </summary>
public class NavigationMenuItem
{
    public int Id { get; set; }

    public int Order { get; set; }

    public string Label { get; set; } = string.Empty;

    public string Icon { get; set; } = string.Empty;

    public string Area { get; set; } = string.Empty;

    public string Controller { get; set; } = string.Empty;

    public string Action { get; set; } = string.Empty;

    public string ItemType { get; set; } = "Link";

    public Dictionary<string, string> RouteValues { get; set; } = [];

    public int? ParentId { get; set; }

    public NavigationMenuItem? Parent { get; set; }

    public ICollection<NavigationMenuItem> Children { get; set; } = [];
}
