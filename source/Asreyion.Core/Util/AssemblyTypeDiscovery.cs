using Asreyion.Core.Util.Interfaces;
using System.Reflection;

namespace Asreyion.Core.Util;

public class AssemblyTypeDiscovery : ITypeDiscovery<Assembly>
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