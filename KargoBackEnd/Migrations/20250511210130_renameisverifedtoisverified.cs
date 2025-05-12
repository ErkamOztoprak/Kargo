using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KargoUygulamasiBackEnd.Migrations
{
    /// <inheritdoc />
    public partial class renameisverifedtoisverified : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsVerifed",
                table: "users",
                newName: "IsVerified");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsVerified",
                table: "users",
                newName: "IsVerifed");
        }
    }
}
