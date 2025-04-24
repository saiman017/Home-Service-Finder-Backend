using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Home_Service_Finder.Migrations
{
    /// <inheritdoc />
    public partial class ServicerequestImageandProfileimage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ServiceRequestImage",
                schema: "Requests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ServiceRequestId = table.Column<Guid>(type: "uuid", nullable: false),
                    ImagePath = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceRequestImage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceRequestImage_ServiceRequest_ServiceRequestId",
                        column: x => x.ServiceRequestId,
                        principalSchema: "Requests",
                        principalTable: "ServiceRequest",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ServiceRequestImage_ServiceRequestId",
                schema: "Requests",
                table: "ServiceRequestImage",
                column: "ServiceRequestId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ServiceRequestImage",
                schema: "Requests");
        }
    }
}
