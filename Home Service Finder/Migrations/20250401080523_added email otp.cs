using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Home_Service_Finder.Migrations
{
    /// <inheritdoc />
    public partial class addedemailotp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EmailOTP",
                schema: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "VARCHAR(6)", nullable: false),
                    ExpiryTime = table.Column<DateTime>(type: "TIMESTAMPTZ", nullable: false),
                    IsUsed = table.Column<bool>(type: "BOOLEAN", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TIMESTAMPTZ", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailOTP", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmailOTP_User_UserId",
                        column: x => x.UserId,
                        principalSchema: "Users",
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmailOTP_UserId",
                schema: "Users",
                table: "EmailOTP",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmailOTP",
                schema: "Users");
        }
    }
}
