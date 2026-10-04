using System.ComponentModel.DataAnnotations;

namespace Asreyion.Modules.Blog.Data;

public class BlogTag
{
    [Key]
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public List<BlogPost> Posts { get; set; } = [];
}