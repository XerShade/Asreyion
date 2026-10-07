using System.Text.RegularExpressions;
using Asreyion.Core.Features.Authentication.Data;
using Asreyion.Core.Features.Database.DbContexts;
using Asreyion.Modules.Blog.Administration.Models;
using Asreyion.Modules.Blog.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Asreyion.Modules.Blog.Administration.Controllers;

[Area("Administration"), Authorize(Roles = "Administrator")]
[Route("Administration/Blog")]
public sealed class BlogManagementController(DataDbContext db, UserManager<ApplicationUser> users, IWebHostEnvironment environment) : Controller
{
    private const long MaxBlogImageBytes = 10 * 1024 * 1024;
    private const long MaxBlogFileBytes = 25 * 1024 * 1024;

    [HttpPost("Images"), ValidateAntiForgeryToken]
    [RequestSizeLimit(MaxBlogImageBytes + 64 * 1024)]
    [RequestFormLimits(MultipartBodyLengthLimit = MaxBlogImageBytes + 64 * 1024)]
    public async Task<IActionResult> UploadImage(IFormFile? file, CancellationToken ct)
    {
        if (file is null || file.Length == 0) return this.BadRequest(new { error = "Choose an image to upload." });
        if (file.Length > MaxBlogImageBytes) return this.BadRequest(new { error = "Images must be 10 MB or smaller." });

        byte[] signature = new byte[12];
        await using (Stream input = file.OpenReadStream())
        {
            int read = 0;
            while (read < signature.Length)
            {
                int count = await input.ReadAsync(signature.AsMemory(read), ct);
                if (count == 0) break;
                read += count;
            }
            signature = signature[..read];
        }

        string? extension = GetBlogImageExtension(signature);
        if (extension is null) return this.BadRequest(new { error = "Use a PNG, JPEG, GIF, or WebP image." });

        string fileName = $"{Guid.NewGuid():N}{extension}";
        string imageDirectory = Path.Combine(environment.ContentRootPath, "store", "blog-images");
        Directory.CreateDirectory(imageDirectory);
        string imagePath = Path.Combine(imageDirectory, fileName);
        await using (Stream input = file.OpenReadStream())
        await using (FileStream output = new(imagePath, FileMode.CreateNew, FileAccess.Write, FileShare.None, 81920, useAsync: true))
        {
            await input.CopyToAsync(output, ct);
        }

        string imageUrl = $"{Request.PathBase}/Administration/Blog/Images/{fileName}";
        return this.Ok(new { url = imageUrl });
    }

    [HttpPost("Files"), ValidateAntiForgeryToken]
    [RequestSizeLimit(MaxBlogFileBytes + 64 * 1024)]
    [RequestFormLimits(MultipartBodyLengthLimit = MaxBlogFileBytes + 64 * 1024)]
    public async Task<IActionResult> UploadFile(IFormFile? file, CancellationToken ct)
    {
        if (file is null || file.Length == 0) return this.BadRequest(new { error = "Choose a file to attach." });
        if (file.Length > MaxBlogFileBytes) return this.BadRequest(new { error = "Attachments must be 25 MB or smaller." });

        string originalName = Path.GetFileName((file.FileName ?? "").Replace('\\', '/'));
        string extension = Path.GetExtension(originalName).ToLowerInvariant();
        if (!IsAllowedBlogFileExtension(extension)) return this.BadRequest(new { error = "This file type is not supported as a blog attachment." });

        string baseName = Regex.Replace(Path.GetFileNameWithoutExtension(originalName), @"[^A-Za-z0-9_-]+", "-").Trim('-', '_');
        if (string.IsNullOrWhiteSpace(baseName)) baseName = "attachment";
        if (baseName.Length > 80) baseName = baseName[..80];
        string downloadName = $"{baseName}{extension}";
        string storedName = $"{Guid.NewGuid():N}_{downloadName}";
        string fileDirectory = Path.Combine(environment.ContentRootPath, "store", "blog-files");
        Directory.CreateDirectory(fileDirectory);
        string filePath = Path.Combine(fileDirectory, storedName);
        await using (Stream input = file.OpenReadStream())
        await using (FileStream output = new(filePath, FileMode.CreateNew, FileAccess.Write, FileShare.None, 81920, useAsync: true))
        {
            await input.CopyToAsync(output, ct);
        }

        string fileUrl = $"{Request.PathBase}/Administration/Blog/Files/{storedName}";
        return this.Ok(new { url = fileUrl, fileName = downloadName });
    }

    [HttpGet("Files/{fileName}"), AllowAnonymous]
    public IActionResult GetBlogFile(string fileName)
    {
        if (!Regex.IsMatch(fileName, @"\A[a-f0-9]{32}_[A-Za-z0-9][A-Za-z0-9._-]{0,119}\z", RegexOptions.IgnoreCase)
            || !IsAllowedBlogFileExtension(Path.GetExtension(fileName))) return this.NotFound();

        string filePath = Path.Combine(environment.ContentRootPath, "store", "blog-files", fileName);
        if (!System.IO.File.Exists(filePath)) return this.NotFound();

        string downloadName = fileName[(fileName.IndexOf('_') + 1)..];
        Response.Headers["X-Content-Type-Options"] = "nosniff";
        Response.Headers["Cache-Control"] = "public,max-age=31536000,immutable";
        return this.PhysicalFile(filePath, "application/octet-stream", downloadName);
    }

    private static bool IsAllowedBlogFileExtension(string extension) => extension.ToLowerInvariant() is
        ".pdf" or ".zip" or ".7z" or ".rar" or ".csv" or ".json" or ".xml" or ".yaml" or ".yml" or ".toml" or ".ini" or ".config" or
        ".cs" or ".csx" or ".js" or ".mjs" or ".cjs" or ".ts" or ".tsx" or ".jsx" or ".py" or ".html" or ".htm" or ".css" or ".scss" or
        ".sql" or ".sh" or ".ps1" or ".go" or ".rs" or ".java" or ".c" or ".h" or ".cpp" or ".hpp" or ".php" or ".rb" or ".log" or
        ".txt" or ".md" or ".markdown" or ".docx" or ".xlsx" or ".pptx";

    [HttpGet("Images/{fileName}"), AllowAnonymous]
    public IActionResult GetBlogImage(string fileName)
    {
        if (!Regex.IsMatch(fileName, @"\A[a-f0-9]{32}\.(?:png|jpg|gif|webp)\z", RegexOptions.IgnoreCase)) return this.NotFound();

        string imagePath = Path.Combine(environment.ContentRootPath, "store", "blog-images", fileName);
        if (!System.IO.File.Exists(imagePath)) return this.NotFound();

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
        return this.PhysicalFile(imagePath, contentType);
    }

    private static string? GetBlogImageExtension(ReadOnlySpan<byte> signature)
    {
        if (signature.Length >= 8 && signature[..8].SequenceEqual(new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A })) return ".png";
        if (signature.Length >= 3 && signature[0] == 0xFF && signature[1] == 0xD8 && signature[2] == 0xFF) return ".jpg";
        if (signature.Length >= 6 && (signature[..6].SequenceEqual("GIF87a"u8) || signature[..6].SequenceEqual("GIF89a"u8))) return ".gif";
        if (signature.Length >= 12 && signature[..4].SequenceEqual("RIFF"u8) && signature[8..12].SequenceEqual("WEBP"u8)) return ".webp";
        return null;
    }

    [HttpGet("Posts")]
    public async Task<IActionResult> Posts(CancellationToken ct) => this.View(await db.Set<BlogPost>().AsNoTracking().Include(p => p.Author).OrderByDescending(p => p.Modified).ToListAsync(ct));

    [HttpGet("Posts/Create")]
    public async Task<IActionResult> CreatePost(CancellationToken ct) => this.View("PostForm", await PopulatePostOptions(new BlogPostInputModel(), ct));

    [HttpPost("Posts/Create"), ValidateAntiForgeryToken]
    public async Task<IActionResult> CreatePost(BlogPostInputModel model, CancellationToken ct)
    {
        if (!ModelState.IsValid) return this.View("PostForm", await PopulatePostOptions(model, ct));
        ApplicationUser? author = await users.GetUserAsync(User);
        if (author is null) return this.Forbid();
        BlogPost post = new() { Title = model.Title.Trim(), Body = model.Body, Slug = await UniqueSlug(model.Slug, model.Title, null, ct), AuthorId = author.Id, Created = DateTime.UtcNow, Modified = DateTime.UtcNow };
        await SetPostRelations(post, model, ct);
        db.Add(post); await db.SaveChangesAsync(ct);
        TempData["SuccessMessage"] = "The post was created.";
        return this.RedirectToAction(nameof(Posts));
    }

    [HttpGet("Posts/{id:int}/Edit")]
    public async Task<IActionResult> EditPost(int id, CancellationToken ct)
    {
        BlogPost? post = await db.Set<BlogPost>().AsNoTracking().Include(p => p.Tags).Include(p => p.Categories).FirstOrDefaultAsync(p => p.Id == id, ct);
        if (post is null) return this.NotFound();
        BlogPostInputModel model = new() { Id = post.Id, Title = post.Title, Body = post.Body, Slug = post.Slug, CategoryIds = post.Categories.Select(c => c.Id).ToList(), TagIds = post.Tags.Select(t => t.Id).ToList() };
        return this.View("PostForm", await PopulatePostOptions(model, ct));
    }

    [HttpPost("Posts/{id:int}/Edit"), ValidateAntiForgeryToken]
    public async Task<IActionResult> EditPost(int id, BlogPostInputModel model, CancellationToken ct)
    {
        if (id != model.Id) return this.BadRequest();
        BlogPost? post = await db.Set<BlogPost>().Include(p => p.Tags).Include(p => p.Categories).FirstOrDefaultAsync(p => p.Id == id, ct);
        if (post is null) return this.NotFound();
        if (!ModelState.IsValid) return this.View("PostForm", await PopulatePostOptions(model, ct));
        post.Title = model.Title.Trim(); post.Body = model.Body; post.Slug = await UniqueSlug(model.Slug, model.Title, id, ct); post.Modified = DateTime.UtcNow;
        await SetPostRelations(post, model, ct); await db.SaveChangesAsync(ct);
        TempData["SuccessMessage"] = "The post was updated.";
        return this.RedirectToAction(nameof(Posts));
    }

    [HttpPost("Posts/{id:int}/Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeletePost(int id, CancellationToken ct)
    {
        BlogPost? post = await db.Set<BlogPost>().FirstOrDefaultAsync(p => p.Id == id, ct);
        if (post is null) return this.NotFound();
        db.Remove(post); await db.SaveChangesAsync(ct);
        TempData["SuccessMessage"] = "The post was deleted.";
        return this.RedirectToAction(nameof(Posts));
    }

    [HttpGet("Categories")]
    public async Task<IActionResult> Categories(CancellationToken ct) => this.View(await db.Set<BlogCategory>().AsNoTracking().Include(c => c.Parent).Include(c => c.Posts).OrderBy(c => c.Name).ToListAsync(ct));

    [HttpGet("Categories/Create")]
    public async Task<IActionResult> CreateCategory(CancellationToken ct) => this.View("CategoryForm", await PopulateCategoryOptions(new BlogCategoryInputModel(), ct));

    [HttpPost("Categories/Create"), ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateCategory(BlogCategoryInputModel model, CancellationToken ct)
    {
        if (model.ParentId.HasValue && !await db.Set<BlogCategory>().AnyAsync(c => c.Id == model.ParentId, ct)) ModelState.AddModelError(nameof(model.ParentId), "Choose an existing parent category.");
        if (!ModelState.IsValid) return this.View("CategoryForm", await PopulateCategoryOptions(model, ct));
        BlogCategory category = new() { Name = model.Name.Trim(), Description = model.Description?.Trim() ?? "", Slug = await UniqueCategorySlug(model.Slug, model.Name, null, ct), ParentId = model.ParentId };
        db.Add(category); await db.SaveChangesAsync(ct);
        TempData["SuccessMessage"] = "The category was created.";
        return this.RedirectToAction(nameof(Categories));
    }

    [HttpGet("Categories/{id:int}/Edit")]
    public async Task<IActionResult> EditCategory(int id, CancellationToken ct)
    {
        BlogCategory? c = await db.Set<BlogCategory>().AsNoTracking().FirstOrDefaultAsync(c => c.Id == id, ct);
        if (c is null) return this.NotFound();
        return this.View("CategoryForm", await PopulateCategoryOptions(new BlogCategoryInputModel { Id = c.Id, Name = c.Name, Description = c.Description, Slug = c.Slug, ParentId = c.ParentId }, ct, id));
    }

    [HttpPost("Categories/{id:int}/Edit"), ValidateAntiForgeryToken]
    public async Task<IActionResult> EditCategory(int id, BlogCategoryInputModel model, CancellationToken ct)
    {
        if (id != model.Id) return this.BadRequest();
        BlogCategory? c = await db.Set<BlogCategory>().FirstOrDefaultAsync(c => c.Id == id, ct);
        if (c is null) return this.NotFound();
        if (model.ParentId == id || (model.ParentId.HasValue && await IsDescendant(id, model.ParentId.Value, ct))) ModelState.AddModelError(nameof(model.ParentId), "A category cannot be its own parent or a descendant.");
        if (model.ParentId.HasValue && !await db.Set<BlogCategory>().AnyAsync(x => x.Id == model.ParentId, ct)) ModelState.AddModelError(nameof(model.ParentId), "Choose an existing parent category.");
        if (!ModelState.IsValid) return this.View("CategoryForm", await PopulateCategoryOptions(model, ct, id));
        c.Name = model.Name.Trim(); c.Description = model.Description?.Trim() ?? ""; c.Slug = await UniqueCategorySlug(model.Slug, model.Name, id, ct); c.ParentId = model.ParentId;
        await db.SaveChangesAsync(ct);
        TempData["SuccessMessage"] = "The category was updated.";
        return this.RedirectToAction(nameof(Categories));
    }

    [HttpPost("Categories/{id:int}/Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteCategory(int id, CancellationToken ct)
    {
        BlogCategory? c = await db.Set<BlogCategory>().Include(c => c.Children).Include(c => c.Posts).FirstOrDefaultAsync(c => c.Id == id, ct);
        if (c is null) return this.NotFound();
        if (c.Children.Count > 0 || c.Posts.Count > 0) { TempData["ErrorMessage"] = "Move this category’s child categories and posts before deleting it."; return this.RedirectToAction(nameof(Categories)); }
        db.Remove(c); await db.SaveChangesAsync(ct);
        TempData["SuccessMessage"] = "The category was deleted.";
        return this.RedirectToAction(nameof(Categories));
    }

    [HttpGet("Tags")]
    public async Task<IActionResult> Tags(CancellationToken ct) => this.View(await db.Set<BlogTag>().AsNoTracking().Include(t => t.Posts).OrderBy(t => t.Name).ToListAsync(ct));

    [HttpGet("Tags/Create")]
    public IActionResult CreateTag() => this.View("TagForm", new BlogTagInputModel());

    [HttpPost("Tags/Create"), ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateTag(BlogTagInputModel model, CancellationToken ct)
    {
        string name = model.Name?.Trim() ?? "";
        if (name.Length > 0 && await db.Set<BlogTag>().AnyAsync(t => t.Name == name, ct)) ModelState.AddModelError(nameof(model.Name), "A tag with this name already exists.");
        if (!ModelState.IsValid) return this.View("TagForm", model);
        db.Add(new BlogTag { Name = name }); await db.SaveChangesAsync(ct);
        TempData["SuccessMessage"] = "The tag was created.";
        return this.RedirectToAction(nameof(Tags));
    }

    [HttpGet("Tags/{id:int}/Edit")]
    public async Task<IActionResult> EditTag(int id, CancellationToken ct)
    {
        BlogTag? t = await db.Set<BlogTag>().AsNoTracking().FirstOrDefaultAsync(t => t.Id == id, ct);
        return t is null ? this.NotFound() : this.View("TagForm", new BlogTagInputModel { Id = t.Id, Name = t.Name });
    }

    [HttpPost("Tags/{id:int}/Edit"), ValidateAntiForgeryToken]
    public async Task<IActionResult> EditTag(int id, BlogTagInputModel model, CancellationToken ct)
    {
        if (id != model.Id) return this.BadRequest();
        BlogTag? t = await db.Set<BlogTag>().FirstOrDefaultAsync(t => t.Id == id, ct);
        if (t is null) return this.NotFound();
        string name = model.Name?.Trim() ?? "";
        if (name.Length > 0 && await db.Set<BlogTag>().AnyAsync(x => x.Id != id && x.Name == name, ct)) ModelState.AddModelError(nameof(model.Name), "A tag with this name already exists.");
        if (!ModelState.IsValid) return this.View("TagForm", model);
        t.Name = name; await db.SaveChangesAsync(ct);
        TempData["SuccessMessage"] = "The tag was updated.";
        return this.RedirectToAction(nameof(Tags));
    }

    [HttpPost("Tags/{id:int}/Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteTag(int id, CancellationToken ct)
    {
        BlogTag? t = await db.Set<BlogTag>().Include(t => t.Posts).FirstOrDefaultAsync(t => t.Id == id, ct);
        if (t is null) return this.NotFound();
        if (t.Posts.Count > 0) { TempData["ErrorMessage"] = "Remove this tag from its posts before deleting it."; return this.RedirectToAction(nameof(Tags)); }
        db.Remove(t); await db.SaveChangesAsync(ct);
        TempData["SuccessMessage"] = "The tag was deleted.";
        return this.RedirectToAction(nameof(Tags));
    }

    private async Task<BlogPostInputModel> PopulatePostOptions(BlogPostInputModel m, CancellationToken ct)
    {
        m.Categories = await db.Set<BlogCategory>().AsNoTracking().OrderBy(c => c.Name).Select(c => new BlogCategoryOption(c.Id, c.Name)).ToListAsync(ct);
        m.Tags = await db.Set<BlogTag>().AsNoTracking().OrderBy(t => t.Name).Select(t => new BlogTagOption(t.Id, t.Name)).ToListAsync(ct);
        return m;
    }

    private async Task SetPostRelations(BlogPost p, BlogPostInputModel m, CancellationToken ct)
    {
        List<int> categories = m.CategoryIds.Distinct().ToList(); List<int> tags = m.TagIds.Distinct().ToList();
        p.Categories = await db.Set<BlogCategory>().Where(c => categories.Contains(c.Id)).ToListAsync(ct);
        p.Tags = await db.Set<BlogTag>().Where(t => tags.Contains(t.Id)).ToListAsync(ct);
    }

    private async Task<BlogCategoryInputModel> PopulateCategoryOptions(BlogCategoryInputModel m, CancellationToken ct, int? excludingId = null)
    {
        m.Categories = await db.Set<BlogCategory>().AsNoTracking().Where(c => !excludingId.HasValue || c.Id != excludingId).OrderBy(c => c.Name).Select(c => new BlogCategoryOption(c.Id, c.Name)).ToListAsync(ct);
        return m;
    }

    private async Task<bool> IsDescendant(int id, int candidateParent, CancellationToken ct)
    {
        int? current = candidateParent; HashSet<int> seen = [];
        while (current.HasValue && seen.Add(current.Value))
        {
            if (current.Value == id) return true;
            current = await db.Set<BlogCategory>().Where(c => c.Id == current.Value).Select(c => c.ParentId).FirstOrDefaultAsync(ct);
        }
        return false;
    }

    private async Task<string> UniqueSlug(string? requested, string fallback, int? except, CancellationToken ct)
    {
        string root = Slugify(string.IsNullOrWhiteSpace(requested) ? fallback : requested); string result = root; int i = 2;
        while (await db.Set<BlogPost>().AnyAsync(p => p.Slug == result && (!except.HasValue || p.Id != except), ct)) result = $"{root}-{i++}";
        return result;
    }

    private async Task<string> UniqueCategorySlug(string? requested, string fallback, int? except, CancellationToken ct)
    {
        string root = Slugify(string.IsNullOrWhiteSpace(requested) ? fallback : requested); string result = root; int i = 2;
        while (await db.Set<BlogCategory>().AnyAsync(c => c.Slug == result && (!except.HasValue || c.Id != except), ct)) result = $"{root}-{i++}";
        return result;
    }

    private static string Slugify(string value)
    {
        string slug = Regex.Replace(value.Trim().ToLowerInvariant(), @"[^a-z0-9]+", "-").Trim('-');
        return string.IsNullOrWhiteSpace(slug) ? "item" : slug;
    }
}
