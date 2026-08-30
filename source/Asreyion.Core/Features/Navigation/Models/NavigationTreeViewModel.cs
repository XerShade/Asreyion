namespace Asreyion.Core.Features.Navigation.Models;

public class NavigationTreeViewModel
{
    public int Id { get; set; }
    public string Label { get; set; } = string.Empty;
    public string Area { get; set; } = string.Empty;
    public string Controller { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public int Order { get; set; }
    public string ItemType { get; set; } = "Link";
    public Dictionary<string, string> RouteValues { get; set; } = [];
    public List<NavigationTreeViewModel> Children { get; set; } = [];
}
