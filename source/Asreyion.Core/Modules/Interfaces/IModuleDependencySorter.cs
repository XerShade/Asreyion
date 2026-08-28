namespace Asreyion.Core.Modules.Interfaces;

public interface IModuleDependencySorter
{
    IReadOnlyList<ICoreModule> Sort(IReadOnlyCollection<ICoreModule> modules);
}