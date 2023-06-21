using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace HerPublicWebsite.Data.Migrations
{
    public partial class AddAnonymisedReports : Migration
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
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnonymisedReports", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AnonymisedReports");
        }
    }
}
