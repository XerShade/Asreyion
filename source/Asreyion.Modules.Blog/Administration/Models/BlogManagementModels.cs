using System.ComponentModel.DataAnnotations;

namespace Asreyion.Modules.Blog.Administration.Models;

/// <summary>
/// Form data for creating or editing a post and assigning its categories and tags.
/// </summary>
public sealed class BlogPostInputModel
{
    /// <summary>
    /// Gets or sets the post identifier, or zero when creating a post.
    /// </summary>
    public int Id { get; set; }
    /// <summary>
    /// Gets or sets the post title.
    /// </summary>
    [Required, StringLength(200)] public string Title { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the post body.
    /// </summary>
    [Required] public string Body { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets an optional requested URL slug.
    /// </summary>
    [StringLength(200)] public string? Slug { get; set; }
    /// <summary>
    /// Gets or sets the selected category identifiers.
    /// </summary>
    public List<int> CategoryIds { get; set; } = [];
    /// <summary>
    /// Gets or sets the selected tag identifiers.
    /// </summary>
    public List<int> TagIds { get; set; } = [];
    /// <summary>
    /// Gets or sets the available category choices shown by the form.
    /// </summary>
    public IReadOnlyList<BlogCategoryOption> Categories { get; set; } = [];
    /// <summary>
    /// Gets or sets the available tag choices shown by the form.
    /// </summary>
    public IReadOnlyList<BlogTagOption> Tags { get; set; } = [];
}

/// <summary>
/// A category choice displayed in the post and category forms.
/// </summary>
/// <param name="Id">The category identifier submitted by the form.</param>
/// <param name="Name">The category name shown to the user.</param>
public sealed record BlogCategoryOption(int Id, string Name);

/// <summary>
/// A tag choice displayed in the post form.
/// </summary>
/// <param name="Id">The tag identifier submitted by the form.</param>
/// <param name="Name">The tag name shown to the user.</param>
public sealed record BlogTagOption(int Id, string Name);

/// <summary>
/// Form data for creating or editing a category and its parent.
/// </summary>
public sealed class BlogCategoryInputModel
{
    /// <summary>
    /// Gets or sets the category identifier, or zero when creating a category.
    /// </summary>
    public int Id { get; set; }
    /// <summary>
    /// Gets or sets the category name.
    /// </summary>
    [Required, StringLength(100)] public string Name { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets an optional description.
    /// </summary>
    [StringLength(500)] public string? Description { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets an optional requested URL slug.
    /// </summary>
    [StringLength(200)] public string? Slug { get; set; }
    /// <summary>
    /// Gets or sets the parent category identifier, if this category is nested.
    /// </summary>
    public int? ParentId { get; set; }
    /// <summary>
    /// Gets or sets the categories available as a parent choice.
    /// </summary>
    public IReadOnlyList<BlogCategoryOption> Categories { get; set; } = [];
}

/// <summary>
/// Form data for creating or editing a tag.
/// </summary>
public sealed class BlogTagInputModel
{
    /// <summary>
    /// Gets or sets the tag identifier, or zero when creating a tag.
    /// </summary>
    public int Id { get; set; }
    /// <summary>
    /// Gets or sets the tag name.
    /// </summary>
    [Required, StringLength(100)] public string Name { get; set; } = string.Empty;
}
