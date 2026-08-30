using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Asreyion.Server.Migrations.DataDb
{
    /// <inheritdoc />
    public partial class AddNavigationItemTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ItemType",
                table: "NavigationMenuItems",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ItemType",
                table: "NavigationMenuItems");
        }
    }
}
