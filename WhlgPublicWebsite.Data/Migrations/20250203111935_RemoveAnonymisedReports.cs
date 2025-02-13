using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace WhlgPublicWebsite.Data.Migrations
{
    public partial class RemoveAnonymisedReports : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AnonymisedReports");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AnonymisedReports",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EpcLodgementDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    EpcRating = table.Column<int>(type: "integer", nullable: false),
                    IncomeBand = table.Column<int>(type: "integer", nullable: false),
                    IsEligible = table.Column<bool>(type: "boolean", nullable: false),
                    IsLsoaProperty = table.Column<bool>(type: "boolean", nullable: false),
                    OwnershipStatus = table.Column<int>(type: "integer", nullable: false),
                    PostcodeFirstHalf = table.Column<string>(type: "text", nullable: true),
                    SubmissionDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnonymisedReports", x => x.Id);
                });
        }
    }
}
