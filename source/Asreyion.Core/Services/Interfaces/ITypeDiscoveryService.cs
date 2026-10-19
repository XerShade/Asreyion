namespace Asreyion.Core.Services.Interfaces;

/// <summary>
/// Finds concrete implementations of a requested contract in a set of sources.
/// </summary>
/// <typeparam name="S">The kind of source to inspect, such as an assembly.</typeparam>
public interface ITypeDiscoveryService<S>
{
    /// <summary>
    /// Returns concrete types implementing or deriving from the requested contract.
    /// </summary>
    /// <typeparam name="T">The contract whose implementations should be found.</typeparam>
    /// <param name="sources">The sources to search.</param>
    /// <returns>The matching concrete types.</returns>
    IReadOnlyCollection<Type> Discover<T>(IReadOnlyCollection<S> sources);
}
