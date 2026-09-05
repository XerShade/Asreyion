using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Asreyion.Core.Areas.Administration.Controllers;

[Area("Administration"), Authorize(Roles = "Administrator")]
public class DashboardController : Controller
{
    public IActionResult Index() 
        => this.View();
}