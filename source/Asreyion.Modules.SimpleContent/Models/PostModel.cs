namespace Asreyion.Modules.SimpleContent.Models;

/// <summary>
/// Defines the model for a post.
/// </summary>
public sealed class PostModel(string title, string slug, DateTime date, string category, string author, string content)
{

    /// <summary>
    /// Gets the title of the post.
    /// </summary>
    public string Title { get; init; } = title ?? throw new ArgumentNullException(nameof(title));
    /// <summary>
    /// Gets the slug of the post.
    /// </summary>
    public string Slug { get; init; } = slug ?? throw new ArgumentNullException(nameof(slug));
    /// <summary>
    /// Gets the date of the post.
    /// </summary>
    public DateTime Date { get; init; } = date;
    /// <summary>
    /// Gets the category of the post.
    /// </summary>
    public string Category { get; init; } = category ?? throw new ArgumentNullException(nameof(category));
    /// <summary>
    /// Gets the author of the post.
    /// </summary>
    public string Author { get; init; } = author ?? throw new ArgumentNullException(nameof(author));
    /// <summary>
    /// Gets the content of the post.
    /// </summary>
    public string Content { get; init; } = content ?? throw new ArgumentNullException(nameof(content));
}
