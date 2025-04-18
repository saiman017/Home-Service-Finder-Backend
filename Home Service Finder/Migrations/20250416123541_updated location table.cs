using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Home_Service_Finder.Migrations
{
    /// <inheritdoc />
    public partial class updatedlocationtable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Location_User_Id",
                schema: "Locations",
                table: "Location");

            migrationBuilder.RenameColumn(
                name: "Id",
                schema: "Locations",
                table: "Location",
                newName: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Location_User_UserId",
                schema: "Locations",
                table: "Location",
                column: "UserId",
                principalSchema: "Users",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Location_User_UserId",
                schema: "Locations",
                table: "Location");

            migrationBuilder.RenameColumn(
                name: "UserId",
                schema: "Locations",
                table: "Location",
                newName: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Location_User_Id",
                schema: "Locations",
                table: "Location",
                column: "Id",
                principalSchema: "Users",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
