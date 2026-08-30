using Asreyion.Core.Theme.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Asreyion.Server.Controllers;

public class HomeController : Controller
{
    public IActionResult Index() 
        => this.View();

    public IActionResult Privacy() 
        => this.View();

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() 
        => this.View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? this.HttpContext.TraceIdentifier });
}
