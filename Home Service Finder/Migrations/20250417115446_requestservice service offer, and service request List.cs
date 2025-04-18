using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Home_Service_Finder.Migrations
{
    /// <inheritdoc />
    public partial class requestserviceserviceofferandservicerequestList : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Requests");

            migrationBuilder.CreateTable(
                name: "ServiceRequest",
                schema: "Requests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uuid", nullable: false),
                    LocationId = table.Column<Guid>(type: "uuid", nullable: false),
                    ServiceCategoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TIMESTAMPTZ", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "TIMESTAMPTZ", nullable: false),
                    Status = table.Column<string>(type: "VARCHAR(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceRequest", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceRequest_Location_LocationId",
                        column: x => x.LocationId,
                        principalSchema: "Locations",
                        principalTable: "Location",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceRequest_ServieCategory_ServiceCategoryId",
                        column: x => x.ServiceCategoryId,
                        principalSchema: "Services",
                        principalTable: "ServieCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceRequest_User_CustomerId",
                        column: x => x.CustomerId,
                        principalSchema: "Users",
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceOffer",
                schema: "Requests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ServiceRequestId = table.Column<Guid>(type: "uuid", nullable: false),
                    ServiceProviderId = table.Column<Guid>(type: "uuid", nullable: false),
                    OfferedPrice = table.Column<decimal>(type: "DECIMAL", nullable: false),
                    Message = table.Column<string>(type: "VARCHAR(500)", nullable: true),
                    SentAt = table.Column<DateTime>(type: "TIMESTAMPTZ", nullable: false),
                    IsAccepted = table.Column<bool>(type: "BOOLEAN", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceOffer", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceOffer_ServiceProvider_ServiceProviderId",
                        column: x => x.ServiceProviderId,
                        principalSchema: "Users",
                        principalTable: "ServiceProvider",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceOffer_ServiceRequest_ServiceRequestId",
                        column: x => x.ServiceRequestId,
                        principalSchema: "Requests",
                        principalTable: "ServiceRequest",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceRequestServiceList",
                schema: "Requests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RequestId = table.Column<Guid>(type: "uuid", nullable: false),
                    ServiceListId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceRequestServiceList", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceRequestServiceList_ServiceList_ServiceListId",
                        column: x => x.ServiceListId,
                        principalSchema: "Services",
                        principalTable: "ServiceList",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceRequestServiceList_ServiceRequest_RequestId",
                        column: x => x.RequestId,
                        principalSchema: "Requests",
                        principalTable: "ServiceRequest",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ServiceOffer_ServiceProviderId",
                schema: "Requests",
                table: "ServiceOffer",
                column: "ServiceProviderId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceOffer_ServiceRequestId",
                schema: "Requests",
                table: "ServiceOffer",
                column: "ServiceRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceRequest_CustomerId",
                schema: "Requests",
                table: "ServiceRequest",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceRequest_LocationId",
                schema: "Requests",
                table: "ServiceRequest",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceRequest_ServiceCategoryId",
                schema: "Requests",
                table: "ServiceRequest",
                column: "ServiceCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceRequestServiceList_RequestId",
                schema: "Requests",
                table: "ServiceRequestServiceList",
                column: "RequestId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceRequestServiceList_ServiceListId",
                schema: "Requests",
                table: "ServiceRequestServiceList",
                column: "ServiceListId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ServiceOffer",
                schema: "Requests");

            migrationBuilder.DropTable(
                name: "ServiceRequestServiceList",
                schema: "Requests");

            migrationBuilder.DropTable(
                name: "ServiceRequest",
                schema: "Requests");
        }
    }
}
