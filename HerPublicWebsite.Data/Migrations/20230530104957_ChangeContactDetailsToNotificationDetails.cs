using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HerPublicWebsite.Data.Migrations
{
    public partial class ChangeContactDetailsToNotificationDetails : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReferralRequests_ContactDetails_ContactDetailsId",
                table: "ReferralRequests");

            migrationBuilder.DropIndex(
                name: "IX_ReferralRequests_ContactDetailsId",
                table: "ReferralRequests");

            migrationBuilder.DropColumn(
                name: "ContactDetailsId",
                table: "ReferralRequests");

            migrationBuilder.DropColumn(
                name: "FullName",
                table: "ContactDetails");

            migrationBuilder.DropColumn(
                name: "LaContactTelephone",
                table: "ContactDetails");

            migrationBuilder.RenameColumn(
                name: "LaContactEmailAddress",
                table: "ContactDetails",
                newName: "FutureSchemeNotificationEmail");

            migrationBuilder.AddColumn<string>(
                name: "ContactEmailAddress",
                table: "ReferralRequests",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContactTelephone",
                table: "ReferralRequests",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "ReferralRequests",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "FutureSchemeNotificationConsent",
                table: "ContactDetails",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "ReferralRequestId",
                table: "ContactDetails",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ContactDetails_ReferralRequestId",
                table: "ContactDetails",
                column: "ReferralRequestId");

            migrationBuilder.AddForeignKey(
                name: "FK_ContactDetails_ReferralRequests_ReferralRequestId",
                table: "ContactDetails",
                column: "ReferralRequestId",
                principalTable: "ReferralRequests",
                principalColumn: "Id");

            migrationBuilder.RenameTable(
                name: "ContactDetails",
                newName: "NotificationDetails");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "NotificationDetails",
                newName: "ContactDetails");
            
            migrationBuilder.DropForeignKey(
                name: "FK_ContactDetails_ReferralRequests_ReferralRequestId",
                table: "ContactDetails");

            migrationBuilder.DropIndex(
                name: "IX_ContactDetails_ReferralRequestId",
                table: "ContactDetails");

            migrationBuilder.DropColumn(
                name: "ContactEmailAddress",
                table: "ReferralRequests");

            migrationBuilder.DropColumn(
                name: "ContactTelephone",
                table: "ReferralRequests");

            migrationBuilder.DropColumn(
                name: "FullName",
                table: "ReferralRequests");

            migrationBuilder.DropColumn(
                name: "FutureSchemeNotificationConsent",
                table: "ContactDetails");

            migrationBuilder.DropColumn(
                name: "ReferralRequestId",
                table: "ContactDetails");

            migrationBuilder.RenameColumn(
                name: "FutureSchemeNotificationEmail",
                table: "ContactDetails",
                newName: "LaContactEmailAddress");

            migrationBuilder.AddColumn<int>(
                name: "ContactDetailsId",
                table: "ReferralRequests",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "ContactDetails",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LaContactTelephone",
                table: "ContactDetails",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReferralRequests_ContactDetailsId",
                table: "ReferralRequests",
                column: "ContactDetailsId");

            migrationBuilder.AddForeignKey(
                name: "FK_ReferralRequests_ContactDetails_ContactDetailsId",
                table: "ReferralRequests",
                column: "ContactDetailsId",
                principalTable: "ContactDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
