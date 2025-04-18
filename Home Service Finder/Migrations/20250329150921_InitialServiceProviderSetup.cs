using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Home_Service_Finder.Migrations
{
    /// <inheritdoc />
    public partial class InitialServiceProviderSetup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Services");

            migrationBuilder.AddColumn<bool>(
                name: "IsEmailVerified",
                schema: "Users",
                table: "User",
                type: "BOOLEAN",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "ServieCategory",
                schema: "Services",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "VARCHAR(50)", nullable: false),
                    Description = table.Column<string>(type: "VARCHAR(500)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TIMESTAMPTZ", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "TIMESTAMPTZ", nullable: false),
                    IsDeleted = table.Column<bool>(type: "BOOLEAN", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServieCategory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ServiceProvider",
                schema: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Licenses = table.Column<string>(type: "VARCHAR(500)", nullable: false),
                    Experience = table.Column<int>(type: "INT", nullable: false),
                    IsActive = table.Column<bool>(type: "BOOLEAN", nullable: false),
                    IsDocumentVerified = table.Column<bool>(type: "BOOLEAN", nullable: false),
                    ServiceCategoryId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceProvider", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceProvider_ServieCategory_ServiceCategoryId",
                        column: x => x.ServiceCategoryId,
                        principalSchema: "Services",
                        principalTable: "ServieCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceProvider_User_Id",
                        column: x => x.Id,
                        principalSchema: "Users",
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ServiceProvider_ServiceCategoryId",
                schema: "Users",
                table: "ServiceProvider",
                column: "ServiceCategoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ServiceProvider",
                schema: "Users");

            migrationBuilder.DropTable(
                name: "ServieCategory",
                schema: "Services");

            migrationBuilder.DropColumn(
                name: "IsEmailVerified",
                schema: "Users",
                table: "User");
        }
    }
}
