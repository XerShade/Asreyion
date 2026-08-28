namespace Asreyion.Core.Mvc.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public class FeatureAreaAttribute(string areaName, string featureName) : Attribute
{
    public string AreaName { get; } = areaName;
    public string FeatureName { get; } = featureName;
}