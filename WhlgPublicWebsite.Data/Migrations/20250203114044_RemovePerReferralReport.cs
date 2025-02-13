using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace WhlgPublicWebsite.Data.Migrations
{
    public partial class RemovePerReferralReport : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PerReferralReports");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PerReferralReports",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AddressCounty = table.Column<string>(type: "text", nullable: true),
                    AddressLine1 = table.Column<string>(type: "text", nullable: true),
                    AddressLine2 = table.Column<string>(type: "text", nullable: true),
                    AddressPostcode = table.Column<string>(type: "text", nullable: true),
                    AddressTown = table.Column<string>(type: "text", nullable: true),
                    ApplicationDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ReferralCode = table.Column<string>(type: "text", nullable: true),
                    Uprn = table.Column<string>(type: "text", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PerReferralReports", x => x.Id);
                });
        }
    }
}
