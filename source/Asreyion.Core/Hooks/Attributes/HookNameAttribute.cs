namespace Asreyion.Core.Hooks.Attributes;

/// <summary>
/// Declares the name of the decorated subscriber.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class HookNameAttribute(string name) : Attribute
{
    /// <summary>
    /// Gets the name of the hook subscriber.
    /// </summary>
    public string Name { get; } = name;
}