using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Home_Service_Finder.Migrations
{
    /// <inheritdoc />
    public partial class Removeusernameandmiddlename : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MiddleName",
                schema: "Users",
                table: "UserDetail");

            migrationBuilder.DropColumn(
                name: "Username",
                schema: "Users",
                table: "User");

            migrationBuilder.DropColumn(
                name: "IsDocumentVerified",
                schema: "Users",
                table: "ServiceProvider");

            migrationBuilder.DropColumn(
                name: "Licenses",
                schema: "Users",
                table: "ServiceProvider");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MiddleName",
                schema: "Users",
                table: "UserDetail",
                type: "VARCHAR(50)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Username",
                schema: "Users",
                table: "User",
                type: "VARCHAR(50)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsDocumentVerified",
                schema: "Users",
                table: "ServiceProvider",
                type: "BOOLEAN",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Licenses",
                schema: "Users",
                table: "ServiceProvider",
                type: "VARCHAR(500)",
                nullable: false,
                defaultValue: "");
        }
    }
}
