using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace WhlgPublicWebsite.Data.Migrations
{
    /// <inheritdoc />
    public partial class _202607081142_AddFutureContactOptions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FutureContactConsents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Email = table.Column<string>(type: "text", nullable: true),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    ConsentToGrants = table.Column<bool>(type: "boolean", nullable: false),
                    ConsentToAdvice = table.Column<bool>(type: "boolean", nullable: false),
                    ConsentToUpdates = table.Column<bool>(type: "boolean", nullable: false),
                    ContactByEmail = table.Column<bool>(type: "boolean", nullable: false),
                    ContactByPhone = table.Column<bool>(type: "boolean", nullable: false),
                    ContactBySms = table.Column<bool>(type: "boolean", nullable: false),
                    ReferralRequestId = table.Column<int>(type: "integer", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FutureContactConsents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FutureContactConsents_ReferralRequests_ReferralRequestId",
                        column: x => x.ReferralRequestId,
                        principalTable: "ReferralRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FutureContactConsents_ReferralRequestId",
                table: "FutureContactConsents",
                column: "ReferralRequestId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FutureContactConsents");
        }
    }
}
