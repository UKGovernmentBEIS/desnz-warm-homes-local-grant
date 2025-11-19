using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WhlgPublicWebsite.Data.Migrations
{
    public partial class AddPendingReferralFlag : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "WasSubmittedToPendingLocalAuthority",
                table: "ReferralRequests",
                type: "boolean",
                nullable: false,
                defaultValue: false);
            migrationBuilder.Sql(@"
                UPDATE ""ReferralRequests"" 
                SET ""WasSubmittedToPendingLocalAuthority"" = TRUE 
                -- It became possible for users to submit referral requests to pending local authorities with the 
                -- release of DESNZ-775 on 2024-02-27, so we only consider referral requests since then.
                -- See: https://softwiretech.atlassian.net/browse/DESNZ-775
                WHERE ""RequestDate"" >= '2024-02-27' 
                -- This is a list of custodian codes for local authorities that were pending on 2024-02-27. 
                AND ""CustodianCode"" IN (
                    '1005', '3005', '2205', '3010', '235', '4605', '2405', '2505', '335', '1510',
                    '1515', '1805', '1905', '3015', '440', '4710', '3405', '2210', '1520', '2410',
                    '1525', '1015', '1530', '4610', '1910', '1350', '1055', '4615', '1355', '1915',
                    '2510', '3410', '1535', '1025', '2250', '3020', '2230', '2615', '2415', '1540',
                    '1730', '724', '5480', '1920', '1030', '2420', '3415', '2515', '2235', '1545',
                    '1820', '3025', '2280', '2430', '435', '3030', '3420', '2002', '1925', '2520',
                    '2003', '2840', '3705', '2435', '2745', '2935', '3060', '3710', '2440', '3110',
                    '345', '728', '1825', '1550', '3715', '3040', '4620', '350', '4625', '1040',
                    '2525', '2530', '3430', '1590', '1930', '3425', '3435', '1935', '738', '3720',
                    '5870', '2255', '3445', '3240', '1560', '2260', '1595', '2270', '1570', '3725',
                    '1950', '340', '2535', '2845', '355', '360', '4635', '1835', '1840', '2741'
                )
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WasSubmittedToPendingLocalAuthority",
                table: "ReferralRequests");
        }
    }
}
