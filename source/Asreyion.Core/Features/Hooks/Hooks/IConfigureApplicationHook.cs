using Asreyion.Core.Features.Hooks.Interfaces;

namespace Asreyion.Core.Features.Hooks.Hooks;

public interface IConfigureApplicationHook : IHookSubscriber, IHookDescriptors
{
    void OnConfigureApplication(WebApplication app, IWebHostEnvironment env);
}