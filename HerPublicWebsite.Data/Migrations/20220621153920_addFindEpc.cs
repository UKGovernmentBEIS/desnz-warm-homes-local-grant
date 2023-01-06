using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HerPublicWebsite.Data.Migrations
{
    public partial class addFindEpc : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FindEpc",
                table: "PropertyData",
                type: "integer",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FindEpc",
                table: "PropertyData");
        }
    }
}
