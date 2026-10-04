using Asreyion.Core.Features.Authentication.Data;
using System.ComponentModel.DataAnnotations;

namespace Asreyion.Modules.Blog.Data;

public class BlogPost
{
    [Key]
    public int Id { get; set; }

    public string Title { get; set; } = "Untitled Post";
    public string Body { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public DateTime Created { get; set; } = DateTime.UtcNow;
    public DateTime Modified { get; set; } = DateTime.UtcNow;

    public string AuthorId { get; set; } = string.Empty;
    public ApplicationUser Author { get; set; } = default!;
    public List<BlogTag> Tags { get; set; } = [];
    public List<BlogCategory> Categories { get; set; } = [];
}