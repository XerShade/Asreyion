using Asreyion.Core.Features.Navigation.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.Json;

namespace Asreyion.Core.Features.Database.DbContexts;

public partial class DataDbContext
{
    public DbSet<NavigationMenu> NavigationMenus { get; set; }
    public DbSet<NavigationMenuItem> NavigationMenuItems { get; set; }

    private class NavigationMenuConfiguration : IEntityTypeConfiguration<NavigationMenu>
    {
        public void Configure(EntityTypeBuilder<NavigationMenu> builder)
            => builder.Property(e => e.Items)
                .HasColumnType("json");
    }

    private class NavigationMenuItemConfiguration : IEntityTypeConfiguration<NavigationMenuItem>
    {
        public void Configure(EntityTypeBuilder<NavigationMenuItem> builder)
        {
            _ = builder.Property(e => e.Children)
                .HasColumnType("json");

            _ = builder.Property(e => e.RouteValues)
                .HasColumnType("json")
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                    v => JsonSerializer.Deserialize<Dictionary<string, string>>(v, (JsonSerializerOptions?)null) ?? new Dictionary<string, string>()
                );
        }
    }
}