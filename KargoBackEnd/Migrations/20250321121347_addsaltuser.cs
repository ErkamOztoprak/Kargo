using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KargoUygulamasiBackEnd.Migrations
{
    /// <inheritdoc />
    public partial class addsaltuser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Salt",
                table: "users",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Salt",
                table: "users");
        }
    }
}
