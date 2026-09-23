using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Asreyion.Modules.Blog.Features.Blog.Data.Configurations;

public class BlogPostConfiguration : IEntityTypeConfiguration<BlogPost>
{
    public void Configure(EntityTypeBuilder<BlogPost> builder)
    {
        _ = builder.HasKey(e => e.Id);

        _ = builder.HasIndex(e => e.Slug)
            .IsUnique();

        _ = builder.HasOne(e => e.Author)
            .WithMany()
            .HasForeignKey(e => e.AuthorId)
            .OnDelete(DeleteBehavior.Restrict);

        _ = builder.HasMany(e => e.Tags)
            .WithMany(e => e.Posts);

        _ = builder.HasMany(e => e.Categories)
            .WithMany(e => e.Posts);
    }
}