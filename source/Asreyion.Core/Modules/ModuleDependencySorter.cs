using Asreyion.Core.Modules.Interfaces;

namespace Asreyion.Core.Modules;

public sealed class ModuleDependencySorter : IModuleDependencySorter
{
    public IReadOnlyList<ICoreModule> Sort(IReadOnlyCollection<ICoreModule> modules)
    {
        Dictionary<Type, ICoreModule> moduleMap = modules.ToDictionary(module => module.GetType());

        List<ICoreModule> sorted = [];
        HashSet<Type> visiting = [];
        HashSet<Type> visited = [];

        foreach (ICoreModule module in modules)
        {
            Visit(module, moduleMap, visiting, visited, sorted);
        }

        return sorted;
    }

    private static void Visit(ICoreModule module, IReadOnlyDictionary<Type, ICoreModule> moduleMap, HashSet<Type> visiting, HashSet<Type> visited, List<ICoreModule> sorted)
    {
        Type moduleType = module.GetType();

        // Already processed.
        if (visited.Contains(moduleType))
        {
            return;
        }

        // We've reached this module while it is already
        // being processed, meaning we have a circular dependency.
        if (!visiting.Add(moduleType))
        {
            throw new InvalidOperationException(
                $"Circular module dependency detected involving " +
                $"'{module.Name}' ({moduleType.FullName}).");
        }

        foreach (Type dependencyType in module.Dependencies)
        {
            if (!moduleMap.TryGetValue(dependencyType, out ICoreModule? dependency))
            {
                throw new InvalidOperationException(
                    $"Module '{module.Name}' depends on " +
                    $"'{dependencyType.FullName}', but that module " +
                    $"was not discovered.");
            }

            Visit(dependency, moduleMap, visiting, visited, sorted);
        }

        _ = visiting.Remove(moduleType);
        _ = visited.Add(moduleType);

        sorted.Add(module);
    }
}