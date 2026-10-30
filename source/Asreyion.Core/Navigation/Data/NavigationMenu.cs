using System.ComponentModel.DataAnnotations;

namespace Asreyion.Core.Navigation.Data;

/// <summary>
/// Legacy database record retained to preserve the existing database model.
/// Runtime navigation menus are registered through hooks.
/// </summary>
public class NavigationMenu
{
    [Key]
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public List<int> Items { get; set; } = [];
}
