using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SeaPublicWebsite.Data.Migrations
{
    public partial class FixPropertyDataUneditedDataRelationship : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PropertyData_PropertyData_UneditedDataPropertyDataId",
                table: "PropertyData");

            migrationBuilder.AddColumn<int>(
                name: "EditedDataId",
                table: "PropertyData",
                type: "integer",
                nullable: true,
                defaultValue: null);
            
            // Delete orphaned unedited data rows
            migrationBuilder.Sql(@"
                DELETE
                FROM public.""PropertyData""
                WHERE ""Reference"" IS NULL AND ""PropertyDataId"" NOT IN (SELECT ""UneditedDataPropertyDataId"" FROM public.""PropertyData"" WHERE ""UneditedDataPropertyDataId"" IS NOT NULL)");
            
            migrationBuilder.Sql(@"
                UPDATE public.""PropertyData"" ""unedited""
                SET ""EditedDataId"" = ""edited"".""PropertyDataId""
                FROM public.""PropertyData"" ""edited""
                WHERE ""edited"".""UneditedDataPropertyDataId"" = ""unedited"".""PropertyDataId""");

            migrationBuilder.DropColumn(
                name: "UneditedDataPropertyDataId",
                table: "PropertyData");

            migrationBuilder.CreateIndex(
                name: "IX_PropertyData_EditedDataId",
                table: "PropertyData",
                column: "EditedDataId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PropertyData_PropertyData_EditedDataId",
                table: "PropertyData",
                column: "EditedDataId",
                principalTable: "PropertyData",
                principalColumn: "PropertyDataId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PropertyData_PropertyData_EditedDataId",
                table: "PropertyData");

            migrationBuilder.DropIndex(
                name: "IX_PropertyData_EditedDataId",
                table: "PropertyData");
            
            migrationBuilder.AddColumn<int>(
                name: "UneditedDataPropertyDataId",
                table: "PropertyData",
                type: "integer",
                nullable: true);
            
            migrationBuilder.Sql(@"
                UPDATE public.""PropertyData"" ""edited""
                SET ""UneditedDataPropertyDataId"" = ""unedited"".""EditedDataId""
                FROM public.""PropertyData"" ""unedited""
                WHERE ""unedited"".""EditedDataId"" = ""edited"".""PropertyDataId""");

            migrationBuilder.DropColumn(
                name: "EditedDataId",
                table: "PropertyData");

            migrationBuilder.AddForeignKey(
                name: "FK_PropertyData_PropertyData_UneditedDataPropertyDataId",
                table: "PropertyData",
                column: "UneditedDataPropertyDataId",
                principalTable: "PropertyData",
                principalColumn: "PropertyDataId");
        }
    }
}
