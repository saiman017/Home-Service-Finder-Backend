using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Home_Service_Finder.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedServiceProviderTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsVerified",
                schema: "Users",
                table: "ServiceProvider",
                newName: "IsAdminVerified");

            migrationBuilder.AddColumn<string>(
                name: "PersonalDescription",
                schema: "Users",
                table: "ServiceProvider",
                type: "VARCHAR(500)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PersonalDescription",
                schema: "Users",
                table: "ServiceProvider");

            migrationBuilder.RenameColumn(
                name: "IsAdminVerified",
                schema: "Users",
                table: "ServiceProvider",
                newName: "IsVerified");
        }
    }
}
