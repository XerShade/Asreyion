namespace Asreyion.Core.Hooks.Attributes;

/// <summary>
/// Declares the description of the decorated subscriber.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class HookDescriptionAttribute(string description) : Attribute
{
    /// <summary>
    /// Gets the description of the hook subscriber.
    /// </summary>
    public string Description { get; } = description;
}