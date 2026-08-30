using System.ComponentModel.DataAnnotations;

namespace Asreyion.Core.Features.Navigation.Data;

public class NavigationMenuItem
{
    [Key]
    public int Id { get; set; }
    public int Order { get; set; } = 0;

    public string Label { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public string Area { get; set; } = string.Empty;
    public string Controller { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;

    public string ItemType { get; set; } = "Link";

    public Dictionary<string, string> RouteValues { get; set; } = [];

    public List<int> Children { get; set; } = [];
}
