using System.Text.RegularExpressions;
using Asreyion.Core.Features.Files.Models;
using Asreyion.Core.Features.Files.Services.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Asreyion.Core.Features.Files.Services;

/// <summary>
/// Validates supported upload formats and stores files beneath the application's content root.
/// </summary>
public sealed class FileUploadService(IWebHostEnvironment environment) : IFileUploadService
{
    private const long MaxImageBytes = 10 * 1024 * 1024;
    private const long MaxAttachmentBytes = 25 * 1024 * 1024;
    private const int CopyBufferSize = 81920;

    private static readonly HashSet<string> AllowedAttachmentExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".pdf", ".zip", ".7z", ".rar", ".csv", ".json", ".xml", ".yaml", ".yml", ".toml", ".ini", ".config",
        ".cs", ".csx", ".js", ".mjs", ".cjs", ".ts", ".tsx", ".jsx", ".py", ".html", ".htm", ".css", ".scss",
        ".sql", ".sh", ".ps1", ".go", ".rs", ".java", ".c", ".h", ".cpp", ".hpp", ".php", ".rb", ".log",
        ".txt", ".md", ".markdown", ".docx", ".xlsx", ".pptx"
    };

    /// <inheritdoc />
    public string GetImageUploadUrl(PathString pathBase) => $"{pathBase}/store/uploads/images";

    /// <inheritdoc />
    public string GetAttachmentUploadUrl(PathString pathBase) => $"{pathBase}/store/uploads/files";

    /// <inheritdoc />
    public string NormalizeLegacyUploadUrls(string content, PathString pathBase)
    {
        string normalized = content.Replace(
            "/Administration/Blog/Images/",
            $"{pathBase}/store/uploads/images/",
            StringComparison.OrdinalIgnoreCase);

        return normalized.Replace(
            "/Administration/Blog/Files/",
            $"{pathBase}/store/uploads/files/",
            StringComparison.OrdinalIgnoreCase);
    }

    /// <inheritdoc />
    public async Task<StoredFile?> UploadImageAsync(IFormFile file, PathString pathBase, CancellationToken cancellationToken)
    {
        if (!IsValidUpload(file, MaxImageBytes))
        {
            return null;
        }

        string? extension = await DetectImageExtensionAsync(file, cancellationToken);
        if (extension is null)
        {
            return null;
        }

        string id = Guid.NewGuid().ToString("N");
        string storedName = $"{id}{extension}";
        string path = GetStoragePath("images", storedName);
        await SaveFileAsync(file, path, cancellationToken);

        string contentType = extension switch
        {
            ".png" => "image/png",
            ".jpg" => "image/jpeg",
            ".gif" => "image/gif",
            ".webp" => "image/webp",
            _ => "application/octet-stream"
        };

        return new StoredFile(id, storedName, contentType, file.Length, BuildUrl(pathBase, "images", storedName), true);
    }

    /// <inheritdoc />
    public async Task<StoredFile?> UploadAttachmentAsync(IFormFile file, PathString pathBase, CancellationToken cancellationToken)
    {
        if (!IsValidUpload(file, MaxAttachmentBytes))
        {
            return null;
        }

        string? downloadName = CreateDownloadName(file.FileName);
        if (downloadName is null)
        {
            return null;
        }

        string id = Guid.NewGuid().ToString("N");
        string storedName = $"{id}_{downloadName}";
        string path = GetStoragePath("files", storedName);
        await SaveFileAsync(file, path, cancellationToken);

        return new StoredFile(id, downloadName, "application/octet-stream", file.Length, BuildUrl(pathBase, "files", storedName), false);
    }

    /// <inheritdoc />
    public string? GetImagePath(string fileName)
    {
        if (!Regex.IsMatch(fileName, @"\A[a-f0-9]{32}\.(?:png|jpg|gif|webp)\z", RegexOptions.IgnoreCase))
        {
            return null;
        }

        string path = GetStoragePath("images", fileName);
        return File.Exists(path) ? path : null;
    }

    /// <inheritdoc />
    public (string Path, string DownloadName)? GetAttachment(string fileName)
    {
        if (!Regex.IsMatch(fileName, @"\A[a-f0-9]{32}_[A-Za-z0-9][A-Za-z0-9._-]{0,119}\z", RegexOptions.IgnoreCase)
            || !AllowedAttachmentExtensions.Contains(Path.GetExtension(fileName)))
        {
            return null;
        }

        string path = GetStoragePath("files", fileName);
        if (!File.Exists(path))
        {
            return null;
        }

        return (path, fileName[(fileName.IndexOf('_') + 1)..]);
    }

    private static bool IsValidUpload(IFormFile file, long maxLength)
        => file is not null && file.Length > 0 && file.Length <= maxLength;

    private static async Task<string?> DetectImageExtensionAsync(IFormFile file, CancellationToken cancellationToken)
    {
        byte[] signature = new byte[12];
        await using Stream input = file.OpenReadStream();
        int read = 0;
        while (read < signature.Length)
        {
            int count = await input.ReadAsync(signature.AsMemory(read), cancellationToken);
            if (count == 0)
            {
                break;
            }

            read += count;
        }

        return GetImageExtension(signature, read);
    }

    private static string? GetImageExtension(byte[] signature, int length)
    {
        // Validate the bytes themselves; upload metadata and file names are supplied by the client.
        if (length >= 8 && signature.AsSpan(0, 8).SequenceEqual(new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A }))
        {
            return ".png";
        }

        if (length >= 3 && signature[0] == 0xFF && signature[1] == 0xD8 && signature[2] == 0xFF)
        {
            return ".jpg";
        }

        if (length >= 6 && (signature.AsSpan(0, 6).SequenceEqual("GIF87a"u8) || signature.AsSpan(0, 6).SequenceEqual("GIF89a"u8)))
        {
            return ".gif";
        }

        if (length >= 12 && signature.AsSpan(0, 4).SequenceEqual("RIFF"u8) && signature.AsSpan(8, 4).SequenceEqual("WEBP"u8))
        {
            return ".webp";
        }

        return null;
    }

    private static string? CreateDownloadName(string? originalName)
    {
        string safeName = Path.GetFileName((originalName ?? string.Empty).Replace('\\', '/'));
        string extension = Path.GetExtension(safeName).ToLowerInvariant();
        if (!AllowedAttachmentExtensions.Contains(extension))
        {
            return null;
        }

        string baseName = Regex.Replace(Path.GetFileNameWithoutExtension(safeName), @"[^A-Za-z0-9_-]+", "-").Trim('-', '_');
        if (string.IsNullOrWhiteSpace(baseName))
        {
            baseName = "attachment";
        }

        if (baseName.Length > 80)
        {
            baseName = baseName[..80];
        }

        return $"{baseName}{extension}";
    }

    private async Task SaveFileAsync(IFormFile file, string path, CancellationToken cancellationToken)
    {
        string? directory = Path.GetDirectoryName(path);
        if (directory is null)
        {
            throw new InvalidOperationException("The upload storage directory could not be resolved.");
        }

        Directory.CreateDirectory(directory);
        await using Stream input = file.OpenReadStream();
        // A generated storage name should never replace an existing upload.
        await using FileStream output = new(path, FileMode.CreateNew, FileAccess.Write, FileShare.None, CopyBufferSize, useAsync: true);
        await input.CopyToAsync(output, cancellationToken);
    }

    private string GetStoragePath(string category, string fileName)
        => Path.Combine(environment.ContentRootPath, "store", "uploads", category, fileName);

    private static string BuildUrl(PathString pathBase, string category, string fileName)
        => $"{pathBase}/store/uploads/{category}/{Uri.EscapeDataString(fileName)}";
}
