using System.ComponentModel.DataAnnotations;

namespace Asreyion.Modules.Blog.Data;

/// <summary>
/// A category used to organize blog posts in a parent and child hierarchy.
/// </summary>
public class BlogCategory
{
    /// <summary>
    /// Gets or sets the category's database identifier.
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the category name shown to readers.
    /// </summary>
    public string Name { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the optional description shown with the category.
    /// </summary>
    public string Description { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the URL-friendly identifier for the category.
    /// </summary>
    public string Slug { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the parent category identifier, when this category is nested.
    /// </summary>
    public int? ParentId { get; set; }
    /// <summary>
    /// Gets or sets the parent category.
    /// </summary>
    public BlogCategory? Parent { get; set; }
    /// <summary>
    /// Gets or sets the categories nested under this category.
    /// </summary>
    public List<BlogCategory> Children { get; set; } = [];
    /// <summary>
    /// Gets or sets the posts assigned to this category.
    /// </summary>
    public List<BlogPost> Posts { get; set; } = [];
}
