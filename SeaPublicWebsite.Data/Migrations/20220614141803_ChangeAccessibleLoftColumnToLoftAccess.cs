using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SeaPublicWebsite.Data.Migrations
{
    public partial class ChangeAccessibleLoftColumnToLoftAccess : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AccessibleLoft",
                table: "PropertyData",
                newName: "LoftAccess");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LoftAccess",
                table: "PropertyData",
                newName: "AccessibleLoft");
        }
    }
}
