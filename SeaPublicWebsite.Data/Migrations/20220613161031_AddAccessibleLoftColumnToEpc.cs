using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SeaPublicWebsite.Data.Migrations
{
    public partial class AddAccessibleLoftColumnToEpc : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AccessibleLoftSpace",
                table: "PropertyData",
                newName: "LoftSpace");

            migrationBuilder.AddColumn<int>(
                name: "AccessibleLoft",
                table: "PropertyData",
                type: "integer",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccessibleLoft",
                table: "PropertyData");

            migrationBuilder.RenameColumn(
                name: "LoftSpace",
                table: "PropertyData",
                newName: "AccessibleLoftSpace");
        }
    }
}
