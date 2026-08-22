using Asreyion.Modules.SimpleContent.Models;
using Asreyion.Modules.SimpleContent.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Asreyion.Modules.SimpleContent.ViewComponents;

public class RecentBlogPostsViewComponent(IContentProvider contentProvider) : ViewComponent
{
    private readonly IContentProvider ContentProvider = contentProvider;

    public IViewComponentResult Invoke(int count = 5)
    {
        List<PostModel> posts = [.. ContentProvider
            .GetPosts()
            .OrderByDescending(p => p.Date)
            .Take(count)];

        return this.View("Default", posts);
    }
}
