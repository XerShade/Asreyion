using Asreyion.Core.Features.Files.Services.Interfaces;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Asreyion.Modules.Blog.Administration;

[HtmlTargetElement("blog-post-content", Attributes = "html")]
public sealed class BlogPostContentTagHelper(IFileUploadService fileUploadService) : TagHelper
{
    [HtmlAttributeName("html")]
    public string Html { get; set; } = string.Empty;

    [ViewContext]
    [HtmlAttributeNotBound]
    public ViewContext ViewContext { get; set; } = default!;

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        string content = fileUploadService.NormalizeLegacyUploadUrls(Html, ViewContext.HttpContext.Request.PathBase);

        output.TagName = null;
        output.Content.SetHtmlContent(content);
    }
}
