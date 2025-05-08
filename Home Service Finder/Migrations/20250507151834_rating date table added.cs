using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Home_Service_Finder.Migrations
{
    /// <inheritdoc />
    public partial class ratingdatetableadded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rating_ServiceProvider_ServiceProviderId",
                schema: "Service",
                table: "Rating");

            migrationBuilder.DropForeignKey(
                name: "FK_Rating_User_CustomerId",
                schema: "Service",
                table: "Rating");

            migrationBuilder.DropIndex(
                name: "IX_Rating_CustomerId",
                schema: "Service",
                table: "Rating");

            migrationBuilder.DropIndex(
                name: "IX_Rating_ServiceProviderId",
                schema: "Service",
                table: "Rating");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Rating_CustomerId",
                schema: "Service",
                table: "Rating",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Rating_ServiceProviderId",
                schema: "Service",
                table: "Rating",
                column: "ServiceProviderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Rating_ServiceProvider_ServiceProviderId",
                schema: "Service",
                table: "Rating",
                column: "ServiceProviderId",
                principalSchema: "Users",
                principalTable: "ServiceProvider",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Rating_User_CustomerId",
                schema: "Service",
                table: "Rating",
                column: "CustomerId",
                principalSchema: "Users",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
