using Asreyion.Modules.SimpleContent.Models;

namespace Asreyion.Modules.SimpleContent.Services.Interfaces;

/// <summary>
/// Defines a contract for a content provider.
/// </summary>
public interface IContentProvider
{
    /// <summary>
    /// Retrieves a read-only list of posts.
    /// </summary>
    /// <returns>A read-only list of posts.</returns>
    IReadOnlyList<PostModel> GetPosts();
    /// <summary>
    /// Retrieves a read-only list of categories.
    /// </summary>
    /// <returns>A read-only list of categories.</returns>
    IReadOnlyList<string> GetCategories();
    /// <summary>
    /// Retrieves a single post by its slug.
    /// </summary>
    /// <param name="slug">The slug of the post to retrieve.</param>
    /// <returns>The post with the specified slug, or null if not found.</returns>
    PostModel? GetPostBySlug(string slug);
    /// <summary>
    /// Retrieves a list of posts for a specific category.
    /// </summary>
    /// <param name="category">The category to retrieve posts for.</param>
    /// <returns>A collection of posts for the specified category.</returns>
    PostModel[] GetPostsByCategory(string category);
}
