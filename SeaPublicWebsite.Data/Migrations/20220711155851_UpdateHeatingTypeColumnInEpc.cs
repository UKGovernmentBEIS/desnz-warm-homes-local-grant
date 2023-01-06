using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SeaPublicWebsite.Data.Migrations
{
    public partial class UpdateHeatingTypeColumnInEpc : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EpcHeatingType",
                table: "Epc",
                type: "integer",
                nullable: true);
            
            // Convert the other heating type into the new column
            // Original column enum had 8 options, so we stack these new values
            // starting from 8
            // We also ignore the 'other' option since it already exists in the normal heating type
            migrationBuilder.Sql(@"
                UPDATE public.""Epc""
                SET ""EpcHeatingType"" = ""Epc"".""OtherHeatingType"" + 8
                WHERE ""Epc"".""OtherHeatingType"" IS NOT NULL AND ""Epc"".""OtherHeatingType"" != 2");
            
            // Then copy over the regular heating type values
            migrationBuilder.Sql(@"
                UPDATE public.""Epc""
                SET ""EpcHeatingType"" = ""Epc"".""HeatingType""
                WHERE ""Epc"".""HeatingType"" IS NOT NULL");
            
            migrationBuilder.DropColumn(
                name: "HeatingType",
                table: "Epc");

            migrationBuilder.DropColumn(
                name: "OtherHeatingType",
                table: "Epc");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "HeatingType",
                table: "Epc",
                type: "integer",
                nullable: true);
            
            migrationBuilder.AddColumn<int>(
                name: "OtherHeatingType",
                table: "Epc",
                type: "integer",
                nullable: true);
            
            migrationBuilder.Sql(@"
                UPDATE public.""Epc""
                SET ""HeatingType"" = ""Epc"".""EpcHeatingType""
                WHERE ""Epc"".""EpcHeatingType"" IS NOT NULL AND ""Epc"".""EpcHeatingType"" < 8");
            
            migrationBuilder.Sql(@"
                UPDATE public.""Epc""
                SET ""OtherHeatingType"" = ""Epc"".""EpcHeatingType"" - 8
                WHERE ""Epc"".""EpcHeatingType"" IS NOT NULL AND ""Epc"".""EpcHeatingType"" >= 8");
            
            // Treating the case when HeatingType and OtherHeatingType were both 'Other' (values 7 and 2)
            migrationBuilder.Sql(@"
                UPDATE public.""Epc""
                SET ""OtherHeatingType"" = 2
                WHERE ""Epc"".""EpcHeatingType"" = 7 AND ""Epc"".""OtherHeatingType"" IS NULL");
            
            migrationBuilder.DropColumn(
                name: "EpcHeatingType",
                table: "Epc");
        }
    }
}
