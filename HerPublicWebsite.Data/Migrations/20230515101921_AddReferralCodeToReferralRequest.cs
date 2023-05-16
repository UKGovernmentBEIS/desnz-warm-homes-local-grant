using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HerPublicWebsite.Data.Migrations
{
    public partial class AddReferralCodeToReferralRequest : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ReferralCode",
                table: "ReferralRequests",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReferralCode",
                table: "ReferralRequests");
        }
    }
}
