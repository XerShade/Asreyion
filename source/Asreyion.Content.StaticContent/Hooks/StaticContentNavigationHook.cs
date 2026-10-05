using Asreyion.Core.Features.Navigation.Builder;
using Asreyion.Core.Features.Navigation.Hooks;

namespace Asreyion.Content.StaticContent.Hooks;

/// <summary>
/// Registers static page navigation items (Home, About, Privacy, Terms) into navigation menus.
/// </summary>
public class StaticContentNavigationHook : IOnBuildNavigationHook
{
    public Task BuildNavigationAsync(INavigationBuilder builder)
    {
        if (!string.Equals(builder.MenuName, "Primary", StringComparison.OrdinalIgnoreCase))
        {
            return Task.CompletedTask;
        }

        // Add Home link
        _ = builder.AddOrGet("Home", item => item
            .WithRoute("Home", "Index")
            .WithIcon("house")
            .WithOrder(10));

        // Add or get "About" parent container
        NavigationItemDefinition aboutParent = builder.AddOrGet("About", item => item
            .WithIcon("circle-info")
            .WithOrder(50));

        // Add Privacy Policy under "About"
        aboutParent.AddOrGetChild("Privacy Policy", child => child
            .WithRoute("Home", "Privacy")
            .WithIcon("shield-halved")
            .WithOrder(10));

        // Add Terms of Service under "About"
        aboutParent.AddOrGetChild("Terms of Service", child => child
            .WithRoute("Home", "Terms")
            .WithIcon("file-contract")
            .WithOrder(20));

        return Task.CompletedTask;
    }
}

