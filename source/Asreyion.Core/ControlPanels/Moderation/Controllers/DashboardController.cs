using Microsoft.AspNetCore.Mvc;

namespace Asreyion.Core.ControlPanels.Moderation.Controllers;

/// <summary>
/// Displays the moderation dashboard.
/// </summary>
[Area("Moderation")]
public class DashboardController : Controller
{
    /// <summary>
    /// Loads the moderation dashboard.
    /// </summary>
    /// <returns>The moderation dashboard view.</returns>
    [Route("Moderation/Dashboard")]
    public async Task<IActionResult> Index() 
        => this.View();
}
