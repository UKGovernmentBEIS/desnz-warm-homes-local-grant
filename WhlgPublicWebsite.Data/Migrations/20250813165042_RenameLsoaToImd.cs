using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WhlgPublicWebsite.Data.Migrations
{
    /// <inheritdoc />
    public partial class RenameLsoaToImd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsLsoaProperty",
                table: "ReferralRequests",
                newName: "IsImdPostcode");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsImdPostcode",
                table: "ReferralRequests",
                newName: "IsLsoaProperty");
        }
    }
}
