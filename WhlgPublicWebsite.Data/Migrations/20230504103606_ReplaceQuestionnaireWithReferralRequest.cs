using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace WhlgPublicWebsite.Data.Migrations
{
    public partial class ReplaceQuestionnaireWithReferralRequest : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ContactDetails_Questionnaires_QuestionnaireId",
                table: "ContactDetails");

            migrationBuilder.DropTable(
                name: "Questionnaires");

            migrationBuilder.DropIndex(
                name: "IX_ContactDetails_QuestionnaireId",
                table: "ContactDetails");

            migrationBuilder.DropColumn(
                name: "QuestionnaireId",
                table: "ContactDetails");

            migrationBuilder.CreateTable(
                name: "ReferralRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AddressLine1 = table.Column<string>(type: "text", nullable: true),
                    AddressLine2 = table.Column<string>(type: "text", nullable: true),
                    AddressLine3 = table.Column<string>(type: "text", nullable: true),
                    AddressTown = table.Column<string>(type: "text", nullable: true),
                    AddressPostcode = table.Column<string>(type: "text", nullable: true),
                    CustodianCode = table.Column<int>(type: "integer", nullable: false),
                    Uprn = table.Column<string>(type: "text", nullable: true),
                    EpcRating = table.Column<int>(type: "integer", nullable: false),
                    IsLsoaProperty = table.Column<bool>(type: "boolean", nullable: false),
                    HasGasBoiler = table.Column<int>(type: "integer", nullable: false),
                    IncomeBand = table.Column<int>(type: "integer", nullable: false),
                    RequestDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ReferralCreated = table.Column<bool>(type: "boolean", nullable: false),
                    ContactDetailsId = table.Column<int>(type: "integer", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReferralRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReferralRequests_ContactDetails_ContactDetailsId",
                        column: x => x.ContactDetailsId,
                        principalTable: "ContactDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReferralRequests_ContactDetailsId",
                table: "ReferralRequests",
                column: "ContactDetailsId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReferralRequests");

            migrationBuilder.AddColumn<int>(
                name: "QuestionnaireId",
                table: "ContactDetails",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Questionnaires",
                columns: table => new
                {
                    QuestionnaireId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AddressLine1 = table.Column<string>(type: "text", nullable: true),
                    AddressLine2 = table.Column<string>(type: "text", nullable: true),
                    AddressLine3 = table.Column<string>(type: "text", nullable: true),
                    AddressPostcode = table.Column<string>(type: "text", nullable: true),
                    AddressTown = table.Column<string>(type: "text", nullable: true),
                    EpcRating = table.Column<int>(type: "integer", nullable: false),
                    HasGasBoiler = table.Column<int>(type: "integer", nullable: false),
                    Hug2ReferralId = table.Column<string>(type: "text", nullable: true),
                    IncomeBand = table.Column<int>(type: "integer", nullable: false),
                    IsEligibleForHug2 = table.Column<bool>(type: "boolean", nullable: false),
                    IsLsoaProperty = table.Column<bool>(type: "boolean", nullable: false),
                    ReferralCreated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Uprn = table.Column<string>(type: "text", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Questionnaires", x => x.QuestionnaireId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ContactDetails_QuestionnaireId",
                table: "ContactDetails",
                column: "QuestionnaireId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ContactDetails_Questionnaires_QuestionnaireId",
                table: "ContactDetails",
                column: "QuestionnaireId",
                principalTable: "Questionnaires",
                principalColumn: "QuestionnaireId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
