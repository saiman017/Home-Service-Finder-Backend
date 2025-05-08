using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Home_Service_Finder.Migrations
{
    /// <inheritdoc />
    public partial class PaymentaddedinServiceOffer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PaymentReason",
                schema: "Requests",
                table: "ServiceOffer",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "PaymentStatus",
                schema: "Requests",
                table: "ServiceOffer",
                type: "BOOLEAN",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentReason",
                schema: "Requests",
                table: "ServiceOffer");

            migrationBuilder.DropColumn(
                name: "PaymentStatus",
                schema: "Requests",
                table: "ServiceOffer");
        }
    }
}
