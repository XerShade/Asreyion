using Asreyion.Core.Features.Files.Models;
using Microsoft.AspNetCore.Http;

namespace Asreyion.Core.Features.Files.Services.Interfaces;

public interface IFileUploadService
{
    string GetImageUploadUrl(PathString pathBase);

    string GetAttachmentUploadUrl(PathString pathBase);

    string NormalizeLegacyUploadUrls(string content, PathString pathBase);

    Task<StoredFile?> UploadImageAsync(IFormFile file, PathString pathBase, CancellationToken cancellationToken);

    Task<StoredFile?> UploadAttachmentAsync(IFormFile file, PathString pathBase, CancellationToken cancellationToken);

    string? GetImagePath(string fileName);

    (string Path, string DownloadName)? GetAttachment(string fileName);
}
