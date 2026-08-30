using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Asreyion.Server.Migrations.DataDb
{
    /// <inheritdoc />
    public partial class InitialDataMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
