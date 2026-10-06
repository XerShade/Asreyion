using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Asreyion.Core.Features.ControlPanels.Administration.Controllers;

[Area("Administration"), Authorize(Roles = "Administrator")]
public class DashboardController : Controller
{
    [Route("Administration/Dashboard")]
    public IActionResult Index() 
        => this.View();
}