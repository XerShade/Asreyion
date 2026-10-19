using Asreyion.Core.Features.Files.Services.Interfaces;
using Asreyion.Core.Features.Files.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Asreyion.Core.Features.Files.Controllers;

/// <summary>
/// Accepts administrator uploads and serves stored files.
/// </summary>
[ApiController]
[Authorize(Roles = "Administrator")]
[Route("store/uploads")]
public sealed class FileUploadController(IFileUploadService fileUploadService) : ControllerBase
{
    private const long MaxImageBytes = 10 * 1024 * 1024;
    private const long MaxAttachmentBytes = 25 * 1024 * 1024;

    /// <summary>
    /// Validates and stores a supported image upload.
    /// </summary>
    /// <param name="file">The image submitted by the administrator.</param>
    /// <param name="cancellationToken">Cancels the upload operation.</param>
    /// <returns>The image URL, or a validation response when the upload is unsupported.</returns>
    [HttpPost("images")]
    [ValidateAntiForgeryToken]
    [RequestSizeLimit(MaxImageBytes + 64 * 1024)]
    [RequestFormLimits(MultipartBodyLengthLimit = MaxImageBytes + 64 * 1024)]
    public async Task<IActionResult> UploadImage(IFormFile? file, CancellationToken cancellationToken)
    {
        if (file is null || file.Length == 0)
        {
            return this.BadRequest(new { error = "Choose an image to upload." });
        }

        if (file.Length > MaxImageBytes)
        {
            return this.BadRequest(new { error = "Images must be 10 MB or smaller." });
        }

        StoredFile? storedFile = await fileUploadService.UploadImageAsync(file, Request.PathBase, cancellationToken);
        if (storedFile is null)
        {
            return this.BadRequest(new { error = "Use a PNG, JPEG, GIF, or WebP image." });
        }

        return this.Ok(new { url = storedFile.Url });
    }

    /// <summary>
    /// Validates and stores an allowed attachment.
    /// </summary>
    /// <param name="file">The attachment submitted by the administrator.</param>
    /// <param name="cancellationToken">Cancels the upload operation.</param>
    /// <returns>The attachment URL and download name, or a validation response.</returns>
    [HttpPost("files")]
    [ValidateAntiForgeryToken]
    [RequestSizeLimit(MaxAttachmentBytes + 64 * 1024)]
    [RequestFormLimits(MultipartBodyLengthLimit = MaxAttachmentBytes + 64 * 1024)]
    public async Task<IActionResult> UploadFile(IFormFile? file, CancellationToken cancellationToken)
    {
        if (file is null || file.Length == 0)
        {
            return this.BadRequest(new { error = "Choose a file to attach." });
        }

        if (file.Length > MaxAttachmentBytes)
        {
            return this.BadRequest(new { error = "Attachments must be 25 MB or smaller." });
        }

        StoredFile? storedFile = await fileUploadService.UploadAttachmentAsync(file, Request.PathBase, cancellationToken);
        if (storedFile is null)
        {
            return this.BadRequest(new { error = "This file type is not supported as an attachment." });
        }

        return this.Ok(new { url = storedFile.Url, fileName = storedFile.FileName });
    }

    /// <summary>
    /// Downloads an allowed stored attachment.
    /// </summary>
    /// <param name="fileName">The stored attachment name.</param>
    /// <returns>The attachment download, or a not-found response.</returns>
    [HttpGet("files/{fileName}")]
    [AllowAnonymous]
    public IActionResult DownloadFile(string fileName)
    {
        (string Path, string DownloadName)? attachment = fileUploadService.GetAttachment(fileName);
        if (attachment is null)
        {
            return this.NotFound();
        }

        Response.Headers["X-Content-Type-Options"] = "nosniff";
        Response.Headers["Cache-Control"] = "public,max-age=31536000,immutable";
        return this.PhysicalFile(attachment.Value.Path, "application/octet-stream", attachment.Value.DownloadName);
    }

    /// <summary>
    /// Returns a supported stored image.
    /// </summary>
    /// <param name="fileName">The stored image name.</param>
    /// <returns>The image response, or a not-found response.</returns>
    [HttpGet("images/{fileName}")]
    [AllowAnonymous]
    public IActionResult GetImage(string fileName)
    {
        string? path = fileUploadService.GetImagePath(fileName);
        if (path is null)
        {
            return this.NotFound();
        }

        string contentType = Path.GetExtension(fileName).ToLowerInvariant() switch
        {
            ".png" => "image/png",
            ".jpg" => "image/jpeg",
            ".gif" => "image/gif",
            ".webp" => "image/webp",
            _ => "application/octet-stream"
        };

        Response.Headers["X-Content-Type-Options"] = "nosniff";
        Response.Headers["Cache-Control"] = "public,max-age=31536000,immutable";
        return this.PhysicalFile(path, contentType);
    }
}
