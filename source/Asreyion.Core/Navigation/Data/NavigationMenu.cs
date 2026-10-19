using System.ComponentModel.DataAnnotations;

namespace Asreyion.Core.Navigation.Data;

/// <summary>
/// A named navigation menu containing references to its root item records.
/// </summary>
public class NavigationMenu
{
    /// <summary>
    /// Gets or sets the menu's database identifier.
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the name used to select this menu.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the identifiers of the menu's root items.
    /// </summary>
    public List<int> Items { get; set; } = [];
}
