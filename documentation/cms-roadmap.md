# CMS Development Roadmap

This roadmap outlines the step-by-step process to build a custom CMS with admin dashboard and user authentication using ASP.NET Core, Entity Framework Core, SQLite, and ASP.NET Core Identity.

## Phase 1: Foundation Setup
**Goal:** Set up the database and authentication infrastructure

- [x] Install required NuGet packages (EF Core, SQLite, Identity)
- [x] Create ApplicationDbContext inheriting from IdentityDbContext
- [x] Configure SQLite connection string in appsettings.json
- [x] Register DbContext and Identity services in Program.cs
- [x] Run initial EF Core migration to create database schema
- [x] Seed initial admin user account

## Phase 2: User Authentication
**Goal:** Enable user registration and login

- [x] Create AccountController with Register action
- [x] Create Register view with form (username, email, password)
- [x] Create Login action and view
- [x] Create Logout action
- [x] Add [Authorize] attributes to protect admin areas
- [x] Test registration and login flow

## Phase 3: Admin Dashboard
**Goal:** Build protected admin interface

- [x] Create AdminController with dashboard view
- [x] Create admin layout (separate from main site layout)
- [x] Add role-based authorization (Admin role required)
- [x] Create admin navigation menu
- [ ] Add user management section (list users, manage roles)
- [x] Style admin dashboard with Bootstrap

## Phase 4: Blog Post Management
**Goal:** Enable content creation and management

- [ ] Create BlogPost model (Id, Title, Content, Slug, CreatedAt, AuthorId, Published)
- [ ] Add BlogPosts DbSet to ApplicationDbContext
- [ ] Create migration for blog posts table
- [ ] Create BlogController for public blog display
- [ ] Create AdminBlogController for CRUD operations
- [ ] Create views: Index (list), Create, Edit, Delete
- [ ] Add rich text editor (TinyMCE or similar)
- [ ] Implement slug generation for SEO-friendly URLs
- [ ] Add published/unpublished status toggle

## Phase 5: Public Site
**Goal:** Display content to visitors

- [ ] Create blog index page listing published posts
- [ ] Create blog post detail page
- [ ] Add date formatting and author display
- [ ] Create navigation to blog from home page
- [ ] Add basic styling to public pages

## Phase 6: Additional Features (Future)
**Goal:** Expand functionality for community features

- [ ] Categories and tags system
- [ ] Comments system
- [ ] Media management (image uploads)
- [x] User profiles
- [ ] Forum/discussion boards
- [ ] Search functionality

## Technical Stack Summary
- **Framework:** ASP.NET Core MVC (.NET 10.0)
- **Database:** SQLite with Entity Framework Core
- **Authentication:** ASP.NET Core Identity
- **Frontend:** Razor Views with Bootstrap
- **Editor:** TinyMCE (rich text)

## Key Files to Create
- `Data/ApplicationDbContext.cs`
- `Models/BlogPost.cs`
- `Controllers/AccountController.cs`
- `Controllers/AdminController.cs`
- `Controllers/AdminBlogController.cs`
- `Controllers/BlogController.cs`
- `Views/Account/Register.cshtml`
- `Views/Account/Login.cshtml`
- `Views/Admin/Dashboard.cshtml`
- `Views/AdminBlog/*.cshtml` (CRUD views)
- `Views/Blog/Index.cshtml`
- `Views/Blog/Details.cshtml`

## Commands Reference
```bash
# Add packages
dotnet add package Microsoft.EntityFrameworkCore.Sqlite
dotnet add package Microsoft.EntityFrameworkCore.Tools
dotnet add package Microsoft.AspNetCore.Identity.EntityFrameworkCore

# Create migration
dotnet ef migrations add InitialCreate
dotnet ef database update
```

## Next Steps
Start with Phase 1: Foundation Setup. Each phase builds on the previous one, so complete them in order.
