using Asreyion.Core.Features.Files.Services.Interfaces;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Asreyion.Modules.Blog.Administration;

/// <summary>
/// Normalizes legacy file URLs before rendering a blog post body.
/// </summary>
/// <param name="fileUploadService">Rewrites legacy upload links to current routes.</param>
[HtmlTargetElement("blog-post-content", Attributes = "html")]
public sealed class BlogPostContentTagHelper(IFileUploadService fileUploadService) : TagHelper
{
    /// <summary>
    /// Gets or sets the post body whose legacy file URLs should be updated.
    /// </summary>
    [HtmlAttributeName("html")]
    public string Html { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the current MVC view context.
    /// </summary>
    [ViewContext]
    [HtmlAttributeNotBound]
    public ViewContext ViewContext { get; set; } = default!;

    /// <inheritdoc />
    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        string content = fileUploadService.NormalizeLegacyUploadUrls(Html, ViewContext.HttpContext.Request.PathBase);

        output.TagName = null;
        output.Content.SetHtmlContent(content);
    }
}
