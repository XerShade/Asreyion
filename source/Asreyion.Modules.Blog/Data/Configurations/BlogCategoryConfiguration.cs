using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Asreyion.Modules.Blog.Data.Configurations;

/// <summary>
/// Configures database constraints and relationships for blog categories.
/// </summary>
public class BlogCategoryConfiguration : IEntityTypeConfiguration<BlogCategory>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<BlogCategory> builder)
    {
        _ = builder.HasKey(e => e.Id);

        _ = builder.HasIndex(e => e.Slug)
            .IsUnique();

        _ = builder.HasOne(e => e.Parent)
            .WithMany(e => e.Children)
            .HasForeignKey(e => e.ParentId)
            .OnDelete(DeleteBehavior.Restrict);

        _ = builder.HasMany(e => e.Posts)
            .WithMany(e => e.Categories);
    }
}
