using Asreyion.Core.Files.Models;
using Microsoft.AspNetCore.Http;

namespace Asreyion.Core.Files.Services.Interfaces;

/// <summary>
/// Stores supported uploads and resolves stored files for download or display.
/// </summary>
public interface IFileUploadService
{
    /// <summary>
    /// Builds the image upload endpoint URL for the current application path base.
    /// </summary>
    /// <param name="pathBase">The path prefix used when the application is hosted beneath a subpath.</param>
    /// <returns>The image upload endpoint URL.</returns>
    string GetImageUploadUrl(PathString pathBase);

    /// <summary>
    /// Builds the attachment upload endpoint URL for the current application path base.
    /// </summary>
    /// <param name="pathBase">The path prefix used when the application is hosted beneath a subpath.</param>
    /// <returns>The attachment upload endpoint URL.</returns>
    string GetAttachmentUploadUrl(PathString pathBase);

    /// <summary>
    /// Rewrites legacy blog upload links to the current storage routes.
    /// </summary>
    /// <param name="content">The content that may contain legacy upload URLs.</param>
    /// <param name="pathBase">The path prefix used by the current host.</param>
    /// <returns>The content with legacy upload links normalized.</returns>
    string NormalizeLegacyUploadUrls(string content, PathString pathBase);

    /// <summary>
    /// Validates and stores an image upload.
    /// </summary>
    /// <param name="file">The uploaded image.</param>
    /// <param name="pathBase">The path prefix used to build its public URL.</param>
    /// <param name="cancellationToken">Cancels validation or file writing.</param>
    /// <returns>The stored file details, or <see langword="null"/> when the upload is unsupported.</returns>
    Task<StoredFile?> UploadImageAsync(IFormFile file, PathString pathBase, CancellationToken cancellationToken);

    /// <summary>
    /// Validates and stores an allowed attachment upload.
    /// </summary>
    /// <param name="file">The uploaded attachment.</param>
    /// <param name="pathBase">The path prefix used to build its public URL.</param>
    /// <param name="cancellationToken">Cancels validation or file writing.</param>
    /// <returns>The stored file details, or <see langword="null"/> when the upload is unsupported.</returns>
    Task<StoredFile?> UploadAttachmentAsync(IFormFile file, PathString pathBase, CancellationToken cancellationToken);

    /// <summary>
    /// Resolves a stored image name to its path when it is a valid image upload.
    /// </summary>
    /// <param name="fileName">The image's stored file name.</param>
    /// <returns>The path to an existing image, or <see langword="null"/> when it cannot be resolved.</returns>
    string? GetImagePath(string fileName);

    /// <summary>
    /// Resolves a stored attachment name and its safe download name.
    /// </summary>
    /// <param name="fileName">The attachment's stored file name.</param>
    /// <returns>The stored path and download name, or <see langword="null"/> when it cannot be resolved.</returns>
    (string Path, string DownloadName)? GetAttachment(string fileName);
}
