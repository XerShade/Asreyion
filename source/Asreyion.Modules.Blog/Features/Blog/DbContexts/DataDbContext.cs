using Asreyion.Modules.Blog.Features.Blog.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Asreyion.Core.Features.Database.DbContexts;

public partial class DataDbContext
{
    public DbSet<BlogPost> BlogPosts { get; set; } = default!;
    public DbSet<BlogCategory> BlogCategories { get; set; } = default!;
    public DbSet<BlogTag> BlogTags { get; set; } = default!;

    private class BlogPostConfiguration : IEntityTypeConfiguration<BlogPost>
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

    private class BlogTagConfiguration : IEntityTypeConfiguration<BlogTag>
    {
        public void Configure(EntityTypeBuilder<BlogTag> builder)
        {
            _ = builder.HasKey(e => e.Id);

            _ = builder.HasIndex(e => e.Name)
                .IsUnique();
        }
    }

    private class BlogCategoryConfiguration : IEntityTypeConfiguration<BlogCategory>
    {
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
}