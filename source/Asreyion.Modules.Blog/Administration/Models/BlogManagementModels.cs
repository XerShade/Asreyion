using System.ComponentModel.DataAnnotations;

namespace Asreyion.Modules.Blog.Administration.Models;

public sealed class BlogPostInputModel
{
    public int Id { get; set; }
    [Required, StringLength(200)] public string Title { get; set; } = string.Empty;
    [Required] public string Body { get; set; } = string.Empty;
    [StringLength(200)] public string? Slug { get; set; }
    public List<int> CategoryIds { get; set; } = [];
    public List<int> TagIds { get; set; } = [];
    public IReadOnlyList<BlogCategoryOption> Categories { get; set; } = [];
    public IReadOnlyList<BlogTagOption> Tags { get; set; } = [];
}

public sealed record BlogCategoryOption(int Id, string Name);
public sealed record BlogTagOption(int Id, string Name);

public sealed class BlogCategoryInputModel
{
    public int Id { get; set; }
    [Required, StringLength(100)] public string Name { get; set; } = string.Empty;
    [StringLength(500)] public string Description { get; set; } = string.Empty;
    [StringLength(200)] public string? Slug { get; set; }
    public int? ParentId { get; set; }
    public IReadOnlyList<BlogCategoryOption> Categories { get; set; } = [];
}

public sealed class BlogTagInputModel
{
    public int Id { get; set; }
    [Required, StringLength(100)] public string Name { get; set; } = string.Empty;
}
