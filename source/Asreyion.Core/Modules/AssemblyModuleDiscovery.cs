using Asreyion.Core.Modules.Interfaces;
using System.Reflection;

namespace Asreyion.Core.Modules;

public class AssemblyModuleDiscovery : IModuleDiscovery
{
    public IReadOnlyCollection<ICoreModule> Discover(IEnumerable<Assembly> assemblies)
    {
        List<ICoreModule> modules = [];

        foreach (Assembly assembly in assemblies)
        {
            IEnumerable<Type> types =
                assembly
                    .GetTypes()
                    .Where(type =>
                        typeof(ICoreModule)
                            .IsAssignableFrom(type)
                        && !type.IsAbstract
                        && !type.IsInterface);

            foreach (Type type in types)
            {
                if (Activator.CreateInstance(type)
                    is ICoreModule module)
                {
                    modules.Add(module);
                }
            }
        }

        return modules;
    }
}