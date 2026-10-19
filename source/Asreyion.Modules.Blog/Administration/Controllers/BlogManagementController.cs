using System.Text.RegularExpressions;
using Asreyion.Core.Authentication.Data;
using Asreyion.Core.Database.DbContexts;
using Asreyion.Core.Files.Services.Interfaces;
using Asreyion.Modules.Blog.Administration.Models;
using Asreyion.Modules.Blog.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Asreyion.Modules.Blog.Administration.Controllers;

/// <summary>
/// Creates, updates, and removes blog posts, categories, and tags.
/// </summary>
/// <param name="db">Reads and writes blog content.</param>
/// <param name="users">Resolves the administrator creating a post.</param>
/// <param name="fileUploadService">Builds upload URLs and normalizes stored links.</param>
[Area("Administration"), Authorize(Roles = "Administrator")]
[Route("Administration/Blog")]
public sealed class BlogManagementController(DataDbContext db, UserManager<ApplicationUser> users, IFileUploadService fileUploadService) : Controller
{
    /// <summary>
    /// Lists the blog posts available to administrators.
    /// </summary>
    /// <param name="ct">Cancels the database query.</param>
    /// <returns>The post administration view.</returns>
    [HttpGet("Posts")]
    public async Task<IActionResult> Posts(CancellationToken ct)
    {
        List<BlogPost> posts = await db.Set<BlogPost>()
            .AsNoTracking()
            .Include(post => post.Author)
            .OrderByDescending(post => post.Modified)
            .ToListAsync(ct);

        return this.View(posts);
    }

    /// <summary>
    /// Displays a blank post form with available categories and tags.
    /// </summary>
    /// <param name="ct">Cancels the option queries.</param>
    /// <returns>The post form.</returns>
    [HttpGet("Posts/Create")]
    public async Task<IActionResult> CreatePost(CancellationToken ct)
    {
        SetUploadUrls();
        return this.View("PostForm", await PopulatePostOptions(new BlogPostInputModel(), ct));
    }

    /// <summary>
    /// Creates a post from the submitted form.
    /// </summary>
    /// <param name="model">The submitted post details and taxonomy selections.</param>
    /// <param name="ct">Cancels database operations.</param>
    /// <returns>The post form when invalid, or the post list after saving.</returns>
    [HttpPost("Posts/Create"), ValidateAntiForgeryToken]
    public async Task<IActionResult> CreatePost(BlogPostInputModel model, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            SetUploadUrls();
            return this.View("PostForm", await PopulatePostOptions(model, ct));
        }
        ApplicationUser? author = await users.GetUserAsync(User);
        if (author is null)
        {
            return this.Forbid();
        }

        BlogPost post = new()
        {
            Title = model.Title.Trim(),
            Body = model.Body,
            Slug = await UniqueSlug(model.Slug, model.Title, null, ct),
            AuthorId = author.Id,
            Created = DateTime.UtcNow,
            Modified = DateTime.UtcNow
        };

        await SetPostRelations(post, model, ct);
        db.Add(post);
        await db.SaveChangesAsync(ct);

        TempData["SuccessMessage"] = "The post was created.";
        return this.RedirectToAction(nameof(Posts));
    }

    /// <summary>
    /// Loads a post and its taxonomy selections for editing.
    /// </summary>
    /// <param name="id">The post identifier.</param>
    /// <param name="ct">Cancels database operations.</param>
    /// <returns>The populated post form, or a not-found response.</returns>
    [HttpGet("Posts/{id:int}/Edit")]
    public async Task<IActionResult> EditPost(int id, CancellationToken ct)
    {
        BlogPost? post = await db.Set<BlogPost>()
            .AsNoTracking()
            .Include(blogPost => blogPost.Tags)
            .Include(blogPost => blogPost.Categories)
            .FirstOrDefaultAsync(blogPost => blogPost.Id == id, ct);
        if (post is null)
        {
            return this.NotFound();
        }

        BlogPostInputModel model = new()
        {
            Id = post.Id,
            Title = post.Title,
            Body = fileUploadService.NormalizeLegacyUploadUrls(post.Body, Request.PathBase),
            Slug = post.Slug,
            CategoryIds = post.Categories.Select(category => category.Id).ToList(),
            TagIds = post.Tags.Select(tag => tag.Id).ToList()
        };

        SetUploadUrls();
        return this.View("PostForm", await PopulatePostOptions(model, ct));
    }

    /// <summary>
    /// Updates a post from the submitted form.
    /// </summary>
    /// <param name="id">The post identifier from the route.</param>
    /// <param name="model">The submitted post details and taxonomy selections.</param>
    /// <param name="ct">Cancels database operations.</param>
    /// <returns>The post form when invalid, or the post list after saving.</returns>
    [HttpPost("Posts/{id:int}/Edit"), ValidateAntiForgeryToken]
    public async Task<IActionResult> EditPost(int id, BlogPostInputModel model, CancellationToken ct)
    {
        if (id != model.Id)
        {
            return this.BadRequest();
        }

        model.Body = fileUploadService.NormalizeLegacyUploadUrls(model.Body, Request.PathBase);

        BlogPost? post = await db.Set<BlogPost>()
            .Include(blogPost => blogPost.Tags)
            .Include(blogPost => blogPost.Categories)
            .FirstOrDefaultAsync(blogPost => blogPost.Id == id, ct);
        if (post is null)
        {
            return this.NotFound();
        }
        if (!ModelState.IsValid)
        {
            SetUploadUrls();
            return this.View("PostForm", await PopulatePostOptions(model, ct));
        }
        post.Title = model.Title.Trim();
        post.Body = model.Body;
        post.Slug = await UniqueSlug(model.Slug, model.Title, id, ct);
        post.Modified = DateTime.UtcNow;

        await SetPostRelations(post, model, ct);
        await db.SaveChangesAsync(ct);
        TempData["SuccessMessage"] = "The post was updated.";
        return this.RedirectToAction(nameof(Posts));
    }

    /// <summary>
    /// Deletes a post.
    /// </summary>
    /// <param name="id">The post identifier.</param>
    /// <param name="ct">Cancels database operations.</param>
    /// <returns>The post list, or a not-found response.</returns>
    [HttpPost("Posts/{id:int}/Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeletePost(int id, CancellationToken ct)
    {
        BlogPost? post = await db.Set<BlogPost>().FirstOrDefaultAsync(blogPost => blogPost.Id == id, ct);
        if (post is null)
        {
            return this.NotFound();
        }
        db.Remove(post);
        await db.SaveChangesAsync(ct);
        TempData["SuccessMessage"] = "The post was deleted.";
        return this.RedirectToAction(nameof(Posts));
    }

    /// <summary>
    /// Lists categories with their parent and post information.
    /// </summary>
    /// <param name="ct">Cancels the database query.</param>
    /// <returns>The category administration view.</returns>
    [HttpGet("Categories")]
    public async Task<IActionResult> Categories(CancellationToken ct)
    {
        List<BlogCategory> categories = await db.Set<BlogCategory>()
            .AsNoTracking()
            .Include(category => category.Parent)
            .Include(category => category.Posts)
            .OrderBy(category => category.Name)
            .ToListAsync(ct);

        return this.View(categories);
    }

    /// <summary>
    /// Displays a blank category form.
    /// </summary>
    /// <param name="ct">Cancels the parent category query.</param>
    /// <returns>The category form.</returns>
    [HttpGet("Categories/Create")]
    public async Task<IActionResult> CreateCategory(CancellationToken ct)
    {
        BlogCategoryInputModel model = await PopulateCategoryOptions(new BlogCategoryInputModel(), ct);
        return this.View("CategoryForm", model);
    }

    /// <summary>
    /// Creates a category from the submitted form.
    /// </summary>
    /// <param name="model">The submitted category details.</param>
    /// <param name="ct">Cancels database operations.</param>
    /// <returns>The category form when invalid, or the category list after saving.</returns>
    [HttpPost("Categories/Create"), ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateCategory(BlogCategoryInputModel model, CancellationToken ct)
    {
        if (model.ParentId.HasValue && !await db.Set<BlogCategory>().AnyAsync(c => c.Id == model.ParentId, ct))
        {
            ModelState.AddModelError(nameof(model.ParentId), "Choose an existing parent category.");
        }
        if (!ModelState.IsValid)
        {
            return this.View("CategoryForm", await PopulateCategoryOptions(model, ct));
        }
        BlogCategory category = new()
        {
            Name = model.Name.Trim(),
            Description = model.Description?.Trim() ?? string.Empty,
            Slug = await UniqueCategorySlug(model.Slug, model.Name, null, ct),
            ParentId = model.ParentId
        };

        db.Add(category);
        await db.SaveChangesAsync(ct);
        TempData["SuccessMessage"] = "The category was created.";
        return this.RedirectToAction(nameof(Categories));
    }

    /// <summary>
    /// Loads a category for editing.
    /// </summary>
    /// <param name="id">The category identifier.</param>
    /// <param name="ct">Cancels database operations.</param>
    /// <returns>The category form, or a not-found response.</returns>
    [HttpGet("Categories/{id:int}/Edit")]
    public async Task<IActionResult> EditCategory(int id, CancellationToken ct)
    {
        BlogCategory? category = await db.Set<BlogCategory>()
            .AsNoTracking()
            .FirstOrDefaultAsync(existingCategory => existingCategory.Id == id, ct);
        if (category is null)
        {
            return this.NotFound();
        }

        BlogCategoryInputModel model = new()
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            Slug = category.Slug,
            ParentId = category.ParentId
        };

        return this.View("CategoryForm", await PopulateCategoryOptions(model, ct, id));
    }

    /// <summary>
    /// Updates a category and validates its parent relationship.
    /// </summary>
    /// <param name="id">The category identifier from the route.</param>
    /// <param name="model">The submitted category details.</param>
    /// <param name="ct">Cancels database operations.</param>
    /// <returns>The category form when invalid, or the category list after saving.</returns>
    [HttpPost("Categories/{id:int}/Edit"), ValidateAntiForgeryToken]
    public async Task<IActionResult> EditCategory(int id, BlogCategoryInputModel model, CancellationToken ct)
    {
        if (id != model.Id)
        {
            return this.BadRequest();
        }
        BlogCategory? category = await db.Set<BlogCategory>().FirstOrDefaultAsync(existingCategory => existingCategory.Id == id, ct);
        if (category is null)
        {
            return this.NotFound();
        }
        if (model.ParentId == id || (model.ParentId.HasValue && await IsDescendant(id, model.ParentId.Value, ct)))
        {
            ModelState.AddModelError(nameof(model.ParentId), "A category cannot be its own parent or a descendant.");
        }
        if (model.ParentId.HasValue && !await db.Set<BlogCategory>().AnyAsync(x => x.Id == model.ParentId, ct))
        {
            ModelState.AddModelError(nameof(model.ParentId), "Choose an existing parent category.");
        }
        if (!ModelState.IsValid)
        {
            return this.View("CategoryForm", await PopulateCategoryOptions(model, ct, id));
        }
        category.Name = model.Name.Trim();
        category.Description = model.Description?.Trim() ?? string.Empty;
        category.Slug = await UniqueCategorySlug(model.Slug, model.Name, id, ct);
        category.ParentId = model.ParentId;
        await db.SaveChangesAsync(ct);
        TempData["SuccessMessage"] = "The category was updated.";
        return this.RedirectToAction(nameof(Categories));
    }

    /// <summary>
    /// Deletes a category that has no child categories or posts.
    /// </summary>
    /// <param name="id">The category identifier.</param>
    /// <param name="ct">Cancels database operations.</param>
    /// <returns>The category list, or a not-found response.</returns>
    [HttpPost("Categories/{id:int}/Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteCategory(int id, CancellationToken ct)
    {
        BlogCategory? category = await db.Set<BlogCategory>()
            .Include(existingCategory => existingCategory.Children)
            .Include(existingCategory => existingCategory.Posts)
            .FirstOrDefaultAsync(existingCategory => existingCategory.Id == id, ct);
        if (category is null)
        {
            return this.NotFound();
        }

        if (category.Children.Count > 0 || category.Posts.Count > 0)
        {
            TempData["ErrorMessage"] = "Move this category’s child categories and posts before deleting it.";
            return this.RedirectToAction(nameof(Categories));
        }

        db.Remove(category);
        await db.SaveChangesAsync(ct);
        TempData["SuccessMessage"] = "The category was deleted.";
        return this.RedirectToAction(nameof(Categories));
    }

    /// <summary>
    /// Lists tags and their associated posts.
    /// </summary>
    /// <param name="ct">Cancels the database query.</param>
    /// <returns>The tag administration view.</returns>
    [HttpGet("Tags")]
    public async Task<IActionResult> Tags(CancellationToken ct)
    {
        List<BlogTag> tags = await db.Set<BlogTag>()
            .AsNoTracking()
            .Include(tag => tag.Posts)
            .OrderBy(tag => tag.Name)
            .ToListAsync(ct);

        return this.View(tags);
    }

    /// <summary>
    /// Displays a blank tag form.
    /// </summary>
    /// <returns>The tag form.</returns>
    [HttpGet("Tags/Create")]
    public IActionResult CreateTag() => this.View("TagForm", new BlogTagInputModel());

    /// <summary>
    /// Creates a tag from the submitted form.
    /// </summary>
    /// <param name="model">The submitted tag name.</param>
    /// <param name="ct">Cancels database operations.</param>
    /// <returns>The tag form when invalid, or the tag list after saving.</returns>
    [HttpPost("Tags/Create"), ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateTag(BlogTagInputModel model, CancellationToken ct)
    {
        string name = model.Name?.Trim() ?? "";
        if (name.Length > 0 && await db.Set<BlogTag>().AnyAsync(t => t.Name == name, ct))
        {
            ModelState.AddModelError(nameof(model.Name), "A tag with this name already exists.");
        }
        if (!ModelState.IsValid)
        {
            return this.View("TagForm", model);
        }
        db.Add(new BlogTag { Name = name });
        await db.SaveChangesAsync(ct);
        TempData["SuccessMessage"] = "The tag was created.";
        return this.RedirectToAction(nameof(Tags));
    }

    /// <summary>
    /// Loads a tag for editing.
    /// </summary>
    /// <param name="id">The tag identifier.</param>
    /// <param name="ct">Cancels the database query.</param>
    /// <returns>The tag form, or a not-found response.</returns>
    [HttpGet("Tags/{id:int}/Edit")]
    public async Task<IActionResult> EditTag(int id, CancellationToken ct)
    {
        BlogTag? tag = await db.Set<BlogTag>()
            .AsNoTracking()
            .FirstOrDefaultAsync(existingTag => existingTag.Id == id, ct);
        if (tag is null)
        {
            return this.NotFound();
        }

        BlogTagInputModel model = new() { Id = tag.Id, Name = tag.Name };
        return this.View("TagForm", model);
    }

    /// <summary>
    /// Updates a tag from the submitted form.
    /// </summary>
    /// <param name="id">The tag identifier from the route.</param>
    /// <param name="model">The submitted tag name.</param>
    /// <param name="ct">Cancels database operations.</param>
    /// <returns>The tag form when invalid, or the tag list after saving.</returns>
    [HttpPost("Tags/{id:int}/Edit"), ValidateAntiForgeryToken]
    public async Task<IActionResult> EditTag(int id, BlogTagInputModel model, CancellationToken ct)
    {
        if (id != model.Id)
        {
            return this.BadRequest();
        }
        BlogTag? tag = await db.Set<BlogTag>().FirstOrDefaultAsync(existingTag => existingTag.Id == id, ct);
        if (tag is null)
        {
            return this.NotFound();
        }
        string name = model.Name?.Trim() ?? "";
        if (name.Length > 0 && await db.Set<BlogTag>().AnyAsync(x => x.Id != id && x.Name == name, ct))
        {
            ModelState.AddModelError(nameof(model.Name), "A tag with this name already exists.");
        }
        if (!ModelState.IsValid)
        {
            return this.View("TagForm", model);
        }
        tag.Name = name;
        await db.SaveChangesAsync(ct);
        TempData["SuccessMessage"] = "The tag was updated.";
        return this.RedirectToAction(nameof(Tags));
    }

    /// <summary>
    /// Deletes a tag that is not assigned to any posts.
    /// </summary>
    /// <param name="id">The tag identifier.</param>
    /// <param name="ct">Cancels database operations.</param>
    /// <returns>The tag list, or a not-found response.</returns>
    [HttpPost("Tags/{id:int}/Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteTag(int id, CancellationToken ct)
    {
        BlogTag? tag = await db.Set<BlogTag>()
            .Include(existingTag => existingTag.Posts)
            .FirstOrDefaultAsync(existingTag => existingTag.Id == id, ct);
        if (tag is null)
        {
            return this.NotFound();
        }
        if (tag.Posts.Count > 0)
        {
            TempData["ErrorMessage"] = "Remove this tag from its posts before deleting it.";
            return this.RedirectToAction(nameof(Tags));
        }

        db.Remove(tag);
        await db.SaveChangesAsync(ct);
        TempData["SuccessMessage"] = "The tag was deleted.";
        return this.RedirectToAction(nameof(Tags));
    }

    private async Task<BlogPostInputModel> PopulatePostOptions(BlogPostInputModel m, CancellationToken ct)
    {
        m.Categories = await db.Set<BlogCategory>().AsNoTracking().OrderBy(c => c.Name).Select(c => new BlogCategoryOption(c.Id, c.Name)).ToListAsync(ct);
        m.Tags = await db.Set<BlogTag>().AsNoTracking().OrderBy(t => t.Name).Select(t => new BlogTagOption(t.Id, t.Name)).ToListAsync(ct);
        return m;
    }

    private void SetUploadUrls()
    {
        ViewData["ImageUploadUrl"] = fileUploadService.GetImageUploadUrl(Request.PathBase);
        ViewData["AttachmentUploadUrl"] = fileUploadService.GetAttachmentUploadUrl(Request.PathBase);
    }

    private async Task SetPostRelations(BlogPost p, BlogPostInputModel m, CancellationToken ct)
    {
        List<int> categoryIds = m.CategoryIds.Distinct().ToList();
        List<int> tagIds = m.TagIds.Distinct().ToList();

        p.Categories = await db.Set<BlogCategory>()
            .Where(category => categoryIds.Contains(category.Id))
            .ToListAsync(ct);
        p.Tags = await db.Set<BlogTag>()
            .Where(tag => tagIds.Contains(tag.Id))
            .ToListAsync(ct);
    }

    private async Task<BlogCategoryInputModel> PopulateCategoryOptions(BlogCategoryInputModel m, CancellationToken ct, int? excludingId = null)
    {
        m.Categories = await db.Set<BlogCategory>().AsNoTracking().Where(c => !excludingId.HasValue || c.Id != excludingId).OrderBy(c => c.Name).Select(c => new BlogCategoryOption(c.Id, c.Name)).ToListAsync(ct);
        return m;
    }

    private async Task<bool> IsDescendant(int id, int candidateParent, CancellationToken ct)
    {
        int? current = candidateParent;
        HashSet<int> seen = [];

        // The seen set also prevents corrupted category data from trapping the traversal in a cycle.
        while (current.HasValue && seen.Add(current.Value))
        {
            if (current.Value == id)
            {
                return true;
            }
            current = await db.Set<BlogCategory>()
                .Where(category => category.Id == current.Value)
                .Select(category => category.ParentId)
                .FirstOrDefaultAsync(ct);
        }
        return false;
    }

    private async Task<string> UniqueSlug(string? requested, string fallback, int? except, CancellationToken ct)
    {
        string root = Slugify(string.IsNullOrWhiteSpace(requested) ? fallback : requested);
        string result = root;
        int suffix = 2;

        // Preserve the requested slug when available and add a numeric suffix only when it is already taken.
        while (await db.Set<BlogPost>().AnyAsync(post => post.Slug == result && (!except.HasValue || post.Id != except), ct))
        {
            result = $"{root}-{suffix++}";
        }

        return result;
    }

    private async Task<string> UniqueCategorySlug(string? requested, string fallback, int? except, CancellationToken ct)
    {
        string root = Slugify(string.IsNullOrWhiteSpace(requested) ? fallback : requested);
        string result = root;
        int suffix = 2;

        while (await db.Set<BlogCategory>().AnyAsync(category => category.Slug == result && (!except.HasValue || category.Id != except), ct))
        {
            result = $"{root}-{suffix++}";
        }

        return result;
    }

    private static string Slugify(string value)
    {
        string slug = Regex.Replace(value.Trim().ToLowerInvariant(), @"[^a-z0-9]+", "-").Trim('-');
        return string.IsNullOrWhiteSpace(slug) ? "item" : slug;
    }
}
