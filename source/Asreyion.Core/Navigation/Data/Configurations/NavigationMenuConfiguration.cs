using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Asreyion.Core.Navigation.Data.Configurations;

/// <summary>
/// Configures database storage for navigation menus.
/// </summary>
public class NavigationMenuConfiguration : IEntityTypeConfiguration<NavigationMenu>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<NavigationMenu> builder) 
        => builder.Property(e => e.Items)
            .HasColumnType("json");
}
