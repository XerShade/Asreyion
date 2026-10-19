namespace Asreyion.Core.Services.Interfaces;

public interface ITypeDiscoveryService<S>
{
    IReadOnlyCollection<Type> Discover<T>(IReadOnlyCollection<S> sources);
}