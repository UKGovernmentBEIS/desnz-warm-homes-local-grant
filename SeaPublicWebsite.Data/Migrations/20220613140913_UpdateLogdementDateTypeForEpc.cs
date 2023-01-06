using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SeaPublicWebsite.Data.Migrations
{
    public partial class UpdateLogdementDateTypeForEpc : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                "LodgementDate",
                "Epc");
            migrationBuilder.AddColumn<DateTime>(
                "LodgementDate",
                "Epc",
                type: "timestamp with time zone",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                "LodgementDate",
                "Epc");
            migrationBuilder.AddColumn<string>(
                "LodgementDate",
                "Epc",
                type: "text",
                nullable: true);
        }
    }
}
