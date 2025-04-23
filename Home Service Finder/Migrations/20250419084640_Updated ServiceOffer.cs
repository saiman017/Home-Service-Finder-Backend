using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Home_Service_Finder.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedServiceOffer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAccepted",
                schema: "Requests",
                table: "ServiceOffer");

            migrationBuilder.DropColumn(
                name: "Message",
                schema: "Requests",
                table: "ServiceOffer");

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpiresAt",
                schema: "Requests",
                table: "ServiceOffer",
                type: "TIMESTAMPTZ",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Status",
                schema: "Requests",
                table: "ServiceOffer",
                type: "VARCHAR(20)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExpiresAt",
                schema: "Requests",
                table: "ServiceOffer");

            migrationBuilder.DropColumn(
                name: "Status",
                schema: "Requests",
                table: "ServiceOffer");

            migrationBuilder.AddColumn<bool>(
                name: "IsAccepted",
                schema: "Requests",
                table: "ServiceOffer",
                type: "BOOLEAN",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Message",
                schema: "Requests",
                table: "ServiceOffer",
                type: "VARCHAR(500)",
                nullable: true);
        }
    }
}
