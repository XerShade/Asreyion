using System.ComponentModel.DataAnnotations;

namespace Asreyion.Core.Features.Navigation.Data;

public class NavigationMenu
{
    [Key]
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public List<int> Items { get; set; } = [];
}
