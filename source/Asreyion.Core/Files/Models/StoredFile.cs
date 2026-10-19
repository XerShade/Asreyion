namespace Asreyion.Core.Files.Models;

/// <summary>
/// Describes a file stored by the upload service.
/// </summary>
/// <param name="Id">The opaque identifier assigned when the file was stored.</param>
/// <param name="FileName">The safe file name used in storage and downloads.</param>
/// <param name="ContentType">The content type reported for the file.</param>
/// <param name="Length">The file size in bytes.</param>
/// <param name="Url">The URL used to retrieve the stored file.</param>
/// <param name="IsImage">Whether the file was validated as a supported image.</param>
public sealed record StoredFile(string Id, string FileName, string ContentType, long Length, string Url, bool IsImage);
