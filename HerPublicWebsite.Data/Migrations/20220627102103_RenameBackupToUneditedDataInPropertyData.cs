using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HerPublicWebsite.Data.Migrations
{
    public partial class RenameBackupToUneditedDataInPropertyData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PropertyData_PropertyData_BackupPropertyDataId",
                table: "PropertyData");
            migrationBuilder.RenameColumn(
                name: "BackupPropertyDataId",
                table: "PropertyData",
                newName: "UneditedDataPropertyDataId");
            migrationBuilder.AddForeignKey(
                name: "FK_PropertyData_PropertyData_UneditedDataPropertyDataId",
                table: "PropertyData",
                column: "UneditedDataPropertyDataId",
                principalTable: "PropertyData",
                principalColumn: "PropertyDataId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PropertyData_PropertyData_UneditedDataPropertyDataId",
                table: "PropertyData");
            migrationBuilder.RenameColumn(
                name: "UneditedDataPropertyDataId",
                table: "PropertyData",
                newName: "BackupPropertyDataId");
            migrationBuilder.AddForeignKey(
                name: "FK_PropertyData_PropertyData_BackupPropertyDataId",
                table: "PropertyData",
                column: "BackupPropertyDataId",
                principalTable: "PropertyData",
                principalColumn: "PropertyDataId");
        }
    }
}
