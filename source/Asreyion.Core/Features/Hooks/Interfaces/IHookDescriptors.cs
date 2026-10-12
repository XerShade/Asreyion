namespace Asreyion.Core.Features.Hooks.Interfaces;

/// <summary>
/// Defines a marker interface for hooks with descriptors.
/// </summary>
public interface IHookDescriptors
{
    /// <summary>
    /// Gets the name of the hook.
    /// </summary>
    string Name { get; }
    /// <summary>
    /// Gets the description of the hook.
    /// </summary>
    string Description { get; }
}