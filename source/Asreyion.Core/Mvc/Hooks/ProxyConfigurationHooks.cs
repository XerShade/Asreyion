using Asreyion.Core.Hooks.Hooks;
using Microsoft.AspNetCore.HttpOverrides;

namespace Asreyion.Core.Mvc.Hooks;

/// <summary>
/// Configures forwarded headers and proxy-aware request handling.
/// </summary>
public class ProxyConfigurationHooks : IConfigureServicesHook, IConfigureApplicationHook
{
    /// <inheritdoc />
    public string Name => "Proxy Configuration";
    /// <inheritdoc />
    public string Description => "Configures the application to run behind a proxy service.";

    /// <inheritdoc />
    public void OnConfigureApplication(WebApplication app, IWebHostEnvironment env)
    {
        // Unpack reverse proxy headers right away.
        _ = app.UseForwardedHeaders();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            _ = app.UseExceptionHandler("/Home/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            _ = app.UseHsts();
        }

        // Check if HTTPS is enabled.
        bool enableHttps = app.Configuration.GetValue<bool>("ENABLE_HTTPS", true);

        // If HTTPS is enabled, redirect HTTP requests to HTTPS, and do other security measure configuration.
        if (enableHttps)
        {
            // Enable the HTTPS redirect middleware.
            _ = app.UseHttpsRedirection();
        }

        // Add security headers to prevent common vulnerabilities.
        _ = app.Use(async (context, next) =>
        {
            // Stops other sites from loading XerShade's Corner inside an iframe (Anti-Clickjacking)
            context.Response.Headers.Append("X-Frame-Options", "SAMEORIGIN");

            // Prevents browsers from sniffing MIME types away from what you explicitly declare
            context.Response.Headers.Append("X-Content-Type-Options", "nosniff");

            // Limits what referrer information is sent when a user clicks a link to leave your site
            context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");

            await next();
        });
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
