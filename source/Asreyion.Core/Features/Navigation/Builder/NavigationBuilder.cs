namespace Asreyion.Core.Features.Navigation.Builder;

/// <summary>
/// Implements a navigation builder that merges and deduplicates items by label.
/// </summary>
public class NavigationBuilder(string menuName) : INavigationBuilder
{
    private readonly List<NavigationItemDefinition> _items = [];

    /// <inheritdoc />
    public string MenuName { get; } = menuName;

    /// <inheritdoc />
    public IReadOnlyList<NavigationItemDefinition> Items => this._items.AsReadOnly();

    /// <inheritdoc />
    public NavigationItemDefinition AddOrGet(string label, Action<NavigationItemDefinition>? configure = null)
    {
        NavigationItemDefinition? existing = this._items
            .FirstOrDefault(i => string.Equals(i.Label, label, StringComparison.OrdinalIgnoreCase));

        if (existing is null)
        {
            existing = new NavigationItemDefinition(label);
            this._items.Add(existing);
        }

        configure?.Invoke(existing);
        return existing;
    }
}

