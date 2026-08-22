namespace Asreyion.Modules.SimpleContent.Models;

/// <summary>
/// Defines the model for a post's metadata.
/// </summary>
public class PostMetaModel
{
    /// <summary>
    /// Gets the title of the post.
    /// </summary>
    public string Title { get; set; } = string.Empty;
    /// <summary>
    /// Gets the slug of the post.
    /// </summary>
    public string Slug { get; set; } = string.Empty;
    /// <summary>
    /// Gets the date of the post.
    /// </summary>
    public DateTime Date { get; set; } = DateTime.Now;
    /// <summary>
    /// Gets the categories of the post.
    /// </summary>
    public List<string> Categories { get; set; } = [];
    /// <summary>
    /// Gets the tags of the post.
    /// </summary>
    public List<string> Tags { get; set; } = []; 
    /// <summary>
    /// Gets the primary category of the post.
    /// </summary>
    public string PrimaryCategory
        => this.Categories.FirstOrDefault() ?? "";

    /// <summary>
    /// Validates the model and returns it.
    /// </summary>
    /// <returns>The validated model.</returns>
    public PostMetaModel Validate()
    {
        // Check if the title is empty.
        if (string.IsNullOrWhiteSpace(this.Title))
        {
            // If it is, set the title to "Untitled Post".
            this.Title = "Untitled Post";
        }

        // Check if the slug is empty.
        if (string.IsNullOrWhiteSpace(this.Slug))
        {
            // If it is, set the slug to the title.
            this.Slug = this.Title
                .ToLower()
                .Replace(" ", "-")
                .Replace("#", "sharp")
                .Replace(":", "colon")
                .Replace("?", "question")
                .Replace("&", "and")
                .Replace("%", "percent")
                .Replace("$", "dollar")
                .Replace("@", "at")
                .Replace("!", "exclamation")
                .Replace("+", "plus")
                .Replace("/", "slash")
                .Replace("\\", "backslash")
                .Replace("=", "equals")
                .Replace("|", "pipe");
        }

        // Check if the categories are empty.
        if (this.Categories.Count == 0)
        {
            // If they are, add "Uncategorized".
            this.Categories.Add("Uncategorized");
        }

        // Return the model.
        return this;
    }
}
