using Microsoft.EntityFrameworkCore;

namespace Asreyion.Core.Features.Database.DbContexts;

public class DataDbContext(DbContextOptions<DataDbContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        _ = modelBuilder.ApplyConfigurationsFromAssembly(typeof(DataDbContext).Assembly);
    }
}