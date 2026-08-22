using Asreyion.Modules.SimpleContent.Models;
using Asreyion.Modules.SimpleContent.Services.Interfaces;
using Asreyion.Modules.SimpleContent.Utilities;
using Markdig;
using System.Text;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Asreyion.Modules.SimpleContent.Services;

public class MarkdownContentProvider(IWebHostEnvironment environment) : IContentProvider
{
    /// <summary>
    /// The path to the content directory.
    /// </summary>
    protected string ContentPath { get; init; } = Path.Combine(environment.ContentRootPath, "Content", "Posts");

    protected readonly MarkdownPipeline Pipeline = new MarkdownPipelineBuilder()
        .UsePipeTables()
        .UseTaskLists()
        .UseAutoLinks()
        .UseEmphasisExtras()
        .UseEmojiAndSmiley()
        .Build();

    /// <inheritdoc />
    public IReadOnlyList<PostModel> GetPosts()
        => [.. Directory.GetFiles(this.ContentPath, "*.md")
        .Select(this.ParseFile)
        .OrderByDescending(p => p.Date)];

    /// <inheritdoc />
    public IReadOnlyList<string> GetCategories()
        => [.. this.GetPosts().Select(p => p.Category).Distinct().OrderBy(c => c)];

    /// <inheritdoc />
    public PostModel? GetPostBySlug(string slug)
        => this.GetPosts().FirstOrDefault(p => p.Slug == slug);

    /// <inheritdoc />
    public PostModel[] GetPostsByCategory(string category) 
        => [.. this.GetPosts().Where(p => SlugFormatter.ToCanonicalSlug(p.Category) == category)];

    /// <summary>
    /// Parses a markdown file into a post model.
    /// </summary>
    /// <param name="path">The path to the markdown file.</param>
    /// <returns>The post model.</returns>
    protected PostModel ParseFile(string path)
    {
        // Read the file contents into memory.
        string file = File.ReadAllText(path);

        // Split the file into meta data and body.
        (string metaText, string body) = SplitMetaData(file);

        // Map the meta data into a post meta model.
        PostMetaModel frontMatter = ParseYamlMeta(metaText);

        // Convert the body into HTML.
        string html = Markdown.ToHtml(body, this.Pipeline);

        // Return the post model.
        return new PostModel(
            frontMatter.Title,
            SlugFormatter.ToCanonicalSlug(frontMatter.Slug),
            frontMatter.Date,
            SlugFormatter.ToCanonicalSlug(frontMatter.PrimaryCategory ?? string.Empty),
            "XerShade",
            html
        );
    }

    /// <summary>
    /// Splits a markdown file into meta data and body.
    /// </summary>
    /// <param name="file">The markdown file.</param>
    /// <returns>The meta data and body.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the meta data is not found.</exception>
    private static (string meta, string body) SplitMetaData(string file)
    {
        // Create a reader for the file.
        using StringReader reader = new(file);

        // Check for the meta data start.
        if (reader.ReadLine()?.Trim() != "---")
        {
            // Throw an exception if the meta data is not found.
            throw new InvalidOperationException("Missing meta data start");
        }

        // Create a string builder for the meta data.
        StringBuilder meta = new();
        string? line;

        // Read the meta data until the end of the meta data.
        while ((line = reader.ReadLine()) != null)
        {
            // Check for the meta data end.
            if (line.Trim() == "---")
            {
                // Break out of the loop.
                break;
            }

            // Append the line to the meta data.
            _ = meta.AppendLine(line);
        }

        // Return the rest of the file as the body.
        string body = reader.ReadToEnd().TrimStart();

        // Return the meta data and body.
        return (meta.ToString(), body);
    }

    /// <summary>
    /// Parses a string of yaml into a post meta model.
    /// </summary>
    /// <param name="meta">The yaml string.</param>
    /// <returns>The post meta model.</returns>
    private static PostMetaModel ParseYamlMeta(string meta)
    {
        // Create a deserializer.
        IDeserializer deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .IgnoreUnmatchedProperties()
            .Build();

        // Deserialize the yaml into a post meta model.
        return deserializer.Deserialize<PostMetaModel>(meta).Validate();
    }
}
