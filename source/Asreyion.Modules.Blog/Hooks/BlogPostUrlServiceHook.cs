using Asreyion.Core.Hooks.Attributes;
using Asreyion.Core.Hooks.Hooks;
using Asreyion.Modules.Blog.Services;

namespace Asreyion.Modules.Blog.Hooks;

/// <summary>
/// Registers the service used to create category-aware blog URLs.
/// </summary>
[HookName("Blog Post URL Service")]
[HookDescription("Registers category-aware URL generation for blog posts.")]
public sealed class BlogPostUrlServiceHook : IConfigureServicesHook
{
    public void OnConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<BlogPostUrlService>();
    }
}
