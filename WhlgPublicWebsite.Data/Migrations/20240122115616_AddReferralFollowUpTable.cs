using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace HerPublicWebsite.Data.Migrations
{
    public partial class AddReferralFollowUpTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ReferralRequestFollowUps",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ReferralRequestId = table.Column<int>(type: "integer", nullable: false),
                    Token = table.Column<string>(type: "text", nullable: true),
                    WasFollowedUp = table.Column<bool>(type: "boolean", nullable: true),
                    DateOfFollowUpResponse = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReferralRequestFollowUps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReferralRequestFollowUps_ReferralRequests_ReferralRequestId",
                        column: x => x.ReferralRequestId,
                        principalTable: "ReferralRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReferralRequestFollowUps_ReferralRequestId",
                table: "ReferralRequestFollowUps",
                column: "ReferralRequestId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReferralRequestFollowUps_Token",
                table: "ReferralRequestFollowUps",
                column: "Token",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReferralRequestFollowUps");
        }
    }
}
