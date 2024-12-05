using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WhlgPublicWebsite.Data.Migrations
{
    public partial class AddFutureGrantsFlag : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "WasSubmittedForFutureGrants",
                table: "ReferralRequests",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WasSubmittedForFutureGrants",
                table: "ReferralRequests");
        }
    }
}
