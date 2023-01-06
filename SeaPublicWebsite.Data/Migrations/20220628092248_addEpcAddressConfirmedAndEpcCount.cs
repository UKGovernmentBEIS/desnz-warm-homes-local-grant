using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SeaPublicWebsite.Data.Migrations
{
    public partial class addEpcAddressConfirmedAndEpcCount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EpcAddressConfirmed",
                table: "PropertyData",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EpcCount",
                table: "PropertyData",
                type: "integer",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EpcAddressConfirmed",
                table: "PropertyData");

            migrationBuilder.DropColumn(
                name: "EpcCount",
                table: "PropertyData");
        }
    }
}
