using Asreyion.Core.Hooks.Interfaces;

namespace Asreyion.Core.Hooks.Hooks;

/// <summary>
/// Configures the application's middleware and request pipeline after service registration.
/// </summary>
public interface IConfigureApplicationHook : IHookSubscriber
{
    /// <summary>
    /// Adds middleware and other application components to the request pipeline.
    /// </summary>
    /// <param name="app">The built web application.</param>
    /// <param name="env">The hosting environment.</param>
    void OnConfigureApplication(WebApplication app, IWebHostEnvironment env);
}
