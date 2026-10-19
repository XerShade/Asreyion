namespace Asreyion.Core.Features.Hooks.Attributes;

/// <summary>
/// Declares hook subscribers that must be configured before the decorated subscriber.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class HookDependenciesAttribute(params Type[] dependencies) : Attribute
{
    /// <summary>
    /// Gets the hook types that must be processed first.
    /// </summary>
    public IReadOnlyCollection<Type> Dependencies { get; } = dependencies;
}
