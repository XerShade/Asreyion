using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Asreyion.Server.Migrations.DataDb
{
    /// <inheritdoc />
    public partial class AddNavigationRouteValues : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RouteValues",
                table: "NavigationMenuItems",
                type: "json",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RouteValues",
                table: "NavigationMenuItems");
        }
    }
}
