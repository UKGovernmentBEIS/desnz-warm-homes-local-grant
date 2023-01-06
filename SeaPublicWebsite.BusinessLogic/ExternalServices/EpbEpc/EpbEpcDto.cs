using System.Text.RegularExpressions;
using Newtonsoft.Json;
using SeaPublicWebsite.BusinessLogic.Models;
using SeaPublicWebsite.BusinessLogic.Models.Enums;

namespace SeaPublicWebsite.BusinessLogic.ExternalServices.EpbEpc;

public class EpbEpcDto
{
    [JsonProperty(PropertyName = "data")]
    public EpbEpcDataDto Data { get; set; }
}

public class EpbEpcDataDto
{
    [JsonProperty(PropertyName = "assessment")]
    public EpbEpcAssessmentDto Assessment { get; set; }
}

public class EpbEpcAssessmentDto
{
    [JsonProperty(PropertyName = "typeOfAssessment")]
    public string AssessmentType { get; set; }
    
    [JsonProperty(PropertyName = "address")]
    public EpbAddressDto Address { get; set; }
    
    [JsonProperty(PropertyName = "lodgementDate")]
    public string LodgementDate { get; set; }
    
    [JsonProperty(PropertyName = "isLatestAssessmentForAddress")]
    public bool IsLatestAssessmentForAddress { get; set; }
    
    [JsonProperty(PropertyName = "propertyType")]
    public string PropertyType { get; set; }
    
    [JsonProperty(PropertyName = "builtForm")]
    public string BuiltForm { get; set; }
    
    [JsonProperty(PropertyName = "PropertyAgeBand")]
    public string PropertyAgeBand { get; set; }

    [JsonProperty(PropertyName = "wallsDescription")]
    public List<string> WallsDescription { get; set; }
    
    [JsonProperty(PropertyName = "floorDescription")]
    public List<string> FloorDescription { get; set; }
    
    [JsonProperty(PropertyName = "roofDescription")]
    public List<string> RoofDescription { get; set; }
    
    [JsonProperty(PropertyName = "windowsDescription")]
    public List<string> WindowsDescription { get; set; }
    
    [JsonProperty(PropertyName = "mainHeatingDescription")]
    public string MainHeatingDescription { get; set; }
    
    [JsonProperty(PropertyName = "mainFuelType")]
    public string MainFuelType { get; set; }
    
    [JsonProperty(PropertyName = "hasHotWaterCylinder")]
    public bool? HasHotWaterCylinder { get; set; }

    public Epc Parse()
    {
        if (AssessmentType.Equals("SAP", StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }
            
        return new Epc
        {
            LodgementYear = ParseLodgementDate()?.Year,
            ConstructionAgeBand = ParseConstructionAgeBand(),
            PropertyType = ParsePropertyType(),
            HouseType = ParseHouseType(),
            BungalowType = ParseBungalowType(),
            FlatType = ParseFlatType(),
            WallConstruction = ParseWallConstruction(),
            SolidWallsInsulated = ParseSolidWallsInsulated(),
            CavityWallsInsulated = ParseCavityWallsInsulated(),
            FloorConstruction = ParseFloorConstruction(),
            FloorInsulated = ParseFloorInsulation(),
            RoofConstruction = ParseRoofConstruction(),
            RoofInsulated = ParseRoofInsulation(),
            GlazingType = ParseGlazingType(),
            EpcHeatingType = ParseHeatingType(),
            HasHotWaterCylinder = ParseHasHotWaterCylinder()
        };
    }

    private DateTime? ParseLodgementDate()
    {
        if (LodgementDate is null)
        {
            return null;
        }
        
        var date = DateTime.Parse(LodgementDate);
        return DateTime.SpecifyKind(date, DateTimeKind.Utc);
    }
    
    private HomeAge? ParseConstructionAgeBand()
    {
        if (PropertyAgeBand is null)
        {
            return null;
        }

        if (PropertyAgeBand.Equals("A", StringComparison.OrdinalIgnoreCase) ||
            PropertyAgeBand.Contains("before 1900"))
        {
            return HomeAge.Pre1900;
        }
        
        if (PropertyAgeBand.Equals("B", StringComparison.OrdinalIgnoreCase) ||
            PropertyAgeBand.Contains("1900-1929"))
        {
            return HomeAge.From1900To1929;
        }
        
        if (PropertyAgeBand.Equals("C", StringComparison.OrdinalIgnoreCase) ||
            PropertyAgeBand.Contains("1930-1949"))
        {
            return HomeAge.From1930To1949;
        }
        
        if (PropertyAgeBand.Equals("D", StringComparison.OrdinalIgnoreCase) ||
            PropertyAgeBand.Contains("1950-1966"))
        {
            return HomeAge.From1950To1966;
        }
        
        if (PropertyAgeBand.Equals("E", StringComparison.OrdinalIgnoreCase) ||
            PropertyAgeBand.Contains("1967-1975"))
        {
            return HomeAge.From1967To1975;
        }
        
        if (PropertyAgeBand.Equals("F", StringComparison.OrdinalIgnoreCase) ||
            PropertyAgeBand.Contains("1976-1982"))
        {
            return HomeAge.From1976To1982;
        }
        
        if (PropertyAgeBand.Equals("G", StringComparison.OrdinalIgnoreCase) ||
            PropertyAgeBand.Contains("1983-1990"))
        {
            return HomeAge.From1983To1990;
        }
        
        if (PropertyAgeBand.Equals("H", StringComparison.OrdinalIgnoreCase) ||
            PropertyAgeBand.Contains("1991-1995"))
        {
            return HomeAge.From1991To1995;
        }
        
        if (PropertyAgeBand.Equals("I", StringComparison.OrdinalIgnoreCase) ||
            PropertyAgeBand.Contains("1996-2002"))
        {
            return HomeAge.From1996To2002;
        }
        
        if (PropertyAgeBand.Equals("J", StringComparison.OrdinalIgnoreCase) ||
            PropertyAgeBand.Contains("2003-2006"))
        {
            return HomeAge.From2003To2006;
        }
        
        if (PropertyAgeBand.Equals("K", StringComparison.OrdinalIgnoreCase) ||
            PropertyAgeBand.Contains("2007-2011"))
        {
            return HomeAge.From2007To2011;
        }
        
        if (PropertyAgeBand.Equals("L", StringComparison.OrdinalIgnoreCase) ||
            PropertyAgeBand.Contains("2012 onwards"))
        {
            return HomeAge.From2012ToPresent;
        }

        return null;
    }

    private PropertyType? ParsePropertyType()
    {
        if (PropertyType is null)
        {
            return null;
        }
        
        if (PropertyType.Contains("House", StringComparison.OrdinalIgnoreCase))
        {
            return SeaPublicWebsite.BusinessLogic.Models.Enums.PropertyType.House;
        }

        if (PropertyType.Contains("Bungalow", StringComparison.OrdinalIgnoreCase))
        {
            return SeaPublicWebsite.BusinessLogic.Models.Enums.PropertyType.Bungalow;
        }

        if (PropertyType.Contains("Flat", StringComparison.OrdinalIgnoreCase) ||
            PropertyType.Contains("Maisonette", StringComparison.OrdinalIgnoreCase))
        {
            return SeaPublicWebsite.BusinessLogic.Models.Enums.PropertyType.ApartmentFlatOrMaisonette;
        }

        return null;
    }
    
    private HouseType? ParseHouseType()
    {
        if (ParsePropertyType() is not SeaPublicWebsite.BusinessLogic.Models.Enums.PropertyType.House)
        {
            return null;
        }

        if (PropertyType.Contains("semi-detached", StringComparison.OrdinalIgnoreCase))
        {
            return HouseType.SemiDetached;
        }
        
        if (PropertyType.Contains("detached", StringComparison.OrdinalIgnoreCase))
        {
            return HouseType.Detached;
        }

        if (PropertyType.Contains("mid-terrace", StringComparison.OrdinalIgnoreCase))
        {
            return HouseType.Terraced;
        }
        
        if (PropertyType.Contains("end-terrace", StringComparison.OrdinalIgnoreCase))
        {
            return HouseType.EndTerrace;
        }

        return null;
    }
    
    private BungalowType? ParseBungalowType()
    {
        if (ParsePropertyType() is not SeaPublicWebsite.BusinessLogic.Models.Enums.PropertyType.Bungalow)
        {
            return null;
        }

        if (PropertyType.Contains("semi-detached", StringComparison.OrdinalIgnoreCase))
        {
            return BungalowType.SemiDetached;
        }
        
        if (PropertyType.Contains("detached", StringComparison.OrdinalIgnoreCase))
        {
            return BungalowType.Detached;
        }

        if (PropertyType.Contains("mid-terrace", StringComparison.OrdinalIgnoreCase))
        {
            return BungalowType.Terraced;
        }
        
        if (PropertyType.Contains("end-terrace", StringComparison.OrdinalIgnoreCase))
        {
            return BungalowType.EndTerrace;
        }

        return null;
    }
    
    private FlatType? ParseFlatType()
    {
        if (ParsePropertyType() is not SeaPublicWebsite.BusinessLogic.Models.Enums.PropertyType.ApartmentFlatOrMaisonette)
        {
            return null;
        }
        
        if (PropertyType.Contains("basement", StringComparison.OrdinalIgnoreCase) ||
            PropertyType.Contains("ground", StringComparison.OrdinalIgnoreCase))
        {
            return FlatType.GroundFloor;
        }

        if (PropertyType.Contains("mid", StringComparison.OrdinalIgnoreCase))
        {
            return FlatType.MiddleFloor;
        }
        
        if (PropertyType.Contains("top", StringComparison.OrdinalIgnoreCase))
        {
            return FlatType.TopFloor;
        }

        return null;
    }

    private static bool HasSolidWalls(string description)
    {
        return description.Contains("solid", StringComparison.OrdinalIgnoreCase) ||
               description.Contains("granite", StringComparison.OrdinalIgnoreCase) ||
               description.Contains("whinstone", StringComparison.OrdinalIgnoreCase) ||
               description.Contains("sandstone", StringComparison.OrdinalIgnoreCase) ||
               description.Contains("limestone", StringComparison.OrdinalIgnoreCase);
    }

    private WallConstruction? ParseWallConstruction()
    {
        if (WallsDescription is null)
        {
            return null;
        }
        
        var hasOther = WallsDescription.Any(description =>
            description.Contains("System built", StringComparison.OrdinalIgnoreCase) ||
            description.Contains("Cob", StringComparison.OrdinalIgnoreCase) ||
            description.Contains("Timber frame", StringComparison.OrdinalIgnoreCase) ||
            description.Contains("Park home wall", StringComparison.OrdinalIgnoreCase));

        if (hasOther)
        {
            return WallConstruction.Other;
        }

        var hasCavity = WallsDescription.Any(description => 
            description.Contains("cavity", StringComparison.OrdinalIgnoreCase));
        var hasSolid = WallsDescription.Any(HasSolidWalls);

        return (hasCavity, hasSolid) switch
        {
            (true, true) => WallConstruction.Mixed,
            (true, false) => WallConstruction.Cavity,
            (false, true) => WallConstruction.Solid,
            (false, false) => null
        };
    }
    
    private SolidWallsInsulated? ParseSolidWallsInsulated()
    {
        if (WallsDescription is null)
        {
            return null;
        }

        if (WallsDescription.Any(description =>
                HasSolidWalls(description) &&
                (description.Contains("insulated", StringComparison.OrdinalIgnoreCase) ||
                 description.Contains("internal insulation", StringComparison.OrdinalIgnoreCase) ||
                 description.Contains("external insulation", StringComparison.OrdinalIgnoreCase))))
        {
            return SolidWallsInsulated.All;
        }
        
        if (WallsDescription.Any(description =>
                HasSolidWalls(description) &&
                description.Contains("partial insulation", StringComparison.OrdinalIgnoreCase)))
        {
            return SolidWallsInsulated.Some;
        }
        
        if (WallsDescription.Any(description =>
                HasSolidWalls(description) &&
                description.Contains("no insulation", StringComparison.OrdinalIgnoreCase)))
        {
            return SolidWallsInsulated.No;
        }

        return null;
    }

    private CavityWallsInsulated? ParseCavityWallsInsulated()
    {
        if (WallsDescription is null)
        {
            return null;
        }

        if (WallsDescription.Any(description =>
                description.Contains("cavity", StringComparison.OrdinalIgnoreCase) &&
                (description.Contains("insulated", StringComparison.OrdinalIgnoreCase) ||
                 description.Contains("internal insulation", StringComparison.OrdinalIgnoreCase) ||
                 description.Contains("external insulation", StringComparison.OrdinalIgnoreCase) ||
                 description.Contains("filled cavity", StringComparison.OrdinalIgnoreCase))))
        {
            return CavityWallsInsulated.All;
        }
        
        if (WallsDescription.Any(description =>
                description.Contains("cavity", StringComparison.OrdinalIgnoreCase) &&
                description.Contains("partial insulation", StringComparison.OrdinalIgnoreCase)))
        {
            return CavityWallsInsulated.Some;
        }
        
        if (WallsDescription.Any(description =>
                description.Contains("cavity", StringComparison.OrdinalIgnoreCase) &&
                description.Contains("no insulation", StringComparison.OrdinalIgnoreCase)))
        {
            return CavityWallsInsulated.No;
        }

        return null;
    }

    private FloorConstruction? ParseFloorConstruction()
    {
        if (FloorDescription is null)
        {
            return null;
        }

        var hasSolid = FloorDescription.Any(description =>
            description.Contains("solid", StringComparison.OrdinalIgnoreCase));
        var hasSuspended = FloorDescription.Any(description =>
            description.Contains("suspended", StringComparison.OrdinalIgnoreCase));

        return (hasSolid, hasSuspended) switch
        {
            (true, true) => FloorConstruction.Mix,
            (true, false) => FloorConstruction.SolidConcrete,
            (false, true) => FloorConstruction.SuspendedTimber,
            (false, false) => null
        };
    }

    private FloorInsulated? ParseFloorInsulation()
    {
        if (FloorDescription is null)
        {
            return null;
        }

        if (FloorDescription.All(description =>
                description.Contains("insulated", StringComparison.OrdinalIgnoreCase) ||
                description.Contains("limited", StringComparison.OrdinalIgnoreCase)
                ))
        {
            return FloorInsulated.Yes;
        }
        
        if (FloorDescription.Any(description =>
                description.Contains("no insulation", StringComparison.OrdinalIgnoreCase)))
        {
            return FloorInsulated.No;
        }

        return null;
    }

    private RoofConstruction? ParseRoofConstruction()
    {
        if (RoofDescription is null)
        {
            return null;
        }

        var hasFlat = RoofDescription.Any(description =>
            description.Contains("flat", StringComparison.OrdinalIgnoreCase) ||
            description.Contains("thatched", StringComparison.OrdinalIgnoreCase));
        var hasPitched = RoofDescription.Any(description =>
            description.Contains("pitched", StringComparison.OrdinalIgnoreCase));

        return (hasFlat, hasPitched) switch
        {
            (true, true) => RoofConstruction.Mixed,
            (true, false) => RoofConstruction.Flat,
            (false, true) => RoofConstruction.Pitched,
            (false, false) => null
        };
    }

    private RoofInsulated? ParseRoofInsulation()
    {
        if (RoofDescription is null)
        {
            return null;
        }

        if (RoofDescription.Any(description =>
            {
                if (description.Contains("no insulation", StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }

                // Extract number xxx from 'xxx mm loft insulation'
                var thickness = Regex.Match(description, @"\d+");
                return thickness.Success && int.Parse(thickness.Value) < 200;
            }))
        {
            return RoofInsulated.No;
        }
            
        if (RoofDescription.All(description =>
            {
                if (description.Contains("limited", StringComparison.OrdinalIgnoreCase) ||
                    description.Contains("insulated", StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }

                // Extract number xxx from 'xxx mm loft insulation'
                var thickness = Regex.Match(description, @"\d+");
                return thickness.Success && int.Parse(thickness.Value) >= 200;
            }))
        {
            return RoofInsulated.Yes;
        }

        return null;
    }

    private GlazingType? ParseGlazingType()
    {
        if (WindowsDescription is null)
        {
            return null;
        }
        
        var hasSingle = WindowsDescription.Any(description =>
            description.Contains("single", StringComparison.OrdinalIgnoreCase) ||
            description.Contains("some", StringComparison.OrdinalIgnoreCase) ||
            description.Contains("partial", StringComparison.OrdinalIgnoreCase) ||
            description.Contains("mostly", StringComparison.OrdinalIgnoreCase) ||
            description.Contains("multiple glazing throughout", StringComparison.OrdinalIgnoreCase));
        
        var hasDoubleOrTriple = WindowsDescription.Any(description =>
            description.Contains("some", StringComparison.OrdinalIgnoreCase) ||
            description.Contains("partial", StringComparison.OrdinalIgnoreCase) ||
            description.Contains("mostly", StringComparison.OrdinalIgnoreCase) ||
            description.Contains("full", StringComparison.OrdinalIgnoreCase) ||
            description.Contains("high", StringComparison.OrdinalIgnoreCase) ||
            description.Contains("multiple glazing throughout", StringComparison.OrdinalIgnoreCase));
        
        return (hasSingle, hasDoubleOrTriple) switch
        {
            (true, true) => GlazingType.Both,
            (true, false) => GlazingType.SingleGlazed,
            (false, true) => GlazingType.DoubleOrTripleGlazed,
            (false, false) => null
        };
    }

    private EpcHeatingType? ParseHeatingType()
    {
        if (MainFuelType is null)
        {
            return null;
        }
        
        // Anything ending in '(community)'
        // Communal heating is treated as 'other heating'
        if (
            MainFuelType.Equals("20") ||
            MainFuelType.Equals("21") ||
            MainFuelType.Equals("22") ||
            MainFuelType.Equals("23") ||
            MainFuelType.Equals("25") ||
            MainFuelType.Equals("30") ||
            MainFuelType.Equals("31") ||
            MainFuelType.Equals("32") ||
            MainFuelType.Equals("56") ||
            MainFuelType.Equals("57") ||
            MainFuelType.Equals("58") ||
            MainFuelType.Equals("99") ||
            MainFuelType.Contains("(community)", StringComparison.OrdinalIgnoreCase))
        {
            return EpcHeatingType.Other;
        }
        
        // 19 - bioethanol
        // 34, 35, 36 - biodiesel from ...
        // Biodiesel and bioethanol is treated as 'other heating'
        if (
            MainFuelType.Equals("19") ||
            MainFuelType.Equals("34") ||
            MainFuelType.Equals("35") ||
            MainFuelType.Equals("36") ||
            MainFuelType.Contains("bioethanol", StringComparison.OrdinalIgnoreCase) ||
            MainFuelType.Contains("biodiesel", StringComparison.OrdinalIgnoreCase))
        {
            return EpcHeatingType.Other;
        }
        
        // 11 - waste combustion
        // Treated as 'other heating'
        if (MainFuelType.Equals("11") ||
            MainFuelType.Contains("waste combustion", StringComparison.OrdinalIgnoreCase))
        {
            return EpcHeatingType.Other;
        }
        
        // 16 - wood pellets in bags for secondary heating
        // Treated as 'other heating'
        if (MainFuelType.Equals("16") ||
            MainFuelType.Contains("wood pellets in bags for secondary heating", StringComparison.OrdinalIgnoreCase))
        {
            return EpcHeatingType.Other;
        }

        // Gas boiler check
        // 1, 26 - mains gas ...
        // 13, 51 - biogas ...
        if (MainFuelType.Equals("1") ||
            MainFuelType.Equals("13") ||
            MainFuelType.Equals("26") ||
            MainFuelType.Equals("51") ||
            MainFuelType.Contains("mains gas", StringComparison.OrdinalIgnoreCase) ||
            MainFuelType.Contains("biogas", StringComparison.OrdinalIgnoreCase)
           )
        {
            return EpcHeatingType.GasBoiler;
        }
        
        // Lpg boiler check
        // 3 - bottled LPG
        // 2, 17, 27 - LPG ...
        if (MainFuelType.Equals("2") ||
            MainFuelType.Equals("3") ||
            MainFuelType.Equals("17") ||
            MainFuelType.Equals("27") ||
            MainFuelType.Contains("LPG", StringComparison.OrdinalIgnoreCase))
        {
            return EpcHeatingType.LpgBoiler;
        }
        
        // Oil boiler check
        // 4 - oil - this is for backwards compatibility only and should not be used
        // 18 - B30K (not community)
        // 28 - oil (not community)
        // 37 - appliances able to use mineral oil or liquid biofuel
        if (MainFuelType.Equals("4") ||
            MainFuelType.Equals("18") ||
            MainFuelType.Equals("28") ||
            MainFuelType.Equals("37") ||
            MainFuelType.Contains("oil", StringComparison.OrdinalIgnoreCase) ||
            MainFuelType.Contains("B30K", StringComparison.OrdinalIgnoreCase))
        {
            return EpcHeatingType.OilBoiler;
        }
        
        // coal check
        // 5 - anthracite
        // 14 - house coal
        // 15 - smokeless coal
        // 33 - house coal (not community)
        if (MainFuelType.Equals("5") ||
            MainFuelType.Equals("14") ||
            MainFuelType.Equals("15") ||
            MainFuelType.Equals("33") ||
            MainFuelType.Contains("coal", StringComparison.OrdinalIgnoreCase) ||
            MainFuelType.Contains("anthracite", StringComparison.OrdinalIgnoreCase))
        {
            return EpcHeatingType.CoalOrSolidFuel;
        }
        
        // biomass boiler check
        // 6 - wood logs
        // 7 - bulk wood pellets
        // 8 - wood chips
        // 9 - dual fuel - mineral + wood
        // 12 - biomass
        if (MainFuelType.Equals("6") || 
            MainFuelType.Equals("7") ||
            MainFuelType.Equals("8") ||
            MainFuelType.Equals("9") ||
            MainFuelType.Equals("12") ||
            MainFuelType.Contains("wood logs", StringComparison.OrdinalIgnoreCase) ||
            MainFuelType.Contains("bulk wood pellets", StringComparison.OrdinalIgnoreCase) ||
            MainFuelType.Contains("wood chips", StringComparison.OrdinalIgnoreCase) ||
            MainFuelType.Contains("dual fuel - mineral + wood", StringComparison.OrdinalIgnoreCase) ||
            MainFuelType.Contains("biomass", StringComparison.OrdinalIgnoreCase))
        {
            return EpcHeatingType.Biomass;
        }

        // electric heating check
        // 10 - electricity
        // 29 - electricity (not community)
        if (MainFuelType.Equals("10") ||
            MainFuelType.Equals("29") ||
            MainFuelType.Contains("electricity", StringComparison.OrdinalIgnoreCase))
        {
            // We need to inspect the main heating description to decide
            
            if (MainHeatingDescription is null)
            {
                return null;
            }
            
            // Communal heating is treated as 'other heating'
            // 6 - community heating system
            if (MainHeatingDescription.Equals("6") ||
                MainHeatingDescription.Contains("community", StringComparison.OrdinalIgnoreCase))
            {
                return EpcHeatingType.Other;
            }
            
            // Direct action electric check
            // 2 - boiler with radiators or underfloor heating
            // 8 - electric underfloor heating
            // 9 - warm air system (not heat pump)
            // 10 - room heaters
            // 11 - other system
            if (MainHeatingDescription.Equals("2") ||
                MainHeatingDescription.Equals("8") ||
                MainHeatingDescription.Equals("9") ||
                MainHeatingDescription.Equals("10") ||
                MainHeatingDescription.Equals("11") ||
                MainHeatingDescription.Contains("boiler with radiators or underfloor heating", StringComparison.OrdinalIgnoreCase) ||
                MainHeatingDescription.Contains("electric underfloor heating", StringComparison.OrdinalIgnoreCase) ||
                MainHeatingDescription.Contains("warm air system (not heat pump)", StringComparison.OrdinalIgnoreCase) ||
                MainHeatingDescription.Contains("room heaters", StringComparison.OrdinalIgnoreCase) ||
                MainHeatingDescription.Contains("other system", StringComparison.OrdinalIgnoreCase))
            {
                return EpcHeatingType.DirectActionElectric;
            }
            
            // Heat pump check
            // 4 - heat pump with radiators or underfloor heating
            // 5 - heat pump with warm air distribution
            if (MainHeatingDescription.Equals("4") ||
                MainHeatingDescription.Equals("5") ||
                MainHeatingDescription.Contains("heat pump with", StringComparison.OrdinalIgnoreCase))
            {
                return EpcHeatingType.HeatPump;
            }
            
            // Storage heater check
            // 7 - electric storage heaters
            if (MainHeatingDescription.Equals("7") ||
                MainHeatingDescription.Contains("electric storage heaters", StringComparison.OrdinalIgnoreCase))
            {
                return EpcHeatingType.Storage;
            }
            
            // Special case of micro combined heat and power
            // 3 - micro-cogeneration
            if (MainHeatingDescription.Equals("3") ||
                MainHeatingDescription.Contains("micro-cogeneration", StringComparison.OrdinalIgnoreCase))
            {
                return EpcHeatingType.GasBoiler;
            }
        }
        
        return null;
    }

    private HasHotWaterCylinder? ParseHasHotWaterCylinder()
    {
        if (HasHotWaterCylinder is null)
        {
            return null;
        }

        return HasHotWaterCylinder.Value
            ? SeaPublicWebsite.BusinessLogic.Models.Enums.HasHotWaterCylinder.Yes
            : SeaPublicWebsite.BusinessLogic.Models.Enums.HasHotWaterCylinder.No;
    }


}
