using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace HerPublicWebsite.Data.Migrations
{
    public partial class AddDedicatedPrimaryKeyToEpcTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                    name: "Id",
                    table: "Epc",
                    type: "integer",
                    nullable: false,
                    defaultValue: 0)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);
            
            migrationBuilder.DropForeignKey(
                name: "FK_PropertyData_Epc_EpcId",
                table: "PropertyData");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Epc",
                table: "Epc");
            
            migrationBuilder.AddPrimaryKey(
                name: "PK_Epc",
                table: "Epc",
                column: "Id");

            migrationBuilder.RenameColumn(
                name: "EpcId",
                table: "PropertyData",
                newName: "EpcIdOld");

            migrationBuilder.AddColumn<int>(
                name: "EpcId",
                table: "PropertyData",
                type: "integer",
                nullable: true
            );
            
            migrationBuilder.Sql(@"
                UPDATE public.""PropertyData""
                SET ""EpcId"" = ""Epc"".""Id""
                FROM public.""Epc""
                WHERE ""Epc"".""EpcId"" = ""PropertyData"".""EpcIdOld""");

            migrationBuilder.AlterColumn<string>(
                name: "EpcId",
                table: "Epc",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddForeignKey(
                name: "FK_PropertyData_Epc_EpcId",
                table: "PropertyData",
                column: "EpcId",
                principalTable: "Epc",
                principalColumn: "Id");

            migrationBuilder.DropColumn(
                name: "EpcIdOld",
                table: "PropertyData");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PropertyData_Epc_EpcId",
                table: "PropertyData");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Epc",
                table: "Epc");
            
            migrationBuilder.RenameColumn(
                name: "EpcId",
                table: "PropertyData",
                newName: "EpcIdOld");

            migrationBuilder.AddColumn<string>(
                name: "EpcId",
                table: "PropertyData",
                type: "text",
                nullable: true
            );
            
            migrationBuilder.Sql(@"
                UPDATE public.""PropertyData""
                SET ""EpcId"" = ""Epc"".""EpcId""
                FROM public.""Epc""
                WHERE ""Epc"".""Id"" = ""PropertyData"".""EpcIdOld""");
            
            migrationBuilder.DropColumn(
                name: "Id",
                table: "Epc");

            migrationBuilder.AlterColumn<string>(
                name: "EpcId",
                table: "Epc",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Epc",
                table: "Epc",
                column: "EpcId");

            migrationBuilder.AddForeignKey(
                name: "FK_PropertyData_Epc_EpcId",
                table: "PropertyData",
                column: "EpcId",
                principalTable: "Epc",
                principalColumn: "EpcId");
            
            migrationBuilder.DropColumn(
                name: "EpcIdOld",
                table: "PropertyData");
        }
    }
}
