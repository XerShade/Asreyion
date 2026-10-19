using Asreyion.Core.Features.Hooks.Hooks;

namespace Asreyion.Core.Features.Hooks.Extensions;

/// <summary>
/// Provides extension methods for executing application middleware and pipeline configuration hooks.
/// </summary>
public static class HookApplicationExtensions
{
    /// <summary>
    /// Resolves and executes all registered application configuration hooks to set up the request pipeline and middleware.
    /// </summary>
    /// <param name="application">The built <see cref="WebApplication"/> instance used to configure middleware and resolve services.</param>
    /// <returns>The same <see cref="WebApplication"/> instance for fluent chaining.</returns>
    public static WebApplication ConfigureApplicationWithHooks(this WebApplication application)
    {
        ArgumentNullException.ThrowIfNull(application);

        using IServiceScope scope = application.Services.CreateScope();

        IEnumerable<IConfigureApplicationHook> hooks = scope.ServiceProvider.GetServices<IConfigureApplicationHook>();

        foreach (IConfigureApplicationHook hook in hooks)
        {
            hook.OnConfigureApplication(application, application.Environment);
        }

        return application;
    }
}
