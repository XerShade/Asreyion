namespace Asreyion.Core.Features.Files.Models;

public sealed record StoredFile(string Id, string FileName, string ContentType, long Length, string Url, bool IsImage);
