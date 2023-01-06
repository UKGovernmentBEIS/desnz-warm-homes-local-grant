using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SeaPublicWebsite.Data.Migrations
{
    public partial class AddRowVersions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Note the xmin columns are built into PostgreSQL so this migration
            // actually does nothing.
            migrationBuilder.AddColumn<uint>(
                name: "xmin",
                table: "PropertyRecommendations",
                type: "xid",
                rowVersion: true,
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.AddColumn<uint>(
                name: "xmin",
                table: "PropertyData",
                type: "xid",
                rowVersion: true,
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.AddColumn<uint>(
                name: "xmin",
                table: "Epc",
                type: "xid",
                rowVersion: true,
                nullable: false,
                defaultValue: 0u);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Note the xmin columns are built into PostgreSQL so this migration
            // actually does nothing.
            migrationBuilder.DropColumn(
                name: "xmin",
                table: "PropertyRecommendations");

            migrationBuilder.DropColumn(
                name: "xmin",
                table: "PropertyData");

            migrationBuilder.DropColumn(
                name: "xmin",
                table: "Epc");
        }
    }
}
