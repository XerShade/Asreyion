namespace Asreyion.Core.Features.ControlPanels.Administration.Models;

/// <summary>
/// Describes a discovered hook and the contracts it is registered to provide.
/// </summary>
/// <param name="ImplementationType">The hook implementation's full type name.</param>
/// <param name="Name">The friendly name supplied by the hook, when available.</param>
/// <param name="Description">The description supplied by the hook, when available.</param>
/// <param name="RegisteredFor">The hook contracts registered for the implementation.</param>
public sealed record HookStatusViewModel(
    string ImplementationType,
    string? Name,
    string? Description,
    IReadOnlyList<string> RegisteredFor);
