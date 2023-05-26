using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HerPublicWebsite.Data.Migrations
{
    public partial class UpdateContactDetails : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConsentedToAnswerEmail",
                table: "ContactDetails");

            migrationBuilder.DropColumn(
                name: "ConsentedToReferral",
                table: "ContactDetails");

            migrationBuilder.DropColumn(
                name: "ConsentedToSchemeNotificationEmails",
                table: "ContactDetails");

            migrationBuilder.DropColumn(
                name: "ContactPreference",
                table: "ContactDetails");

            migrationBuilder.RenameColumn(
                name: "ReferralCreated",
                table: "ReferralRequests",
                newName: "ReferralWrittenToCsv");

            migrationBuilder.RenameColumn(
                name: "Telephone",
                table: "ContactDetails",
                newName: "LaContactTelephone");

            migrationBuilder.RenameColumn(
                name: "Email",
                table: "ContactDetails",
                newName: "LaContactEmailAddress");

            migrationBuilder.AlterColumn<DateTime>(
                name: "RequestDate",
                table: "ReferralRequests",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ReferralWrittenToCsv",
                table: "ReferralRequests",
                newName: "ReferralCreated");

            migrationBuilder.RenameColumn(
                name: "LaContactTelephone",
                table: "ContactDetails",
                newName: "Telephone");

            migrationBuilder.RenameColumn(
                name: "LaContactEmailAddress",
                table: "ContactDetails",
                newName: "Email");

            migrationBuilder.AlterColumn<DateTime>(
                name: "RequestDate",
                table: "ReferralRequests",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AddColumn<bool>(
                name: "ConsentedToAnswerEmail",
                table: "ContactDetails",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ConsentedToReferral",
                table: "ContactDetails",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ConsentedToSchemeNotificationEmails",
                table: "ContactDetails",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "ContactPreference",
                table: "ContactDetails",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
