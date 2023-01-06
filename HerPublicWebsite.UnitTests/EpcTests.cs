using System;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using HerPublicWebsite.BusinessLogic.ExternalServices.EpbEpc;
using HerPublicWebsite.BusinessLogic.Models;
using HerPublicWebsite.BusinessLogic.Models.Enums;

namespace Tests;

public class EpcTests
{
    [TestCaseSource(nameof(EpcParseTestCases))]
    public void CanParseEpcDto(EpcTestCase testCase)
    {
        // Act
        var epc = testCase.Input.Parse();
        
        // Assert
        epc.Should().BeEquivalentTo(testCase.Output);
    }

    private static readonly EpcTestCase[] AssessmentTypeTestCases =
        new (string Descrption, string inputAssessmentType, Epc outputEpc)[]
        {
            ("Can parse an RdSAP assessment", "RdSAP", new Epc()),
            ("Does not parse a SAP assessment", "SAP", null)
        }
            .Select(p => new EpcTestCase(
                p.Descrption, 
                new EpbEpcAssessmentDto 
                { 
                    AssessmentType = p.inputAssessmentType 
                }, 
                p.outputEpc))
            .ToArray();
    
    private static readonly EpcTestCase[] LodgementDateTestCases =
        new (string Descrption, string inputLodgementDate, int? outputLodgementYear)[]
            {
                ("Can handle null lodgement year", null, null),
                ("Can parse lodgement year", "2012-12-22", 2012),
            }
            .Select(p => new EpcTestCase(
                p.Descrption, 
                new EpbEpcAssessmentDto 
                { 
                    AssessmentType = "RdSAP",
                    LodgementDate = p.inputLodgementDate
                }, 
                new Epc
                {
                    LodgementYear = p.outputLodgementYear
                }))
            .ToArray();
    
    private static readonly EpcTestCase[] ConstructionAgeBandTestCases =
        new (string Descrption, string inputPropertyAgeBand, HomeAge? outputConstructionAgeBand)[]
            {
                ("Can handle null home age", null, null),
                ("Can parse home age before 1900", "before 1900", HomeAge.Pre1900),
                ("Can parse home age band A", "A", HomeAge.Pre1900),
                ("Can parse home age 1900-1929", "1900-1929", HomeAge.From1900To1929),
                ("Can parse home age band B", "B", HomeAge.From1900To1929),
                ("Can parse home age 1930-1949", "1930-1949", HomeAge.From1930To1949),
                ("Can parse home age band C", "C", HomeAge.From1930To1949),
                ("Can parse home age 1950-1966", "1950-1966", HomeAge.From1950To1966),
                ("Can parse home age band D", "D", HomeAge.From1950To1966),
                ("Can parse home age 1967-1975", "1967-1975", HomeAge.From1967To1975),
                ("Can parse home age band E", "E", HomeAge.From1967To1975),
                ("Can parse home age 1976-1982", "1976-1982", HomeAge.From1976To1982),
                ("Can parse home age band F", "F", HomeAge.From1976To1982),
                ("Can parse home age 1983-1990", "1983-1990", HomeAge.From1983To1990),
                ("Can parse home age band G", "G", HomeAge.From1983To1990),
                ("Can parse home age 1991-1995", "1991-1995", HomeAge.From1991To1995),
                ("Can parse home age band H", "H", HomeAge.From1991To1995),
                ("Can parse home age 1996-2002", "1996-2002", HomeAge.From1996To2002),
                ("Can parse home age band I", "I", HomeAge.From1996To2002),
                ("Can parse home age 2003-2006", "2003-2006", HomeAge.From2003To2006),
                ("Can parse home age band J", "J", HomeAge.From2003To2006),
                ("Can parse home age 2007-2011", "2007-2011", HomeAge.From2007To2011),
                ("Can parse home age band K", "K", HomeAge.From2007To2011),
                ("Can parse home age 2012 onwards", "2012 onwards", HomeAge.From2012ToPresent),
                ("Can parse home age band L", "L", HomeAge.From2012ToPresent),
            }
            .Select(p => new EpcTestCase(
                p.Descrption, 
                new EpbEpcAssessmentDto 
                { 
                    AssessmentType = "RdSAP",
                    PropertyAgeBand = p.inputPropertyAgeBand
                }, 
                new Epc
                {
                    ConstructionAgeBand = p.outputConstructionAgeBand
                }))
            .ToArray();
    
    private static readonly EpcTestCase[] PropertyTypeTestCases =
        new (string Descrption, string inputPropertyType, PropertyType? outputPropertyType)[]
            {
                ("Can handle null property type", null, null),
                ("Can parse property type house", "house", PropertyType.House),
                ("Can parse property type bungalow", "bungalow", PropertyType.Bungalow),
                ("Can parse property type flat", "flat", PropertyType.ApartmentFlatOrMaisonette),
                ("Can parse property type maisonette", "maisonette", PropertyType.ApartmentFlatOrMaisonette),
            }
            .Select(p => new EpcTestCase(
                p.Descrption, 
                new EpbEpcAssessmentDto 
                { 
                    AssessmentType = "RdSAP",
                    PropertyType = p.inputPropertyType
                }, 
                new Epc
                {
                    PropertyType = p.outputPropertyType
                }))
            .ToArray();
    
    private static readonly EpcTestCase[] HouseTypeTestCases =
        new (string Descrption, string inputPropertyType, HouseType? outputHouseType)[]
            {
                ("Can parse house type detached", "detached house", HouseType.Detached),
                ("Can parse house type semi-detached", "semi-detached house", HouseType.SemiDetached),
                ("Can parse house type mid-terrace", "mid-terrace house", HouseType.Terraced),
                ("Can parse house type end terrace", "end-terrace house", HouseType.EndTerrace),
            }
            .Select(p => new EpcTestCase(
                p.Descrption, 
                new EpbEpcAssessmentDto 
                { 
                    AssessmentType = "RdSAP",
                    PropertyType = p.inputPropertyType
                }, 
                new Epc
                {
                    PropertyType = PropertyType.House,
                    HouseType = p.outputHouseType
                }))
            .ToArray();
    
    private static readonly EpcTestCase[] BungalowTypeTestCases =
        new (string Descrption, string inputPropertyType, BungalowType? outputBungalowType)[]
            {
                ("Can parse bungalow type detached", "detached bungalow", BungalowType.Detached),
                ("Can parse bungalow type semi-detached", "semi-detached bungalow", BungalowType.SemiDetached),
                ("Can parse bungalow type mid-terrace", "mid-terrace bungalow", BungalowType.Terraced),
                ("Can parse bungalow type end terrace", "end-terrace bungalow", BungalowType.EndTerrace),
            }
            .Select(p => new EpcTestCase(
                p.Descrption, 
                new EpbEpcAssessmentDto 
                { 
                    AssessmentType = "RdSAP",
                    PropertyType = p.inputPropertyType
                }, 
                new Epc
                {
                    PropertyType = PropertyType.Bungalow,
                    BungalowType = p.outputBungalowType
                }))
            .ToArray();
    
    private static readonly EpcTestCase[] FlatTypeTestCases =
        new (string Descrption, string inputPropertyType, FlatType? outputFlatType)[]
            {
                ("Can parse flat type ground flat (basement)", "basement flat", FlatType.GroundFloor),
                ("Can parse flat type ground flat", "ground flat", FlatType.GroundFloor),
                ("Can parse flat type middle flat", "mid flat", FlatType.MiddleFloor),
                ("Can parse flat type top flat", "top flat", FlatType.TopFloor),
            }
            .Select(p => new EpcTestCase(
                p.Descrption, 
                new EpbEpcAssessmentDto 
                { 
                    AssessmentType = "RdSAP",
                    PropertyType = p.inputPropertyType
                }, 
                new Epc
                {
                    PropertyType = PropertyType.ApartmentFlatOrMaisonette,
                    FlatType = p.outputFlatType
                }))
            .ToArray();
    
    private static readonly EpcTestCase[] WallConstructionTestCases =
        new (string Descrption, string[] inputWallsDescription, WallConstruction? outputWallConstruction)[]
            {
                ("Can handle null wall construction", null, null),
                ("Can parse wall construction solid", new[] {"solid brick"}, WallConstruction.Solid),
                ("Can parse wall construction solid (sandstone or limestone)", new[] {"Sandstone or limestone"}, WallConstruction.Solid),
                ("Can parse wall construction solid (granite or whinstone)", new[] {"Granite or whinstone"}, WallConstruction.Solid),
                ("Can parse wall construction solid (solid and sandstone)", new[] {"solid brick", "Sandstone or limestone"}, WallConstruction.Solid),
                ("Can parse wall construction cavity", new[] {"cavity wall"}, WallConstruction.Cavity),
                ("Can parse wall construction mixed walls", new[] {"solid brick", "cavity wall"}, WallConstruction.Mixed),
                ("Can parse wall construction other (System built)", new[] {"System built"}, WallConstruction.Other),
                ("Can parse wall construction other (Cob)", new[] {"Cob "}, WallConstruction.Other),
                ("Can parse wall construction other (Timber frame)", new[] {"Timber frame"}, WallConstruction.Other),
                ("Can parse wall construction other (Park home wall)", new[] {"Park home wall"}, WallConstruction.Other),
                ("Can parse wall construction other (other + solid)", new[] {"Cob", "solid brick"}, WallConstruction.Other),
            }
            .Select(p => new EpcTestCase(
                p.Descrption, 
                new EpbEpcAssessmentDto 
                { 
                    AssessmentType = "RdSAP",
                    WallsDescription = p.inputWallsDescription?.ToList()
                }, 
                new Epc
                {
                    WallConstruction = p.outputWallConstruction
                }))
            .ToArray();
    
    private static readonly EpcTestCase[] SolidWallsInsulatedTestCases =
        new (string Descrption, string[] inputWallsDescription, SolidWallsInsulated? outputSolidWallsInsulated)[]
            {
                ("Can parse solid walls insulated", new[] {"solid brick, as built, insulated (assumed)"}, SolidWallsInsulated.All),
                ("Can parse solid walls insulated (internal)", new[] {"solid brick, with internal insulation"}, SolidWallsInsulated.All),
                ("Can parse solid walls insulated (external)", new[] {"solid brick, with external insulation"}, SolidWallsInsulated.All),
                ("Can parse solid walls partially insulated", new[] {"solid brick, as built, partial insulation (assumed)"}, SolidWallsInsulated.Some),
                ("Can parse solid walls no insulation", new[] {"solid brick, as built, no insulation (assumed)"}, SolidWallsInsulated.No),
            }
            .Select(p => new EpcTestCase(
                p.Descrption, 
                new EpbEpcAssessmentDto 
                { 
                    AssessmentType = "RdSAP",
                    WallsDescription = p.inputWallsDescription?.ToList()
                }, 
                new Epc
                {
                    WallConstruction = WallConstruction.Solid,
                    SolidWallsInsulated = p.outputSolidWallsInsulated
                }))
            .ToArray();
    
    private static readonly EpcTestCase[] CavityWallsInsulatedTestCases =
        new (string Descrption, string[] inputWallsDescription, CavityWallsInsulated? outputCavityWallsInsulated)[]
            {
                ("Can parse cavity walls insulated", new[] {"cavity wall, as built, insulated (assumed)"}, CavityWallsInsulated.All),
                ("Can parse cavity walls insulated (filled cavity)", new[] {"cavity wall, filled cavity"}, CavityWallsInsulated.All),
                ("Can parse cavity walls insulated (internal)", new[] {"cavity wall, with internal insulation"}, CavityWallsInsulated.All),
                ("Can parse cavity walls insulated (external)", new[] {"cavity wall, with external insulation"}, CavityWallsInsulated.All),
                ("Can parse cavity walls partially insulated", new[] {"cavity wall, as built, partial insulation (assumed)"}, CavityWallsInsulated.Some),
                ("Can parse cavity walls no insulation", new[] {"cavity wall, as built, no insulation (assumed)"}, CavityWallsInsulated.No),
            }
            .Select(p => new EpcTestCase(
                p.Descrption, 
                new EpbEpcAssessmentDto 
                { 
                    AssessmentType = "RdSAP",
                    WallsDescription = p.inputWallsDescription?.ToList()
                }, 
                new Epc
                {
                    WallConstruction = WallConstruction.Cavity,
                    CavityWallsInsulated = p.outputCavityWallsInsulated
                }))
            .ToArray();
    
    private static readonly EpcTestCase[] FloorConstructionTestCases =
        new (string Descrption, string[] inputFloorDescription, FloorConstruction? outputFloorConstruction)[]
            {
                ("Can handle null floor construction", null, null),
                ("Can parse floor construction suspended timber", new[] {"suspended"}, FloorConstruction.SuspendedTimber),
                ("Can parse floor construction suspended timber (suspended + suspended)", new[] {"suspended", "suspended"}, FloorConstruction.SuspendedTimber),
                ("Can parse floor construction solid concrete", new[] {"solid"}, FloorConstruction.SolidConcrete),
                ("Can parse floor construction solid concrete (solid + solid)", new[] {"solid", "solid"}, FloorConstruction.SolidConcrete),
                ("Can parse floor construction mixed", new[] {"suspended", "solid"}, FloorConstruction.Mix)
            }
            .Select(p => new EpcTestCase(
                p.Descrption, 
                new EpbEpcAssessmentDto 
                { 
                    AssessmentType = "RdSAP",
                    FloorDescription = p.inputFloorDescription?.ToList()
                }, 
                new Epc
                {
                    FloorConstruction = p.outputFloorConstruction
                }))
            .ToArray();
    
    private static readonly EpcTestCase[] FloorInsulatedTestCases =
        new (string Descrption, string[] inputFloorDescription, FloorInsulated? outputFloorInsulation)[]
            {
                ("Can parse floor insulated", new[] {"solid, insulated"}, FloorInsulated.Yes),
                ("Can parse floor insulated (limited)", new[] {"solid, limited insulation (assumed)"}, FloorInsulated.Yes),
                ("Can parse floor uninsulated", new[] {"solid, no insulation (assumed)"}, FloorInsulated.No),
            }
            .Select(p => new EpcTestCase(
                p.Descrption, 
                new EpbEpcAssessmentDto 
                { 
                    AssessmentType = "RdSAP",
                    FloorDescription = p.inputFloorDescription?.ToList()
                }, 
                new Epc
                {
                    FloorConstruction = FloorConstruction.SolidConcrete,
                    FloorInsulated = p.outputFloorInsulation
                }))
            .ToArray();
    
    private static readonly EpcTestCase[] RoofConstructionTestCases =
        new (string Descrption, string[] inputRoofDescription, RoofConstruction? outputRoofConstruction)[]
            {
                ("Can handle null roof construction", null, null),
                ("Can parse roof construction pitched", new[] {"pitched"}, RoofConstruction.Pitched),
                ("Can parse roof construction flat", new[] {"flat"}, RoofConstruction.Flat),
                ("Can parse roof construction flat (thatched)", new[] {"thatched"}, RoofConstruction.Flat),
                ("Can parse roof construction mixed", new[] {"pitched", "flat"}, RoofConstruction.Mixed),
            }
            .Select(p => new EpcTestCase(
                p.Descrption, 
                new EpbEpcAssessmentDto 
                { 
                    AssessmentType = "RdSAP",
                    RoofDescription = p.inputRoofDescription?.ToList()
                }, 
                new Epc
                {
                    RoofConstruction = p.outputRoofConstruction
                }))
            .ToArray();
    
    private static readonly EpcTestCase[] RoofInsulatedTestCases =
        new (string Descrption, string[] inputRoofDescription, RoofInsulated? outputRoofInsulated)[]
            {
                ("Can parse roof insulated", new[] {"pitched, insulated"}, RoofInsulated.Yes),
                ("Can parse roof insulated (insulated at rafters)", new[] {"pitched, insulated at rafters"}, RoofInsulated.Yes),
                ("Can parse roof insulated (limited)", new[] {"pitched, limited insulation (assumed)"}, RoofInsulated.Yes),
                ("Can parse roof insulated >= 200mm", new[] {"pitched, 200 mm loft insulation"}, RoofInsulated.Yes),
                ("Can parse roof uninsulated", new[] {"pitched, no insulation"}, RoofInsulated.No),
                ("Can parse roof uninsulated < 200mm", new[] {"pitched, 150 mm loft insulation"}, RoofInsulated.No),
            }
            .Select(p => new EpcTestCase(
                p.Descrption, 
                new EpbEpcAssessmentDto 
                { 
                    AssessmentType = "RdSAP",
                    RoofDescription = p.inputRoofDescription?.ToList()
                }, 
                new Epc
                {
                    RoofConstruction = RoofConstruction.Pitched,
                    RoofInsulated = p.outputRoofInsulated
                }))
            .ToArray();
    
    private static readonly EpcTestCase[] GlazingTypeTestCases =
        new (string Descrption, string[] inputWindowsDescription, GlazingType? outputGlazingType)[]
            {
                ("Can handle null glazing type", null, null),
                ("Can parse glazing type single glazing", new[] {"single glazed"}, GlazingType.SingleGlazed),
                ("Can parse glazing type both (some double)", new[] {"some double glazing"}, GlazingType.Both),
                ("Can parse glazing type both (some triple)", new[] {"some triple glazing"}, GlazingType.Both),
                ("Can parse glazing type both (some secondary)", new[] {"some secondary glazing"}, GlazingType.Both),
                ("Can parse glazing type both (some multiple)", new[] {"some multiple glazing"}, GlazingType.Both),
                ("Can parse glazing type both (partial double)", new[] {"partial double glazing"}, GlazingType.Both),
                ("Can parse glazing type both (partial triple)", new[] {"partial triple glazing"}, GlazingType.Both),
                ("Can parse glazing type both (partial secondary)", new[] {"partial secondary glazing"}, GlazingType.Both),
                ("Can parse glazing type both (partial multiple)", new[] {"partial multiple glazing"}, GlazingType.Both),
                ("Can parse glazing type both (mostly double)", new[] {"mostly double glazing"}, GlazingType.Both),
                ("Can parse glazing type both (mostly triple)", new[] {"mostly triple glazing"}, GlazingType.Both),
                ("Can parse glazing type both (mostly secondary)", new[] {"mostly secondary glazing"}, GlazingType.Both),
                ("Can parse glazing type both (mostly multiple)", new[] {"mostly multiple glazing"}, GlazingType.Both),
                ("Can parse glazing type both (throughout)", new[] {"multiple glazing throughout"}, GlazingType.Both),
                ("Can parse glazing type double or triple (full secondary)", new[] {"full secondary glazing"}, GlazingType.DoubleOrTripleGlazed),
                ("Can parse glazing type double or triple (fully double)", new[] {"fully double glazed"}, GlazingType.DoubleOrTripleGlazed),
                ("Can parse glazing type double or triple (fully triple)", new[] {"fully triple glazed"}, GlazingType.DoubleOrTripleGlazed),
                ("Can parse glazing type double or triple (high)", new[] {"High performance glazing"}, GlazingType.DoubleOrTripleGlazed),
            }
            .Select(p => new EpcTestCase(
                p.Descrption, 
                new EpbEpcAssessmentDto 
                { 
                    AssessmentType = "RdSAP",
                    WindowsDescription = p.inputWindowsDescription?.ToList()
                }, 
                new Epc
                {
                    GlazingType = p.outputGlazingType
                }))
            .ToArray();
    
    private static readonly EpcTestCase[] HeatingTypeTestCases =
        new (string Descrption, string inputMainFuelType, string inputMainHeatingDescription, EpcHeatingType? outputEpcHeatingType)[]
            {
                ("Can handle null heating type", null, null, null),
                ("Can parse heating type other (mains gas (community))", "mains gas (community)", null, EpcHeatingType.Other),
                ("Can parse heating type other (LPG (community))", "LPG (community)", null, EpcHeatingType.Other),
                ("Can parse heating type other (oil (community))", "oil (community)", null, EpcHeatingType.Other),
                ("Can parse heating type other (B30D (community))", "B30D (community)", null, EpcHeatingType.Other),
                ("Can parse heating type other (coal (community))", "coal (community)", null, EpcHeatingType.Other),
                ("Can parse heating type other (electricity (community))", "electricity (community)", null, EpcHeatingType.Other),
                ("Can parse heating type other (waste combustion (community))", "waste combustion (community)", null, EpcHeatingType.Other),
                ("Can parse heating type other (biomass (community))", "biomass (community)", null, EpcHeatingType.Other),
                ("Can parse heating type other (biogas (community))", "biogas (community)", null, EpcHeatingType.Other),
                ("Can parse heating type other (heat from boilers that can use mineral oil or biodiesel (community))", "heat from boilers that can use mineral oil or biodiesel (community)", null, EpcHeatingType.Other),
                ("Can parse heating type other (heat from boilers using biodiesel from any biomass source (community))", "heat from boilers using biodiesel from any biomass source (community)", null, EpcHeatingType.Other),
                ("Can parse heating type other (biodiesel from vegetable oil only (community))", "biodiesel from vegetable oil only (community)", null, EpcHeatingType.Other),
                ("Can parse heating type other (from heat network data (community))", "from heat network data (community)", null, EpcHeatingType.Other),
                ("Can parse heating type other (biodiesel from any biomass source)", "biodiesel from any biomass source", null, EpcHeatingType.Other),
                ("Can parse heating type other (biodiesel from used cooking oil only)", "biodiesel from used cooking oil only", null, EpcHeatingType.Other),
                ("Can parse heating type other (biodiesel from vegetable oil only (not community))", "biodiesel from vegetable oil only (not community)", null, EpcHeatingType.Other),
                ("Can parse heating type other (waste combustion)", "waste combustion", null, EpcHeatingType.Other),
                ("Can parse heating type other (wood pellets in bags for secondary heating)", "wood pellets in bags for secondary heating", null, EpcHeatingType.Other),
                ("Can parse heating type gas boiler (mains gas - this is for backwards compatibility only and should not be used)", "mains gas - this is for backwards compatibility only and should not be used", null, EpcHeatingType.GasBoiler),
                ("Can parse heating type gas boiler (biogas - landfill)", "biogas - landfill", null, EpcHeatingType.GasBoiler),
                ("Can parse heating type gas boiler (mains gas (not community))", "mains gas (not community)", null, EpcHeatingType.GasBoiler),
                ("Can parse heating type gas boiler (biogas (not community))", "biogas (not community)", null, EpcHeatingType.GasBoiler),
                ("Can parse heating type LPG boiler (LPG - this is for backwards compatibility only and should not be used)", "LPG - this is for backwards compatibility only and should not be used", null, EpcHeatingType.LpgBoiler),
                ("Can parse heating type LPG boiler (bottled LPG)", "bottled LPG", null, EpcHeatingType.LpgBoiler),
                ("Can parse heating type LPG boiler (LPG special condition)", "LPG special condition", null, EpcHeatingType.LpgBoiler),
                ("Can parse heating type LPG boiler (LPG (not community))", "LPG (not community)", null, EpcHeatingType.LpgBoiler),
                ("Can parse heating type oil boiler (oil - this is for backwards compatibility only and should not be used)", "oil - this is for backwards compatibility only and should not be used", null, EpcHeatingType.OilBoiler),
                ("Can parse heating type oil boiler (B30K (not community))", "B30K (not community)", null, EpcHeatingType.OilBoiler),
                ("Can parse heating type oil boiler (oil (not community))", "oil (not community)", null, EpcHeatingType.OilBoiler),
                ("Can parse heating type oil boiler (appliances able to use mineral oil or liquid biofuel)", "appliances able to use mineral oil or liquid biofuel", null, EpcHeatingType.OilBoiler),
                ("Can parse heating type coal or solid fuel (anthracite)", "anthracite", null, EpcHeatingType.CoalOrSolidFuel),
                ("Can parse heating type coal or solid fuel (house coal)", "house coal", null, EpcHeatingType.CoalOrSolidFuel),
                ("Can parse heating type coal or solid fuel (smokeless coal)", "smokeless coal", null, EpcHeatingType.CoalOrSolidFuel),
                ("Can parse heating type coal or solid fuel (house coal (not community))", "house coal (not community)", null, EpcHeatingType.CoalOrSolidFuel),
                ("Can parse heating type biomass boiler (wood logs)", "wood logs", null, EpcHeatingType.Biomass),
                ("Can parse heating type biomass boiler (bulk wood pellets)", "bulk wood pellets", null, EpcHeatingType.Biomass),
                ("Can parse heating type biomass boiler (wood chips)", "wood chips", null, EpcHeatingType.Biomass),
                ("Can parse heating type biomass boiler (dual fuel)", "dual fuel - mineral + wood", null, EpcHeatingType.Biomass),
                ("Can parse heating type biomass boiler (biomass)", "biomass", null, EpcHeatingType.Biomass),
                ("Can parse heating type other (electricity)", "electricity", "community heating system", EpcHeatingType.Other),
                ("Can parse heating type direct action electric (boiler with radiators)", "electricity", "boiler with radiators or underfloor heating", EpcHeatingType.DirectActionElectric),
                ("Can parse heating type direct action electric (underflow heating)", "electricity", "electric underfloor heating", EpcHeatingType.DirectActionElectric),
                ("Can parse heating type direct action electric (warm air system)", "electricity", "warm air system (not heat pump)", EpcHeatingType.DirectActionElectric),
                ("Can parse heating type direct action electric (room heaters)", "electricity", "room heaters", EpcHeatingType.DirectActionElectric),
                ("Can parse heating type direct action electric (other)", "electricity", "other system", EpcHeatingType.DirectActionElectric),
                ("Can parse heating type heat pump", "electricity", "heat pump with radiators or underfloor heating", EpcHeatingType.HeatPump),
                ("Can parse heating type heat pump", "electricity", "heat pump with warm air distribution", EpcHeatingType.HeatPump),
                ("Can parse heating type storage heater", "electricity", "electric storage heaters", EpcHeatingType.Storage),
                ("Can parse heating type gas boiler (electricity + micro-cogeneration)", "electricity", "micro-cogeneration", EpcHeatingType.GasBoiler),
            }
            .Select(p => new EpcTestCase(
                p.Descrption, 
                new EpbEpcAssessmentDto 
                { 
                    AssessmentType = "RdSAP",
                    MainFuelType = p.inputMainFuelType,
                    MainHeatingDescription = p.inputMainHeatingDescription
                }, 
                new Epc
                {
                    EpcHeatingType = p.outputEpcHeatingType
                }))
            .ToArray();
    
    private static readonly EpcTestCase[] HasHotWaterCylinderTestCases =
        new (string Descrption, bool? inputHasHotWaterCylinder, HasHotWaterCylinder? outputHasHotWaterCylinder)[]
            {
                ("Can handle null has hot water cylinder", null, null),
                ("Can parse has hot water cylinder (true)", true, HasHotWaterCylinder.Yes),
                ("Can parse has hot water cylinder (false)", false, HasHotWaterCylinder.No),
            }
            .Select(p => new EpcTestCase(
                p.Descrption, 
                new EpbEpcAssessmentDto 
                { 
                    AssessmentType = "RdSAP",
                    HasHotWaterCylinder = p.inputHasHotWaterCylinder
                }, 
                new Epc
                {
                    HasHotWaterCylinder = p.outputHasHotWaterCylinder
                }))
            .ToArray();
    
    private static readonly EpcTestCase[] EpcParseTestCases =
        Array.Empty<EpcTestCase>()
            .Concat(AssessmentTypeTestCases)
            .Concat(LodgementDateTestCases)
            .Concat(ConstructionAgeBandTestCases)
            .Concat(PropertyTypeTestCases)
            .Concat(HouseTypeTestCases)
            .Concat(BungalowTypeTestCases)
            .Concat(FlatTypeTestCases)
            .Concat(WallConstructionTestCases)
            .Concat(SolidWallsInsulatedTestCases)
            .Concat(CavityWallsInsulatedTestCases)
            .Concat(FloorConstructionTestCases)
            .Concat(FloorInsulatedTestCases)
            .Concat(RoofConstructionTestCases)
            .Concat(RoofInsulatedTestCases)
            .Concat(GlazingTypeTestCases)
            .Concat(HeatingTypeTestCases)
            .Concat(HasHotWaterCylinderTestCases)
            .ToArray();

    public class EpcTestCase
    {
        public readonly string Description;
        public readonly EpbEpcAssessmentDto Input;
        public readonly Epc Output;

        public EpcTestCase(
            string description, EpbEpcAssessmentDto input, Epc output)
        {
            Description = description;
            Input = input;
            Output = output;
        }

        public override string ToString()
        {
            return Description;
        }
    }
}