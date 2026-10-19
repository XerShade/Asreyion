using System.ComponentModel.DataAnnotations;

namespace Asreyion.Modules.Blog.Data;

/// <summary>
/// A label that can be applied to multiple blog posts.
/// </summary>
public class BlogTag
{
    /// <summary>
    /// Gets or sets the tag's database identifier.
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the tag name shown to readers.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the posts associated with this tag.
    /// </summary>
    public List<BlogPost> Posts { get; set; } = [];
}
