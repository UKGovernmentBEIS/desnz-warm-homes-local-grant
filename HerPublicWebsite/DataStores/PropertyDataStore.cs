using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using HerPublicWebsite.BusinessLogic.Models;
using HerPublicWebsite.BusinessLogic.Models.Enums;

namespace HerPublicWebsite.DataStores;

public class PropertyDataStore
{
    private readonly ILogger<PropertyDataStore> logger;

    public PropertyDataStore(ILogger<PropertyDataStore> logger)
    {
        this.logger = logger;
    }
    
    public async Task<PropertyData> LoadPropertyDataAsync(string reference)
    {
        return new PropertyData()
        {
            Reference = "DUMMY123",
            OwnershipStatus = OwnershipStatus.OwnerOccupancy,
            Country = Country.England,
            SearchForEpc = SearchForEpc.No,

            PropertyType = PropertyType.House,
            HouseType = HouseType.Detached,

            YearBuilt = YearBuilt.From1967To1982,

            WallConstruction = WallConstruction.Cavity,
            CavityWallsInsulated = CavityWallsInsulated.All,
            FloorConstruction = FloorConstruction.SolidConcrete,
            FloorInsulated = FloorInsulated.Yes,
            RoofConstruction = RoofConstruction.Pitched,
            LoftSpace = LoftSpace.Yes,
            LoftAccess = LoftAccess.Yes,
            RoofInsulated = RoofInsulated.Yes,
            HasOutdoorSpace = HasOutdoorSpace.Yes,
            GlazingType = GlazingType.DoubleOrTripleGlazed,
            HeatingType = HeatingType.GasBoiler,
            HasHotWaterCylinder = HasHotWaterCylinder.Yes,

            NumberOfOccupants = 5,
            HeatingPattern = HeatingPattern.AllDayNotNight,
            Temperature = 20,
        };
    }

    public async Task<bool> IsReferenceValidAsync(string reference)
    {
        return true;
    }

    public async Task SavePropertyDataAsync(PropertyData propertyData)
    {
        try
        {
            // TODO Save actual data
        }
        catch (DbUpdateConcurrencyException e)
        {
            // Most likely reason for this is that the user double-clicked a form submit button so just continue,
            // the previous request should have updated the DB already.
            logger.LogWarning($"DbUpdateConcurrencyException caught. This probably means that a user double-clicked a form submit. {e.Message}");
        }
    }

    public async Task<PropertyData> CreateNewPropertyDataAsync()
    {
        return  new()
        {
            Reference = "DUMMY123"
        };
    }
}
