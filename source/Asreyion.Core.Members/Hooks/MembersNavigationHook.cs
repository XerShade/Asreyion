using Asreyion.Core.Hooks.Attributes;
using Asreyion.Core.Navigation.Builder;
using Asreyion.Core.Navigation.Hooks;

namespace Asreyion.Core.Members.Hooks;

[HookName("Members Navigation")]
[HookDescription("Adds member content links to the primary navigation menu.")]
public class MembersNavigationHook : IOnBuildNavigationHook
{
    public Task BuildNavigationAsync(INavigationBuilder builder)
    {
        NavigationItemDefinition commmunityParent = builder.AddOrGet("Community", item => item
            .WithIcon("users")
            .WithOrder(40));

        _ = commmunityParent.AddOrGetChild("Members", child => child
            .WithRoute("MemberList", "Index", "Members")
            .WithIcon("list-alt")
            .WithOrder(10));

        return Task.CompletedTask;
    }
}