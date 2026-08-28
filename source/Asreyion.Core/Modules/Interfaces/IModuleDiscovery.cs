using System.Reflection;

namespace Asreyion.Core.Modules.Interfaces;

public interface IModuleDiscovery
{
    IReadOnlyCollection<ICoreModule> Discover(IEnumerable<Assembly> assemblies);
}