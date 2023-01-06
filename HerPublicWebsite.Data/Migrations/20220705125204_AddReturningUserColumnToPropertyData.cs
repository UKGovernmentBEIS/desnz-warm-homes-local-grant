using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HerPublicWebsite.Data.Migrations
{
    public partial class AddReturningUserColumnToPropertyData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ReturningUser",
                table: "PropertyData",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReturningUser",
                table: "PropertyData");
        }
    }
}