using Asreyion.Core.Modules.Interfaces;
using Microsoft.AspNetCore.HttpOverrides;

namespace Asreyion.Core.Modules;

public class ProxyConfigurationModule : ICoreModule
{
    /// <inheritdoc />
    public string Name => "Proxy Configuration";
    /// <inheritdoc />
    public string Description => "Configures the application to run behind a proxy service.";
    /// <inheritdoc />
    public Type[] Dependencies => [];

    /// <inheritdoc />
    public void OnConfigureApplication(WebApplication app, IWebHostEnvironment env)
    {
        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            _ = app.UseExceptionHandler("/Home/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            _ = app.UseHsts();
        }

        bool enableHttps = app.Configuration.GetValue<bool>("ENABLE_HTTPS");

        if (enableHttps)
        {
            _ = app.UseHttpsRedirection();
        }

        _ = app.UseForwardedHeaders();
    }

    /// <inheritdoc />
    public void OnConfigureServices(IServiceCollection services, IConfiguration configuration)
        // Configure the options to read Forwarded Headers.
        => services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            options.KnownIPNetworks.Clear();
            options.KnownProxies.Clear();
        });
}