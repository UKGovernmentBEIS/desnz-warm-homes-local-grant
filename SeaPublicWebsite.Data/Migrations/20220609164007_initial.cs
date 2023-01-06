using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SeaPublicWebsite.Data.Migrations
{
    public partial class initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Epc",
                columns: table => new
                {
                    EpcId = table.Column<string>(type: "text", nullable: false),
                    Address1 = table.Column<string>(type: "text", nullable: true),
                    Address2 = table.Column<string>(type: "text", nullable: true),
                    Postcode = table.Column<string>(type: "text", nullable: true),
                    BuildingReference = table.Column<string>(type: "text", nullable: true),
                    InspectionDate = table.Column<string>(type: "text", nullable: true),
                    PropertyType = table.Column<int>(type: "integer", nullable: true),
                    HeatingType = table.Column<int>(type: "integer", nullable: true),
                    WallConstruction = table.Column<int>(type: "integer", nullable: true),
                    SolidWallsInsulated = table.Column<int>(type: "integer", nullable: true),
                    CavityWallsInsulated = table.Column<int>(type: "integer", nullable: true),
                    FloorConstruction = table.Column<int>(type: "integer", nullable: true),
                    FloorInsulated = table.Column<int>(type: "integer", nullable: true),
                    ConstructionAgeBand = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Epc", x => x.EpcId);
                });

            migrationBuilder.CreateTable(
                name: "PropertyData",
                columns: table => new
                {
                    PropertyDataId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Reference = table.Column<string>(type: "text", nullable: true),
                    OwnershipStatus = table.Column<int>(type: "integer", nullable: true),
                    Country = table.Column<int>(type: "integer", nullable: true),
                    EpcId = table.Column<string>(type: "text", nullable: true),
                    Postcode = table.Column<string>(type: "text", nullable: true),
                    EpcLmkKey = table.Column<string>(type: "text", nullable: true),
                    HouseNameOrNumber = table.Column<string>(type: "text", nullable: true),
                    PropertyType = table.Column<int>(type: "integer", nullable: true),
                    HouseType = table.Column<int>(type: "integer", nullable: true),
                    BungalowType = table.Column<int>(type: "integer", nullable: true),
                    FlatType = table.Column<int>(type: "integer", nullable: true),
                    YearBuilt = table.Column<int>(type: "integer", nullable: true),
                    WallConstruction = table.Column<int>(type: "integer", nullable: true),
                    CavityWallsInsulated = table.Column<int>(type: "integer", nullable: true),
                    SolidWallsInsulated = table.Column<int>(type: "integer", nullable: true),
                    FloorConstruction = table.Column<int>(type: "integer", nullable: true),
                    FloorInsulated = table.Column<int>(type: "integer", nullable: true),
                    RoofConstruction = table.Column<int>(type: "integer", nullable: true),
                    AccessibleLoftSpace = table.Column<int>(type: "integer", nullable: true),
                    RoofInsulated = table.Column<int>(type: "integer", nullable: true),
                    HasOutdoorSpace = table.Column<int>(type: "integer", nullable: true),
                    GlazingType = table.Column<int>(type: "integer", nullable: true),
                    HeatingType = table.Column<int>(type: "integer", nullable: true),
                    OtherHeatingType = table.Column<int>(type: "integer", nullable: true),
                    HasHotWaterCylinder = table.Column<int>(type: "integer", nullable: true),
                    NumberOfOccupants = table.Column<int>(type: "integer", nullable: true),
                    HeatingPattern = table.Column<int>(type: "integer", nullable: true),
                    HoursOfHeatingMorning = table.Column<int>(type: "integer", nullable: true),
                    HoursOfHeatingEvening = table.Column<int>(type: "integer", nullable: true),
                    Temperature = table.Column<decimal>(type: "numeric", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropertyData", x => x.PropertyDataId);
                    table.ForeignKey(
                        name: "FK_PropertyData_Epc_EpcId",
                        column: x => x.EpcId,
                        principalTable: "Epc",
                        principalColumn: "EpcId");
                });

            migrationBuilder.CreateTable(
                name: "PropertyRecommendations",
                columns: table => new
                {
                    PropertyRecommendationId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Key = table.Column<int>(type: "integer", nullable: false),
                    MinInstallCost = table.Column<int>(type: "integer", nullable: false),
                    MaxInstallCost = table.Column<int>(type: "integer", nullable: false),
                    Saving = table.Column<int>(type: "integer", nullable: false),
                    LifetimeSaving = table.Column<int>(type: "integer", nullable: false),
                    Lifetime = table.Column<int>(type: "integer", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: true),
                    Summary = table.Column<string>(type: "text", nullable: true),
                    RecommendationAction = table.Column<int>(type: "integer", nullable: true),
                    PropertyDataId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropertyRecommendations", x => x.PropertyRecommendationId);
                    table.ForeignKey(
                        name: "FK_PropertyRecommendations_PropertyData_PropertyDataId",
                        column: x => x.PropertyDataId,
                        principalTable: "PropertyData",
                        principalColumn: "PropertyDataId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PropertyData_EpcId",
                table: "PropertyData",
                column: "EpcId");

            migrationBuilder.CreateIndex(
                name: "IX_PropertyRecommendations_PropertyDataId",
                table: "PropertyRecommendations",
                column: "PropertyDataId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PropertyRecommendations");

            migrationBuilder.DropTable(
                name: "PropertyData");

            migrationBuilder.DropTable(
                name: "Epc");
        }
    }
}
