using Microsoft.AspNetCore.Mvc;

namespace Asreyion.Core.Areas.Moderation.Controllers;

[Area("Moderation")]
public class DashboardController : Controller
{
    [Route("Moderation/Dashboard")]
    public async Task<IActionResult> Index() 
        => this.View();
}