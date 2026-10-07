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
public sealed class BlogManagementController(DataDbContext db, UserManager<ApplicationUser> users) : Controller
{
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
