using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Asreyion.Core.Hooks.Extensions;
using Asreyion.Core.Hooks.Interfaces;
using Asreyion.Core.ControlPanels.Administration.Models;
using Asreyion.Core.Hooks.Attributes;

namespace Asreyion.Core.ControlPanels.Administration.Controllers;

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
    [Route("administration/dashboard")]
    public IActionResult Index() 
        => this.View();

    /// <summary>
    /// Displays registered hooks and the contracts they provide.
    /// </summary>
    /// <returns>The hook status view.</returns>
    [Route("administration/hooks")]
    public IActionResult Hooks()
    {
        using IServiceScope scope = this.scopeFactory.CreateScope();
        List<HookStatusViewModel> hooks = [];

        foreach (IGrouping<Type, RegisteredHook> registration in this.registeredHooks
                     .GroupBy(hook => hook.ImplementationType)
                     .OrderBy(group => group.Key.FullName))
        {
            hooks.Add(new HookStatusViewModel(
                registration.Key.FullName ?? registration.Key.Name,
                HookAttributeExtensions.GetHookName(registration.Key) ?? registration.Key.Name,
                HookAttributeExtensions.GetHookDescription(registration.Key),
                HookAttributeExtensions.GetHookDependencies(registration.Key),
                [.. registration.SelectMany(hook => hook.ServiceTypes)
                    .Select(type => type.Name)
                    .Distinct()
                    .OrderBy(name => name)]));
        }

        return this.View(hooks);
    }
}
