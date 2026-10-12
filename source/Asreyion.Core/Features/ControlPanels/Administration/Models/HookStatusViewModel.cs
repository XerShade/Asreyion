namespace Asreyion.Core.Features.ControlPanels.Administration.Models;

public sealed record HookStatusViewModel(
    string ImplementationType,
    string? Name,
    string? Description,
    IReadOnlyList<string> RegisteredFor);
