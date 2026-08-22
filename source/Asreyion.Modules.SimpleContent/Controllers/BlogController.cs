using Asreyion.Modules.SimpleContent.Models;
using Asreyion.Modules.SimpleContent.Services.Interfaces;
using Asreyion.Modules.SimpleContent.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace Asreyion.Modules.SimpleContent.Controllers;

[Route("Blog")]
public class BlogController(IContentProvider contentProvider) : Controller
{
    protected IContentProvider ContentProvider { get; init; } = contentProvider;

    [HttpGet("")]
    public IActionResult Index()
        => this.View("BlogList", this.ContentProvider.GetPosts());

    [HttpGet("Category/{categorySlug}")]
    public IActionResult Category(string categorySlug)
    {
        PostModel[] posts = this.ContentProvider.GetPostsByCategory(categorySlug);

        if (posts.Length == 0)
        {
            return this.NotFound();
        }

        this.ViewData["Category"] = SlugFormatter.ToTitle(categorySlug);
        return this.View("BlogList", posts);
    }

    [HttpGet("{slug}")]
    public IActionResult Post(string slug)
    {
        PostModel? post = this.ContentProvider.GetPostBySlug(slug);
        return post == null ? this.NotFound() : this.View("BlogPost", post);
    }
}
