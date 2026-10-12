namespace Asreyion.Core.Features.Hooks.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class HookDependenciesAttribute(params Type[] dependencies) : Attribute
{
    public IReadOnlyCollection<Type> Dependencies { get; } = dependencies;
}