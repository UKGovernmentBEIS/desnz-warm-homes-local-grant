using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WhlgPublicWebsite.Data.Migrations
{
    public partial class AddEpcDateToReferralRequest : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "EpcLodgementDate",
                table: "ReferralRequests",
                type: "timestamp without time zone",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EpcLodgementDate",
                table: "ReferralRequests");
        }
    }
}
