using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Asreyion.Server.Migrations.DataDb
{
    /// <inheritdoc />
    public partial class RefactorModulesToCleanArchitecture : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BlogCategories_BlogCategories_ParentId",
                table: "BlogCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_BlogCategoryBlogPost_BlogCategories_CategoriesId",
                table: "BlogCategoryBlogPost");

            migrationBuilder.DropForeignKey(
                name: "FK_BlogCategoryBlogPost_BlogPosts_PostsId",
                table: "BlogCategoryBlogPost");

            migrationBuilder.DropForeignKey(
                name: "FK_BlogPostBlogTag_BlogPosts_PostsId",
                table: "BlogPostBlogTag");

            migrationBuilder.DropForeignKey(
                name: "FK_BlogPostBlogTag_BlogTags_TagsId",
                table: "BlogPostBlogTag");

            migrationBuilder.DropForeignKey(
                name: "FK_BlogPosts_ApplicationUser_AuthorId",
                table: "BlogPosts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_NavigationMenus",
                table: "NavigationMenus");

            migrationBuilder.DropPrimaryKey(
                name: "PK_NavigationMenuItems",
                table: "NavigationMenuItems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BlogTags",
                table: "BlogTags");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BlogPosts",
                table: "BlogPosts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BlogCategories",
                table: "BlogCategories");

            migrationBuilder.DropColumn(
                name: "Children",
                table: "NavigationMenuItems");

            migrationBuilder.RenameTable(
                name: "NavigationMenus",
                newName: "NavigationMenu");

            migrationBuilder.RenameTable(
                name: "NavigationMenuItems",
                newName: "NavigationMenuItem");

            migrationBuilder.RenameTable(
                name: "BlogTags",
                newName: "BlogTag");

            migrationBuilder.RenameTable(
                name: "BlogPosts",
                newName: "BlogPost");

            migrationBuilder.RenameTable(
                name: "BlogCategories",
                newName: "BlogCategory");

            migrationBuilder.RenameIndex(
                name: "IX_BlogTags_Name",
                table: "BlogTag",
                newName: "IX_BlogTag_Name");

            migrationBuilder.RenameIndex(
                name: "IX_BlogPosts_Slug",
                table: "BlogPost",
                newName: "IX_BlogPost_Slug");

            migrationBuilder.RenameIndex(
                name: "IX_BlogPosts_AuthorId",
                table: "BlogPost",
                newName: "IX_BlogPost_AuthorId");

            migrationBuilder.RenameIndex(
                name: "IX_BlogCategories_Slug",
                table: "BlogCategory",
                newName: "IX_BlogCategory_Slug");

            migrationBuilder.RenameIndex(
                name: "IX_BlogCategories_ParentId",
                table: "BlogCategory",
                newName: "IX_BlogCategory_ParentId");

            migrationBuilder.AddColumn<int>(
                name: "ParentId",
                table: "NavigationMenuItem",
                type: "int",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_NavigationMenu",
                table: "NavigationMenu",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_NavigationMenuItem",
                table: "NavigationMenuItem",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BlogTag",
                table: "BlogTag",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BlogPost",
                table: "BlogPost",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BlogCategory",
                table: "BlogCategory",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_NavigationMenuItem_ParentId",
                table: "NavigationMenuItem",
                column: "ParentId");

            migrationBuilder.AddForeignKey(
                name: "FK_BlogCategory_BlogCategory_ParentId",
                table: "BlogCategory",
                column: "ParentId",
                principalTable: "BlogCategory",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BlogCategoryBlogPost_BlogCategory_CategoriesId",
                table: "BlogCategoryBlogPost",
                column: "CategoriesId",
                principalTable: "BlogCategory",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BlogCategoryBlogPost_BlogPost_PostsId",
                table: "BlogCategoryBlogPost",
                column: "PostsId",
                principalTable: "BlogPost",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BlogPost_ApplicationUser_AuthorId",
                table: "BlogPost",
                column: "AuthorId",
                principalTable: "ApplicationUser",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BlogPostBlogTag_BlogPost_PostsId",
                table: "BlogPostBlogTag",
                column: "PostsId",
                principalTable: "BlogPost",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BlogPostBlogTag_BlogTag_TagsId",
                table: "BlogPostBlogTag",
                column: "TagsId",
                principalTable: "BlogTag",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_NavigationMenuItem_NavigationMenuItem_ParentId",
                table: "NavigationMenuItem",
                column: "ParentId",
                principalTable: "NavigationMenuItem",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BlogCategory_BlogCategory_ParentId",
                table: "BlogCategory");

            migrationBuilder.DropForeignKey(
                name: "FK_BlogCategoryBlogPost_BlogCategory_CategoriesId",
                table: "BlogCategoryBlogPost");

            migrationBuilder.DropForeignKey(
                name: "FK_BlogCategoryBlogPost_BlogPost_PostsId",
                table: "BlogCategoryBlogPost");

            migrationBuilder.DropForeignKey(
                name: "FK_BlogPost_ApplicationUser_AuthorId",
                table: "BlogPost");

            migrationBuilder.DropForeignKey(
                name: "FK_BlogPostBlogTag_BlogPost_PostsId",
                table: "BlogPostBlogTag");

            migrationBuilder.DropForeignKey(
                name: "FK_BlogPostBlogTag_BlogTag_TagsId",
                table: "BlogPostBlogTag");

            migrationBuilder.DropForeignKey(
                name: "FK_NavigationMenuItem_NavigationMenuItem_ParentId",
                table: "NavigationMenuItem");

            migrationBuilder.DropPrimaryKey(
                name: "PK_NavigationMenuItem",
                table: "NavigationMenuItem");

            migrationBuilder.DropIndex(
                name: "IX_NavigationMenuItem_ParentId",
                table: "NavigationMenuItem");

            migrationBuilder.DropPrimaryKey(
                name: "PK_NavigationMenu",
                table: "NavigationMenu");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BlogTag",
                table: "BlogTag");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BlogPost",
                table: "BlogPost");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BlogCategory",
                table: "BlogCategory");

            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "NavigationMenuItem");

            migrationBuilder.RenameTable(
                name: "NavigationMenuItem",
                newName: "NavigationMenuItems");

            migrationBuilder.RenameTable(
                name: "NavigationMenu",
                newName: "NavigationMenus");

            migrationBuilder.RenameTable(
                name: "BlogTag",
                newName: "BlogTags");

            migrationBuilder.RenameTable(
                name: "BlogPost",
                newName: "BlogPosts");

            migrationBuilder.RenameTable(
                name: "BlogCategory",
                newName: "BlogCategories");

            migrationBuilder.RenameIndex(
                name: "IX_BlogTag_Name",
                table: "BlogTags",
                newName: "IX_BlogTags_Name");

            migrationBuilder.RenameIndex(
                name: "IX_BlogPost_Slug",
                table: "BlogPosts",
                newName: "IX_BlogPosts_Slug");

            migrationBuilder.RenameIndex(
                name: "IX_BlogPost_AuthorId",
                table: "BlogPosts",
                newName: "IX_BlogPosts_AuthorId");

            migrationBuilder.RenameIndex(
                name: "IX_BlogCategory_Slug",
                table: "BlogCategories",
                newName: "IX_BlogCategories_Slug");

            migrationBuilder.RenameIndex(
                name: "IX_BlogCategory_ParentId",
                table: "BlogCategories",
                newName: "IX_BlogCategories_ParentId");

            migrationBuilder.AddColumn<string>(
                name: "Children",
                table: "NavigationMenuItems",
                type: "json",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddPrimaryKey(
                name: "PK_NavigationMenuItems",
                table: "NavigationMenuItems",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_NavigationMenus",
                table: "NavigationMenus",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BlogTags",
                table: "BlogTags",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BlogPosts",
                table: "BlogPosts",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BlogCategories",
                table: "BlogCategories",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BlogCategories_BlogCategories_ParentId",
                table: "BlogCategories",
                column: "ParentId",
                principalTable: "BlogCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BlogCategoryBlogPost_BlogCategories_CategoriesId",
                table: "BlogCategoryBlogPost",
                column: "CategoriesId",
                principalTable: "BlogCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BlogCategoryBlogPost_BlogPosts_PostsId",
                table: "BlogCategoryBlogPost",
                column: "PostsId",
                principalTable: "BlogPosts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BlogPostBlogTag_BlogPosts_PostsId",
                table: "BlogPostBlogTag",
                column: "PostsId",
                principalTable: "BlogPosts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BlogPostBlogTag_BlogTags_TagsId",
                table: "BlogPostBlogTag",
                column: "TagsId",
                principalTable: "BlogTags",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BlogPosts_ApplicationUser_AuthorId",
                table: "BlogPosts",
                column: "AuthorId",
                principalTable: "ApplicationUser",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
