using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.Json;

namespace Asreyion.Core.Navigation.Data.Configurations;

/// <summary>
/// Preserves the legacy navigation item mapping for database compatibility.
/// </summary>
public class NavigationMenuItemConfiguration : IEntityTypeConfiguration<NavigationMenuItem>
{
    public void Configure(EntityTypeBuilder<NavigationMenuItem> builder)
    {
        _ = builder.HasKey(item => item.Id);

        _ = builder.HasOne(item => item.Parent)
            .WithMany(item => item.Children)
            .HasForeignKey(item => item.ParentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(item => item.RouteValues)
            .HasColumnType("json")
            .HasConversion(
                values => JsonSerializer.Serialize(values, (JsonSerializerOptions?)null),
                values => JsonSerializer.Deserialize<Dictionary<string, string>>(values, (JsonSerializerOptions?)null) ?? new Dictionary<string, string>())
            .Metadata.SetValueComparer(new ValueComparer<Dictionary<string, string>>(
                (left, right) => left != null && right != null && left.Count == right.Count && !left.Except(right).Any(),
                values => values.Aggregate(0, (hash, pair) => HashCode.Combine(hash, pair.Key.GetHashCode(), pair.Value.GetHashCode())),
                values => new Dictionary<string, string>(values)));
    }
}
