using Asreyion.Modules.SimpleContent.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Asreyion.Modules.SimpleContent.ViewComponents;

public class BlogNavigationViewComponent(IContentProvider contentProvider) : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        IReadOnlyList<string> categories = contentProvider.GetCategories();

        return this.View(categories);
    }
}