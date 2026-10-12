using Asreyion.Core.Features.Hooks.Attributes;
using Asreyion.Core.Features.Hooks.Interfaces;
using System.Reflection;

namespace Asreyion.Core.Hooks;

public static class HookDependencySorter
{
    public static IReadOnlyList<Type> Sort(IReadOnlyCollection<Type> hookTypes)
    {
        Dictionary<Type, IReadOnlyCollection<Type>> hookDependenciesMap = [];

        foreach (Type type in hookTypes)
        {
            if (!typeof(IHookSubscriber).IsAssignableFrom(type))
            {
                throw new InvalidOperationException($"Type '{type.FullName}' does not implement IHookSubscriber.");
            }

            // Read the attribute metadata without instantiating the type
            HookDependenciesAttribute? attribute = type.GetCustomAttribute<HookDependenciesAttribute>();

            hookDependenciesMap[type] = attribute?.Dependencies ?? [];
        }

        List<Type> sorted = [];
        HashSet<Type> visiting = [];
        HashSet<Type> visited = [];

        foreach (Type hookType in hookTypes)
        {
            Visit(hookType, hookDependenciesMap, visiting, visited, sorted);
        }

        return sorted;
    }

    private static void Visit(Type hookType, IReadOnlyDictionary<Type, IReadOnlyCollection<Type>> dependencyMap, HashSet<Type> visiting, HashSet<Type> visited, List<Type> sorted)
    {
        // Already processed.
        if (visited.Contains(hookType))
        {
            return;
        }

        // Circular dependency check.
        if (!visiting.Add(hookType))
        {
            throw new InvalidOperationException(
                $"Circular hook dependency detected involving '{hookType.Name}' ({hookType.FullName}).");
        }

        // Process dependencies of this type
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