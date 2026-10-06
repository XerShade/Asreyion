using Asreyion.Core.Features.Hooks.Interfaces;
using Microsoft.AspNetCore.Mvc.Razor;

namespace Asreyion.Core.Mvc.Hooks;

public interface IViewLocationPopulateHook : IHookSubscriber
{
    Task OnPopulateAsync(ViewLocationExpanderContext context);
}