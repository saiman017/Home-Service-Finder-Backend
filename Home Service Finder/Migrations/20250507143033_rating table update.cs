using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Home_Service_Finder.Migrations
{
    /// <inheritdoc />
    public partial class ratingtableupdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rating_ServiceProvider_ServiceProviderId",
                schema: "Service",
                table: "Rating");

            migrationBuilder.DropIndex(
                name: "IX_Rating_ServiceProviderId",
                schema: "Service",
                table: "Rating");
        }
    }
}
