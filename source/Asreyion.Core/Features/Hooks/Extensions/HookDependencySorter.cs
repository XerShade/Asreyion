using Asreyion.Core.Features.Hooks.Attributes;
using Asreyion.Core.Features.Hooks.Interfaces;
using System.Reflection;

namespace Asreyion.Core.Features.Hooks.Extensions;

/// <summary>
/// Orders hook subscribers so each declared dependency is processed first.
/// </summary>
public static class HookDependencySorter
{
    /// <summary>
    /// Sorts discovered hook types according to their <see cref="HookDependenciesAttribute"/> declarations.
    /// </summary>
    /// <param name="hookTypes">The hook subscriber types to order.</param>
    /// <returns>The types in dependency order.</returns>
    /// <exception cref="InvalidOperationException">
    /// A type is not a hook subscriber, a dependency is missing, or the dependencies contain a cycle.
    /// </exception>
    public static IReadOnlyList<Type> Sort(IReadOnlyCollection<Type> hookTypes)
    {
        Dictionary<Type, IReadOnlyCollection<Type>> hookDependenciesMap = [];

        foreach (Type type in hookTypes)
        {
            if (!typeof(IHookSubscriber).IsAssignableFrom(type))
            {
                throw new InvalidOperationException($"Type '{type.FullName}' does not implement IHookSubscriber.");
            }

            // Read dependency metadata without constructing the subscriber.
            HookDependenciesAttribute? attribute = type.GetCustomAttribute<HookDependenciesAttribute>();

            hookDependenciesMap[type] = attribute?.Dependencies ?? [];
        }

        List<Type> sorted = [];
        HashSet<Type> visiting = [];
        HashSet<Type> visited = [];

        // Visit dependencies first so the returned list is ready for sequential registration.
        foreach (Type hookType in hookTypes)
        {
            Visit(hookType, hookDependenciesMap, visiting, visited, sorted);
        }

        return sorted;
    }

    private static void Visit(Type hookType, IReadOnlyDictionary<Type, IReadOnlyCollection<Type>> dependencyMap, HashSet<Type> visiting, HashSet<Type> visited, List<Type> sorted)
    {
        if (visited.Contains(hookType))
        {
            return;
        }

        if (!visiting.Add(hookType))
        {
            throw new InvalidOperationException(
                $"Circular hook dependency detected involving '{hookType.Name}' ({hookType.FullName}).");
        }

        if (dependencyMap.TryGetValue(hookType, out IReadOnlyCollection<Type>? dependencies))
        {
            foreach (Type dependencyType in dependencies)
            {
                if (!dependencyMap.ContainsKey(dependencyType))
                {
                    throw new InvalidOperationException(
                        $"Hook '{hookType.Name}' depends on '{dependencyType.FullName}', but that hook type was not discovered in the provided assembly.");
                }

                Visit(dependencyType, dependencyMap, visiting, visited, sorted);
            }
        }

        _ = visiting.Remove(hookType);
        _ = visited.Add(hookType);

        sorted.Add(hookType);
    }
}
