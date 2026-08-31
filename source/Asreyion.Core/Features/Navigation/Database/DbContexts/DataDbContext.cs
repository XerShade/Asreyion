using Asreyion.Core.Features.Navigation.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

// Must be set to the same namespace as the base DataDbContext
namespace Asreyion.Core.Features.Database.DbContexts;

public partial class DataDbContext
{
    public DbSet<NavigationMenu> NavigationMenus { get; set; }
    public DbSet<NavigationMenuItem> NavigationMenuItems { get; set; }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
    {
        _ = modelBuilder.Entity<NavigationMenu>()
            .Property(e => e.Items)
            .HasColumnType("json");

        _ = modelBuilder.Entity<NavigationMenuItem>()
            .Property(e => e.Children)
            .HasColumnType("json");

        _ = modelBuilder.Entity<NavigationMenuItem>()
          .Property(e => e.RouteValues)
          .HasColumnType("json")
          .HasConversion(
              v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
              v => JsonSerializer.Deserialize<Dictionary<string, string>>(v, (JsonSerializerOptions?)null) ?? new Dictionary<string, string>()
          );
    }
}