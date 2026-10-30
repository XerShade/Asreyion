using Microsoft.EntityFrameworkCore;

namespace Asreyion.Core.Database.DbContexts;

/// <summary>
/// Database context for blog content.
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
