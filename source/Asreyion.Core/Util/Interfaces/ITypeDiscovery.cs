namespace Asreyion.Core.Util.Interfaces;

public interface ITypeDiscovery<S>
{
    IReadOnlyCollection<Type> Discover<T>(IReadOnlyCollection<S> sources);
}