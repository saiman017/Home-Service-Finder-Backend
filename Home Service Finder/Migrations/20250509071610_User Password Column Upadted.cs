using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Home_Service_Finder.Migrations
{
    /// <inheritdoc />
    public partial class UserPasswordColumnUpadted : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Password",
                schema: "Users",
                table: "User",
                type: "VARCHAR(100)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "VARCHAR(20)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Password",
                schema: "Users",
                table: "User",
                type: "VARCHAR(20)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "VARCHAR(100)");
        }
    }
}
