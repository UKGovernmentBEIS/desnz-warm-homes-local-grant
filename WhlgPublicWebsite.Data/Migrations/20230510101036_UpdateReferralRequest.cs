using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WhlgPublicWebsite.Data.Migrations
{
    public partial class UpdateReferralRequest : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "CustodianCode",
                table: "ReferralRequests",
                type: "text",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "CustodianCode",
                table: "ReferralRequests",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);
        }
    }
}
