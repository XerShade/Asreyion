using Microsoft.EntityFrameworkCore;

namespace Asreyion.Core.Features.Database.DbContexts;

/// <summary>
/// Database context for application content such as navigation and blog data.
/// </summary>
public class DataDbContext(DbContextOptions<DataDbContext> options) : DbContext(options)
{
    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        _ = modelBuilder.ApplyConfigurationsFromAssembly(typeof(DataDbContext).Assembly);
    }
}
