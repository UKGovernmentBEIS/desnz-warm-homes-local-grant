using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace HerPublicWebsite.Data.Migrations
{
    public partial class AddReportingData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AnonymisedReports",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PostcodeFirstHalf = table.Column<string>(type: "text", nullable: true),
                    IsLsoaProperty = table.Column<bool>(type: "boolean", nullable: false),
                    EpcRating = table.Column<int>(type: "integer", nullable: false),
                    EpcLodgementDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsEligible = table.Column<bool>(type: "boolean", nullable: false),
                    HasGasBoiler = table.Column<int>(type: "integer", nullable: false),
                    IncomeBand = table.Column<int>(type: "integer", nullable: false),
                    CustodianCode = table.Column<string>(type: "text", nullable: true),
                    OwnershipStatus = table.Column<int>(type: "integer", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnonymisedReports", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PerReferralReports",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ApplicationDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ReferralCode = table.Column<string>(type: "text", nullable: true),
                    Uprn = table.Column<string>(type: "text", nullable: true),
                    AddressLine1 = table.Column<string>(type: "text", nullable: true),
                    AddressLine2 = table.Column<string>(type: "text", nullable: true),
                    AddressTown = table.Column<string>(type: "text", nullable: true),
                    AddressCounty = table.Column<string>(type: "text", nullable: true),
                    AddressPostcode = table.Column<string>(type: "text", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PerReferralReports", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AnonymisedReports");

            migrationBuilder.DropTable(
                name: "PerReferralReports");
        }
    }
}
