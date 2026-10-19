namespace Asreyion.Core.Mvc.Attributes;

/// <summary>
/// Associates a controller with the area and feature used for view lookup.
/// </summary>
/// <param name="areaName">The MVC area name.</param>
/// <param name="featureName">The feature folder name.</param>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public class FeatureAreaAttribute(string areaName, string featureName) : Attribute
{
    /// <summary>
    /// Gets the MVC area name.
    /// </summary>
    public string AreaName { get; } = areaName;
    /// <summary>
    /// Gets the feature folder name.
    /// </summary>
    public string FeatureName { get; } = featureName;
}
