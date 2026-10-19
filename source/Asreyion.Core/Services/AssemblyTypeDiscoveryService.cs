using Asreyion.Core.Services.Interfaces;
using System.Reflection;

namespace Asreyion.Core.Services;

public class AssemblyTypeDiscoveryService : ITypeDiscoveryService<Assembly>
{
    public IReadOnlyCollection<Type> Discover<T>(IReadOnlyCollection<Assembly> sources)
    {
        Type baseType = typeof(T);

        return [.. sources
            .SelectMany(static assembly => assembly.GetTypes())
            .Where(type =>
                baseType.IsAssignableFrom(type)
                && !type.IsAbstract
                && !type.IsInterface)];
    }
}