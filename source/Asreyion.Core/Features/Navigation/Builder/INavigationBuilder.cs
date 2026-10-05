namespace Asreyion.Core.Features.Navigation.Builder;

/// <summary>
/// Defines a contract for building code-driven navigation menus.
/// </summary>
public interface INavigationBuilder
{
    /// <summary>
    /// Gets the name of the navigation menu being built (e.g. "Primary").
    /// </summary>
    string MenuName { get; }

    /// <summary>
    /// Adds or retrieves a root navigation item by label. If an item with the label already exists, it returns the existing item.
    /// </summary>
    /// <param name="label">The label/name of the item (e.g. "About" or "Blog").</param>
    /// <param name="configure">Optional configuration action.</param>
    /// <returns>The created or existing navigation item definition.</returns>
    NavigationItemDefinition AddOrGet(string label, Action<NavigationItemDefinition>? configure = null);

    /// <summary>
    /// Gets all top-level navigation item definitions registered so far.
    /// </summary>
    IReadOnlyList<NavigationItemDefinition> Items { get; }
}

