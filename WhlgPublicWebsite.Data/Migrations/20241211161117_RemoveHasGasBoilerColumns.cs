using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WhlgPublicWebsite.Data.Migrations
{
    public partial class RemoveHasGasBoilerColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasGasBoiler",
                table: "ReferralRequests");

            migrationBuilder.DropColumn(
                name: "HasGasBoiler",
                table: "AnonymisedReports");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "HasGasBoiler",
                table: "ReferralRequests",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "HasGasBoiler",
                table: "AnonymisedReports",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
