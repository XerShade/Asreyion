using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Asreyion.Modules.Blog.Data.Configurations;

/// <summary>
/// Configures database constraints and relationships for blog tags.
/// </summary>
public class BlogTagConfiguration : IEntityTypeConfiguration<BlogTag>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<BlogTag> builder)
    {
        _ = builder.HasKey(e => e.Id);

        _ = builder.HasIndex(e => e.Name)
            .IsUnique();
    }
}
