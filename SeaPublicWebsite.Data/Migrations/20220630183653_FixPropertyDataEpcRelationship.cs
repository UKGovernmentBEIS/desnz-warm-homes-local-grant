using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SeaPublicWebsite.Data.Migrations
{
    public partial class FixPropertyDataEpcRelationship : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PropertyData_Epc_EpcId",
                table: "PropertyData");

            // This index was silently lost in 20220614163252_AddDedicatedPrimaryKeyToEpcTable
            migrationBuilder.Sql(@"DROP INDEX IF EXISTS IX_PropertyData_EpcId");
            // migrationBuilder.DropIndex(
            //     name: "IX_PropertyData_EpcId",
            //     table: "PropertyData");
            
            migrationBuilder.AddColumn<int>(
                name: "PropertyDataId",
                table: "Epc",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            // Delete orphaned epc rows
            migrationBuilder.Sql(@"
                DELETE
                FROM public.""Epc""
                WHERE ""Id"" NOT IN (SELECT ""EpcId"" FROM public.""PropertyData"" WHERE ""EpcId"" IS NOT NULL)");
            
            migrationBuilder.Sql(@"
                UPDATE public.""Epc""
                SET ""PropertyDataId"" = ""PropertyData"".""PropertyDataId""
                FROM public.""PropertyData""
                WHERE ""PropertyData"".""EpcId"" = ""Epc"".""Id""");

            migrationBuilder.DropColumn(
                name: "EpcId",
                table: "PropertyData");

            migrationBuilder.CreateIndex(
                name: "IX_Epc_PropertyDataId",
                table: "Epc",
                column: "PropertyDataId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Epc_PropertyData_PropertyDataId",
                table: "Epc",
                column: "PropertyDataId",
                principalTable: "PropertyData",
                principalColumn: "PropertyDataId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Epc_PropertyData_PropertyDataId",
                table: "Epc");

            migrationBuilder.DropIndex(
                name: "IX_Epc_PropertyDataId",
                table: "Epc");

            migrationBuilder.AddColumn<int>(
                name: "EpcId",
                table: "PropertyData",
                type: "integer",
                nullable: true);
            
            migrationBuilder.Sql(@"
                UPDATE public.""PropertyData""
                SET ""EpcId"" = ""Epc"".""Id""
                FROM public.""Epc""
                WHERE ""Epc"".""PropertyDataId"" = ""PropertyData"".""PropertyDataId""");

            migrationBuilder.DropColumn(
                name: "PropertyDataId",
                table: "Epc");

            migrationBuilder.CreateIndex(
                name: "IX_PropertyData_EpcId",
                table: "PropertyData",
                column: "EpcId");

            migrationBuilder.AddForeignKey(
                name: "FK_PropertyData_Epc_EpcId",
                table: "PropertyData",
                column: "EpcId",
                principalTable: "Epc",
                principalColumn: "Id");
        }
    }
}
