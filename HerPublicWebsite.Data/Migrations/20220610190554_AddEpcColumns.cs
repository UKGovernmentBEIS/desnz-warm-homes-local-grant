using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HerPublicWebsite.Data.Migrations
{
    public partial class AddEpcColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EpcLmkKey",
                table: "PropertyData");

            migrationBuilder.DropColumn(
                name: "BuildingReference",
                table: "Epc");

            migrationBuilder.RenameColumn(
                name: "InspectionDate",
                table: "Epc",
                newName: "LodgementDate");

            migrationBuilder.AddColumn<int>(
                name: "BungalowType",
                table: "Epc",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FlatType",
                table: "Epc",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "GlazingType",
                table: "Epc",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "HouseType",
                table: "Epc",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RoofConstruction",
                table: "Epc",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RoofInsulated",
                table: "Epc",
                type: "integer",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BungalowType",
                table: "Epc");

            migrationBuilder.DropColumn(
                name: "FlatType",
                table: "Epc");

            migrationBuilder.DropColumn(
                name: "GlazingType",
                table: "Epc");

            migrationBuilder.DropColumn(
                name: "HouseType",
                table: "Epc");

            migrationBuilder.DropColumn(
                name: "RoofConstruction",
                table: "Epc");

            migrationBuilder.DropColumn(
                name: "RoofInsulated",
                table: "Epc");

            migrationBuilder.RenameColumn(
                name: "LodgementDate",
                table: "Epc",
                newName: "InspectionDate");

            migrationBuilder.AddColumn<string>(
                name: "EpcLmkKey",
                table: "PropertyData",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BuildingReference",
                table: "Epc",
                type: "text",
                nullable: true);
        }
    }
}
