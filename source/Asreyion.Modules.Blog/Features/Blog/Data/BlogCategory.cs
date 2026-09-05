using System.ComponentModel.DataAnnotations;

namespace Asreyion.Modules.Blog.Features.Blog.Data;

public class BlogCategory
{
    [Key]
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;

    public int? ParentId { get; set; }
    public BlogCategory? Parent { get; set; }
    public List<BlogCategory> Children { get; set; } = [];
    public List<BlogPost> Posts { get; set; } = [];
}