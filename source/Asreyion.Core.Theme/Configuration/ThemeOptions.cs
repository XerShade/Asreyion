namespace Asreyion.Core.Theme.Configuration;

/// <summary>
/// Site-specific values used by the shared theme.
/// </summary>
public sealed class ThemeOptions
{
    public string SiteName { get; set; } = "Asreyion";

    public string BrandTagline { get; set; } = string.Empty;

    public string CopyrightName { get; set; } = string.Empty;

    public string HomeUrl { get; set; } = "/";

    public List<ThemeLinkOptions> QuickLinks { get; set; } = [];

    public List<ThemeLinkOptions> LegalLinks { get; set; } = [];

    public List<ThemeLinkOptions> SocialLinks { get; set; } = [];
}

/// <summary>
/// A configurable link displayed by the theme.
/// </summary>
public sealed class ThemeLinkOptions
{
    public string Label { get; set; } = string.Empty;

    public string Url { get; set; } = string.Empty;

    public string Icon { get; set; } = string.Empty;
}
