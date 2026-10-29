using Asreyion.Core.Hooks.Attributes;
using Asreyion.Core.Hooks.Interfaces;
using System.Reflection;

namespace Asreyion.Core.Hooks.Extensions;

public static class HookAttributeExtensions
{
    public static string GetHookName(this IHookSubscriber hook)
    {
        HookNameAttribute? attribute = hook.GetType().GetCustomAttribute<HookNameAttribute>();

        return attribute?.Name ?? hook.GetType().Name;
    }

    public static string GetHookDescription(this IHookSubscriber hook)
    {
        HookDescriptionAttribute? attribute = hook.GetType().GetCustomAttribute<HookDescriptionAttribute>();

        return attribute?.Description ?? "No description provided.";
    }

    public static IReadOnlyCollection<Type> GetHookDependencies(this IHookSubscriber hook)
    {
        HookDependenciesAttribute? attribute = hook.GetType().GetCustomAttribute<HookDependenciesAttribute>();

        return attribute?.Dependencies ?? [];
    }

    public static bool HasHookDependencies(this IHookSubscriber hook)
        => hook.GetHookDependencies().Count != 0;

    public static string GetHookName(Type hook)
    {
        HookNameAttribute? attribute = hook.GetCustomAttribute<HookNameAttribute>();

        return attribute?.Name ?? hook.Name;
    }

    public static string GetHookDescription(Type hook)
    {
        HookDescriptionAttribute? attribute = hook.GetCustomAttribute<HookDescriptionAttribute>();

        return attribute?.Description ?? "No description provided.";
    }

    public static IReadOnlyCollection<Type> GetHookDependencies(Type hook)
    {
        HookDependenciesAttribute? attribute = hook.GetCustomAttribute<HookDependenciesAttribute>();

        return attribute?.Dependencies ?? [];
    }

    public static bool HasHookDependencies(Type hook)
        => GetHookDependencies(hook).Count != 0;
}