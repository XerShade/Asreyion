using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Asreyion.Core.Features.Hooks.Extensions;
using Asreyion.Core.Features.Hooks.Interfaces;
using Asreyion.Core.Features.ControlPanels.Administration.Models;

namespace Asreyion.Core.Features.ControlPanels.Administration.Controllers;

/// <summary>
/// Displays administrative tools and registered hook information.
/// </summary>
[Area("Administration"), Authorize(Roles = "Administrator")]
public class DashboardController : Controller
{
    private readonly IEnumerable<RegisteredHook> registeredHooks;
    private readonly IServiceScopeFactory scopeFactory;

    /// <summary>
    /// Creates the dashboard with hook metadata and a scope factory.
    /// </summary>
    /// <param name="registeredHooks">Hook registrations discovered by the host.</param>
    /// <param name="scopeFactory">Creates scopes for reading hook descriptors.</param>
    public DashboardController(IEnumerable<RegisteredHook> registeredHooks, IServiceScopeFactory scopeFactory)
    {
        this.registeredHooks = registeredHooks;
        this.scopeFactory = scopeFactory;
    }

    /// <summary>
    /// Displays the administration dashboard.
    /// </summary>
    /// <returns>The dashboard view.</returns>
    [Route("Administration/Dashboard")]
    public IActionResult Index() 
        => this.View();

    /// <summary>
    /// Displays registered hooks and the contracts they provide.
    /// </summary>
    /// <returns>The hook status view.</returns>
    [Route("Administration/Hooks")]
    public IActionResult Hooks()
    {
        using IServiceScope scope = this.scopeFactory.CreateScope();
        List<HookStatusViewModel> hooks = [];

        foreach (IGrouping<Type, RegisteredHook> registration in this.registeredHooks
                     .GroupBy(hook => hook.ImplementationType)
                     .OrderBy(group => group.Key.FullName))
        {
            IHookDescriptors? descriptors = null;
            foreach (Type serviceType in registration.SelectMany(hook => hook.ServiceTypes)
                         .Distinct())
            {
                descriptors = scope.ServiceProvider.GetServices(serviceType)
                    .OfType<IHookDescriptors>()
                    .FirstOrDefault(instance => instance.GetType() == registration.Key);

                if (descriptors is not null)
                {
                    break;
                }
            }

            hooks.Add(new HookStatusViewModel(
                registration.Key.FullName ?? registration.Key.Name,
                descriptors?.Name ?? registration.Key.Name,
                descriptors?.Description,
                registration.SelectMany(hook => hook.ServiceTypes)
                    .Select(type => type.Name)
                    .Distinct()
                    .OrderBy(name => name)
                    .ToArray()));
        }

        return this.View(hooks);
    }
}
