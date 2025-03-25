using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WhlgPublicWebsite.Data.Migrations
{
    public partial class SetNoActionOndeleteForNotificationDetailsReferral : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ContactDetails_ReferralRequests_ReferralRequestId",
                table: "NotificationDetails");

            migrationBuilder.AddForeignKey(
                name: "FK_NotificationDetails_ReferralRequests_ReferralRequestId",
                table: "NotificationDetails",
                column: "ReferralRequestId",
                principalTable: "ReferralRequests",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NotificationDetails_ReferralRequests_ReferralRequestId",
                table: "NotificationDetails");

            migrationBuilder.AddForeignKey(
                name: "FK_ContactDetails_ReferralRequests_ReferralRequestId",
                table: "NotificationDetails",
                column: "ReferralRequestId",
                principalTable: "ReferralRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
