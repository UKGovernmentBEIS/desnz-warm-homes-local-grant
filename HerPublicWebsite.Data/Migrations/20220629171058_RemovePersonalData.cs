using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HerPublicWebsite.Data.Migrations
{
    public partial class RemovePersonalData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EpcAddressConfirmed",
                table: "PropertyData");

            migrationBuilder.DropColumn(
                name: "EpcCount",
                table: "PropertyData");

            migrationBuilder.DropColumn(
                name: "HouseNameOrNumber",
                table: "PropertyData");

            migrationBuilder.DropColumn(
                name: "Postcode",
                table: "PropertyData");

            migrationBuilder.DropColumn(
                name: "Address1",
                table: "Epc");

            migrationBuilder.DropColumn(
                name: "Address2",
                table: "Epc");

            migrationBuilder.DropColumn(
                name: "EpcId",
                table: "Epc");

            migrationBuilder.DropColumn(
                name: "LodgementDate",
                table: "Epc");

            migrationBuilder.DropColumn(
                name: "Postcode",
                table: "Epc");

            migrationBuilder.RenameColumn(
                name: "FindEpc",
                table: "PropertyData",
                newName: "SearchForEpc");

            migrationBuilder.AddColumn<int>(
                name: "LodgementYear",
                table: "Epc",
                type: "integer",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LodgementYear",
                table: "Epc");

            migrationBuilder.RenameColumn(
                name: "SearchForEpc",
                table: "PropertyData",
                newName: "FindEpc");

            migrationBuilder.AddColumn<int>(
                name: "EpcAddressConfirmed",
                table: "PropertyData",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EpcCount",
                table: "PropertyData",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HouseNameOrNumber",
                table: "PropertyData",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Postcode",
                table: "PropertyData",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Address1",
                table: "Epc",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Address2",
                table: "Epc",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EpcId",
                table: "Epc",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LodgementDate",
                table: "Epc",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Postcode",
                table: "Epc",
                type: "text",
                nullable: true);
        }
    }
}
