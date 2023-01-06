using SeaPublicWebsite.BusinessLogic.Models.Enums;

namespace SeaPublicWebsite.BusinessLogic.Models
{
    public class Epc
    {
        public int? LodgementYear { get; set; }
        public PropertyType? PropertyType { get; set; }
        public HouseType? HouseType { get; set; }
        public BungalowType? BungalowType { get; set; }
        public FlatType? FlatType { get; set; }
        public EpcHeatingType? EpcHeatingType { get; set; }
        public WallConstruction? WallConstruction { get; set; }
        public SolidWallsInsulated? SolidWallsInsulated { get; set; }
        public CavityWallsInsulated? CavityWallsInsulated { get; set; }
        public FloorConstruction? FloorConstruction { get; set; }
        public FloorInsulated? FloorInsulated { get; set; }
        public HomeAge? ConstructionAgeBand { get; set; }
        public RoofConstruction? RoofConstruction { get; set; }
        public RoofInsulated? RoofInsulated { get; set; }
        public GlazingType? GlazingType { get; set; }
        public HasHotWaterCylinder? HasHotWaterCylinder { get; set; }

        public bool ContainsPropertyTypeAndAge()
        {
            return ConstructionAgeBand != null
                   && ((PropertyType == Enums.PropertyType.House && HouseType != null)
                       || (PropertyType == Enums.PropertyType.Bungalow && BungalowType != null)
                       || (PropertyType == Enums.PropertyType.ApartmentFlatOrMaisonette && FlatType != null));
        }
    }
}

