using Asreyion.Core.Features.Hooks.Interfaces;

namespace Asreyion.Core.Features.Hooks.Hooks;

public interface IConfigureServicesHook : IHookSubscriber, IHookDescriptors
{
    void OnConfigureServices(IServiceCollection services, IConfiguration configuration);
}