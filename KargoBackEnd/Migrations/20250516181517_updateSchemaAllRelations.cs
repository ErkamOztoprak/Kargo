using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KargoUygulamasiBackEnd.Migrations
{
    /// <inheritdoc />
    public partial class updateSchemaAllRelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_deliverylogs_users_VerifiedByAdminId",
                table: "deliverylogs");

            migrationBuilder.DropForeignKey(
                name: "FK_logusers_users_UserId",
                table: "logusers");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "users",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "AssignedToUserId",
                table: "parcels",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EventType",
                table: "deliverylogs",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "deliverylogs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_parcels_AssignedToUserId",
                table: "parcels",
                column: "AssignedToUserId");

            migrationBuilder.CreateIndex(
                name: "IX_parcels_RequestedByUserId",
                table: "parcels",
                column: "RequestedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_deliverylogs_users_VerifiedByAdminId",
                table: "deliverylogs",
                column: "VerifiedByAdminId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_logusers_users_UserId",
                table: "logusers",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_parcels_users_AssignedToUserId",
                table: "parcels",
                column: "AssignedToUserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_parcels_users_RequestedByUserId",
                table: "parcels",
                column: "RequestedByUserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_deliverylogs_users_VerifiedByAdminId",
                table: "deliverylogs");

            migrationBuilder.DropForeignKey(
                name: "FK_logusers_users_UserId",
                table: "logusers");

            migrationBuilder.DropForeignKey(
                name: "FK_parcels_users_AssignedToUserId",
                table: "parcels");

            migrationBuilder.DropForeignKey(
                name: "FK_parcels_users_RequestedByUserId",
                table: "parcels");

            migrationBuilder.DropIndex(
                name: "IX_parcels_AssignedToUserId",
                table: "parcels");

            migrationBuilder.DropIndex(
                name: "IX_parcels_RequestedByUserId",
                table: "parcels");

            migrationBuilder.DropColumn(
                name: "AssignedToUserId",
                table: "parcels");

            migrationBuilder.DropColumn(
                name: "EventType",
                table: "deliverylogs");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "deliverylogs");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "users",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AddForeignKey(
                name: "FK_deliverylogs_users_VerifiedByAdminId",
                table: "deliverylogs",
                column: "VerifiedByAdminId",
                principalTable: "users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_logusers_users_UserId",
                table: "logusers",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
