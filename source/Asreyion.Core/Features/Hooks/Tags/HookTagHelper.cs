using Asreyion.Core.Features.Hooks.Interfaces;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Asreyion.Core.Features.Hooks.Tags;

/// <summary>
/// Defines a html hook point tag for use in cshtml files.
/// </summary>
/// <param name="hookEngine">The hook engine to use.</param>
/// <remarks>This tag is intended to be used inside of cshtml files to cleanly inject content.</remarks>
[HtmlTargetElement("hook", Attributes = "name", TagStructure = TagStructure.WithoutEndTag)]
public class HookTagHelper(IHookEngine hookEngine) : TagHelper
{
    /// <summary>
    /// Gets a reference to the hook engine.
    /// </summary>
    private IHookEngine HookEngine { get; } = hookEngine;

    /// <summary>
    /// The name of the hook to render.
    /// </summary>
    [HtmlAttributeName("name")]
    public string Name { get; set; } = string.Empty;

    /// <inheritdoc />
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        // Don't render the <hook-point> tag itself, just its contents.
        output.TagName = null;
        output.TagMode = TagMode.SelfClosing;

        // Process the hook.
        List<string> htmlSnippets = await this.HookEngine.RenderHtmlPointsAsync(this.Name);

        // Render the snippets.
        foreach (string snippet in htmlSnippets)
        {
            // Append the snippet.
            _ = output.PostContent.AppendHtml(snippet);
        }
    }
}