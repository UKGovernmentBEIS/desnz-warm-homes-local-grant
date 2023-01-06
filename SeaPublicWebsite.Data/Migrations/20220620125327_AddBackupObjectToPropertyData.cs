using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SeaPublicWebsite.Data.Migrations
{
    public partial class AddUpdatesObjectToPropertyData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BackupPropertyDataId",
                table: "PropertyData",
                type: "integer",
                nullable: true);
            
            migrationBuilder.AddForeignKey(
                name: "FK_PropertyData_PropertyData_BackupPropertyDataId",
                table: "PropertyData",
                column: "BackupPropertyDataId",
                principalTable: "PropertyData",
                principalColumn: "PropertyDataId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PropertyData_PropertyData_BackupPropertyDataId",
                table: "PropertyData");
            
            migrationBuilder.DropColumn(
                name: "BackupPropertyDataId",
                table: "PropertyData");
        }
    }
}
