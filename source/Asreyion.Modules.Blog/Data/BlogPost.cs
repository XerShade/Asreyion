using Asreyion.Core.Authentication.Data;
using System.ComponentModel.DataAnnotations;

namespace Asreyion.Modules.Blog.Data;

/// <summary>
/// A published article and its author, tags, and categories.
/// </summary>
public class BlogPost
{
    /// <summary>
    /// Gets or sets the post's database identifier.
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the title shown for the post.
    /// </summary>
    public string Title { get; set; } = "Untitled Post";
    /// <summary>
    /// Gets or sets the post body, stored as HTML or Markdown content.
    /// </summary>
    public string Body { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the URL-friendly identifier for the post.
    /// </summary>
    public string Slug { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets when the post was created.
    /// </summary>
    public DateTime Created { get; set; } = DateTime.UtcNow;
    /// <summary>
    /// Gets or sets when the post was last changed.
    /// </summary>
    public DateTime Modified { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the identifier of the author.
    /// </summary>
    public string AuthorId { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the user who authored the post.
    /// </summary>
    public ApplicationUser Author { get; set; } = default!;
    /// <summary>
    /// Gets or sets the tags assigned to the post.
    /// </summary>
    public List<BlogTag> Tags { get; set; } = [];
    /// <summary>
    /// Gets or sets the categories assigned to the post.
    /// </summary>
    public List<BlogCategory> Categories { get; set; } = [];
}
