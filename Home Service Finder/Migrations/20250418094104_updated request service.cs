using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Home_Service_Finder.Migrations
{
    /// <inheritdoc />
    public partial class updatedrequestservice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LocationAddress",
                schema: "Requests",
                table: "ServiceRequest",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LocationCity",
                schema: "Requests",
                table: "ServiceRequest",
                type: "VARCHAR(100)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<double>(
                name: "LocationLatitude",
                schema: "Requests",
                table: "ServiceRequest",
                type: "DOUBLE PRECISION",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "LocationLongitude",
                schema: "Requests",
                table: "ServiceRequest",
                type: "DOUBLE PRECISION",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "LocationPostalCode",
                schema: "Requests",
                table: "ServiceRequest",
                type: "VARCHAR(20)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LocationAddress",
                schema: "Requests",
                table: "ServiceRequest");

            migrationBuilder.DropColumn(
                name: "LocationCity",
                schema: "Requests",
                table: "ServiceRequest");

            migrationBuilder.DropColumn(
                name: "LocationLatitude",
                schema: "Requests",
                table: "ServiceRequest");

            migrationBuilder.DropColumn(
                name: "LocationLongitude",
                schema: "Requests",
                table: "ServiceRequest");

            migrationBuilder.DropColumn(
                name: "LocationPostalCode",
                schema: "Requests",
                table: "ServiceRequest");
        }
    }
}
