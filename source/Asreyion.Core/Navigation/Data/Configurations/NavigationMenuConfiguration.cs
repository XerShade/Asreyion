using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Asreyion.Core.Navigation.Data.Configurations;

/// <summary>
/// Preserves the legacy navigation table mapping for database compatibility.
/// </summary>
public class NavigationMenuConfiguration : IEntityTypeConfiguration<NavigationMenu>
{
    public void Configure(EntityTypeBuilder<NavigationMenu> builder)
    {
        builder.Property(menu => menu.Items)
            .HasColumnType("json");
    }
}
