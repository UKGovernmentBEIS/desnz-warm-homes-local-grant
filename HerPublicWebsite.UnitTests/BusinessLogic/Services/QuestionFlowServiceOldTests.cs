using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
using HerPublicWebsite.BusinessLogic.Models;
using HerPublicWebsite.BusinessLogic.Models.Enums;
using HerPublicWebsite.BusinessLogic.Services;

namespace Tests.BusinessLogic.Services;

[TestFixture]
public class QuestionFlowServiceOldTests
{
    [SetUp]
    public void Setup()
    {
        questionFlowServiceOld = new QuestionFlowServiceOld();
    }

    private IQuestionFlowServiceOld questionFlowServiceOld;

    [TestCaseSource(nameof(BackTestCases))]
    public void RunBackLinkTestCases(QuestionFlowServiceTestCase testCase)
    {
        // Act
        var output = questionFlowServiceOld.PreviousStep(
            testCase.Input.Page,
            testCase.Input.PropertyData,
            testCase.Input.EntryPoint);

        // Assert
        output.Should().Be(testCase.ExpectedOutput);
    }

    [TestCaseSource(nameof(ForwardTestCases))]
    public void RunForwardLinkTestCases(QuestionFlowServiceTestCase testCase)
    {
        // Act
        var output = questionFlowServiceOld.NextStep(
            testCase.Input.Page,
            testCase.Input.PropertyData,
            testCase.Input.EntryPoint);

        // Assert
        output.Should().Be(testCase.ExpectedOutput);
    }

    [TestCaseSource(nameof(SkipTestCases))]
    public void RunSkipLinkTestCases(QuestionFlowServiceTestCase testCase)
    {
        // Act
        var output = questionFlowServiceOld.SkipDestination(
            testCase.Input.Page,
            testCase.Input.PropertyData,
            testCase.Input.EntryPoint);

        // Assert
        output.Should().Be(testCase.ExpectedOutput);
    }

    private static QuestionFlowServiceTestCase[] BackTestCases =
    {
        new(
            "A new or returning user goes back to the Index page",
            new Input(
                QuestionFlowStepOld.NewOrReturningUser
            ),
            QuestionFlowStepOld.Start),
        new(
            "Country goes back to new or returning user",
            new Input(
                QuestionFlowStepOld.Country
            ),
            QuestionFlowStepOld.NewOrReturningUser),
        new(
            "Ownership status goes back to Country",
            new Input(
                QuestionFlowStepOld.OwnershipStatus,
                "ABCDEFGH"
            ),
            QuestionFlowStepOld.Country),
        new(
            "Service unsuitable goes back to the country you came from",
            new Input(
                QuestionFlowStepOld.ServiceUnsuitable,
                "ABCDEFGH",
                country: Country.Other
            ),
            QuestionFlowStepOld.Country),
        new(
            "Service unsuitable goes back to ownership status if user is a private tenant",
            new Input(
                QuestionFlowStepOld.ServiceUnsuitable,
                "ABCDEFGH",
                country: Country.England,
                ownershipStatus: OwnershipStatus.PrivateTenancy
            ),
            QuestionFlowStepOld.OwnershipStatus),
        new(
            "Find EPC goes back to ownership status",
            new Input(
                QuestionFlowStepOld.FindEpc,
                "ABCDEFGH"
            ),
            QuestionFlowStepOld.OwnershipStatus),
        new(
            "Postcode goes back to find EPC",
            new Input(
                QuestionFlowStepOld.AskForPostcode,
                "ABCDEFGH"
            ),
            QuestionFlowStepOld.FindEpc),
        new(
            "Confirm address goes back to postcode",
            new Input(
                QuestionFlowStepOld.ConfirmAddress,
                "ABCDEFGH"
            ),
            QuestionFlowStepOld.AskForPostcode),
        new(
            "Confirm EPC details goes back to ask for postcode",
            new Input(
                QuestionFlowStepOld.ConfirmEpcDetails,
                "ABCDEFGH"
            ),
            QuestionFlowStepOld.AskForPostcode),
        new(
            "No EPC found goes back to postcode",
            new Input(
                QuestionFlowStepOld.NoEpcFound,
                "ABCDEFGH"
            ),
            QuestionFlowStepOld.AskForPostcode),
        new(
            "Property type goes back to confirm EPC details if epc exists with property type and age",
            new Input(
                QuestionFlowStepOld.PropertyType,
                "ABCDEFGH",
                epc: new Epc
                {
                    PropertyType = PropertyType.House,
                    HouseType = HouseType.Detached,
                    ConstructionAgeBand = HomeAge.From1950To1966
                }
            ),
            QuestionFlowStepOld.ConfirmEpcDetails),
        new(
            "Property type goes back to postcode if epc doesn't contain property type",
            new Input(
                QuestionFlowStepOld.PropertyType,
                "ABCDEFGH",
                epc: new Epc
                {
                    ConstructionAgeBand = HomeAge.From1950To1966
                }
            ),
            QuestionFlowStepOld.AskForPostcode),
        new(
            "Property type goes back to postcode if epc doesn't contain property age",
            new Input(
                QuestionFlowStepOld.PropertyType,
                "ABCDEFGH",
                epc: new Epc()
                {
                    PropertyType = PropertyType.House,
                    HouseType = HouseType.Detached
                }
            ),
            QuestionFlowStepOld.AskForPostcode),
        new(
            "Property type goes back to no EPC found if searched for EPC but no EPC set",
            new Input(
                QuestionFlowStepOld.PropertyType,
                "ABCDEFGH",
                epc: null,
                searchForEpc: SearchForEpc.Yes
            ),
            QuestionFlowStepOld.NoEpcFound),
        new(
            "Property type goes back to find EPC if didn't search for EPC",
            new Input(
                QuestionFlowStepOld.PropertyType,
                "ABCDEFGH",
                searchForEpc: SearchForEpc.No
            ),
            QuestionFlowStepOld.FindEpc),
        new(
            "Changing property type goes back to check your unchangeable answers",
            new Input(
                QuestionFlowStepOld.PropertyType,
                "ABCDEFGH",
                entryPoint: QuestionFlowStepOld.PropertyType
            ),
            QuestionFlowStepOld.CheckYourUnchangeableAnswers),
        new(
            "House type goes back to property type",
            new Input(
                QuestionFlowStepOld.HouseType,
                "ABCDEFGH"
            ),
            QuestionFlowStepOld.PropertyType),
        new(
            "Bungalow type goes back to property type",
            new Input(
                QuestionFlowStepOld.BungalowType,
                "ABCDEFGH"
            ),
            QuestionFlowStepOld.PropertyType),
        new(
            "Flat type goes back to property type",
            new Input(
                QuestionFlowStepOld.FlatType,
                "ABCDEFGH"
            ),
            QuestionFlowStepOld.PropertyType),
        new(
            "Home age goes back to the property type it came from (house)",
            new Input(
                QuestionFlowStepOld.HomeAge,
                "ABCDEFGH",
                propertyType: PropertyType.House
            ),
            QuestionFlowStepOld.HouseType),
        new(
            "Home age goes back to the property type it came from (bungalow)",
            new Input(
                QuestionFlowStepOld.HomeAge,
                "ABCDEFGH",
                propertyType: PropertyType.Bungalow
            ),
            QuestionFlowStepOld.BungalowType),
        new(
            "Home age goes back to the property type it came from (flat)",
            new Input(
                QuestionFlowStepOld.HomeAge,
                "ABCDEFGH",
                propertyType: PropertyType.ApartmentFlatOrMaisonette
            ),
            QuestionFlowStepOld.FlatType),
        new(
            "Changing home age goes back to check your unchangeable answers",
            new Input(
                QuestionFlowStepOld.HomeAge,
                "ABCDEFGH",
                entryPoint: QuestionFlowStepOld.HomeAge
            ),
            QuestionFlowStepOld.CheckYourUnchangeableAnswers),
        new(
            "Wall construction goes back to check your unchangeable answers if EPC details were not confirmed",
            new Input(
                QuestionFlowStepOld.WallConstruction,
                "ABCDEFGH",
                epcDetailsConfirmed: EpcDetailsConfirmed.No

            ),
            QuestionFlowStepOld.CheckYourUnchangeableAnswers),
        new(
            "Wall construction goes back to confirm EPC details if EPC details were confirmed",
            new Input(
                QuestionFlowStepOld.WallConstruction,
                "ABCDEFGH",
                epcDetailsConfirmed: EpcDetailsConfirmed.Yes
            ),
            QuestionFlowStepOld.ConfirmEpcDetails),
        new(
            "Changing wall construction goes back to summary",
            new Input(
                QuestionFlowStepOld.WallConstruction,
                "ABCDEFGH",
                entryPoint: QuestionFlowStepOld.WallConstruction
            ),
            QuestionFlowStepOld.AnswerSummary),
        new(
            "Cavity walls insulated goes back to wall construction",
            new Input(
                QuestionFlowStepOld.CavityWallsInsulated,
                "ABCDEFGH"
            ),
            QuestionFlowStepOld.WallConstruction),
        new(
            "Changing cavity walls insulated goes back to summary",
            new Input(
                QuestionFlowStepOld.CavityWallsInsulated,
                "ABCDEFGH",
                entryPoint: QuestionFlowStepOld.CavityWallsInsulated
            ),
            QuestionFlowStepOld.AnswerSummary),
        new(
            "Solid walls insulated goes back to cavity walls insulated if user has mixed walls",
            new Input(
                QuestionFlowStepOld.SolidWallsInsulated,
                "ABCDEFGH",
                wallConstruction: WallConstruction.Mixed
            ),
            QuestionFlowStepOld.CavityWallsInsulated),
        new(
            "Solid walls insulated goes back to wall construction if user does not have mixed walls",
            new Input(
                QuestionFlowStepOld.SolidWallsInsulated,
                "ABCDEFGH",
                wallConstruction: WallConstruction.Solid
            ),
            QuestionFlowStepOld.WallConstruction),
        new(
            "Changing solid walls insulated goes back to summary",
            new Input(
                QuestionFlowStepOld.SolidWallsInsulated,
                "ABCDEFGH",
                entryPoint: QuestionFlowStepOld.SolidWallsInsulated
            ),
            QuestionFlowStepOld.AnswerSummary),
        new(
            "Floor construction goes back to the wall insulation the user answered last (solid walls)",
            new Input(
                QuestionFlowStepOld.FloorConstruction,
                "ABCDEFGH",
                wallConstruction: WallConstruction.Solid
            ),
            QuestionFlowStepOld.SolidWallsInsulated),
        new(
            "Floor construction goes back to the wall insulation the user answered last (cavity walls)",
            new Input(
                QuestionFlowStepOld.FloorConstruction,
                "ABCDEFGH",
                wallConstruction: WallConstruction.Cavity
            ),
            QuestionFlowStepOld.CavityWallsInsulated),
        new(
            "Floor construction goes back to wall construction if user has neither cavity not solid walls",
            new Input(
                QuestionFlowStepOld.FloorConstruction,
                "ABCDEFGH",
                wallConstruction: WallConstruction.Other
            ),
            QuestionFlowStepOld.WallConstruction),
        new(
            "Changing floor construction goes back to summary",
            new Input(
                QuestionFlowStepOld.FloorConstruction,
                "ABCDEFGH",
                entryPoint: QuestionFlowStepOld.FloorConstruction
            ),
            QuestionFlowStepOld.AnswerSummary),
        new(
            "Floor insulated goes back to floor construction",
            new Input(
                QuestionFlowStepOld.FloorInsulated,
                "ABCDEFGH"
            ),
            QuestionFlowStepOld.FloorConstruction),
        new(
            "Changing floor insulated goes back to summary",
            new Input(
                QuestionFlowStepOld.FloorInsulated,
                "ABCDEFGH",
                entryPoint: QuestionFlowStepOld.FloorInsulated
            ),
            QuestionFlowStepOld.AnswerSummary),
        new(
            "Roof construction goes back to floor insulation if the user has timber or concrete floors",
            new Input(
                QuestionFlowStepOld.RoofConstruction,
                propertyType: PropertyType.House,
                floorConstruction: FloorConstruction.SuspendedTimber,
                reference: "ABCDEFGH"
            ),
            QuestionFlowStepOld.FloorInsulated),
        new(
            "Roof construction goes back to floor construction if the user has different floors",
            new Input(
                QuestionFlowStepOld.RoofConstruction,
                propertyType: PropertyType.House,
                floorConstruction: FloorConstruction.Other,
                reference: "ABCDEFGH"
            ),
            QuestionFlowStepOld.FloorConstruction),
        new(
            "Roof construction goes back to wall insulation if user was not asked about their floor but has insulated walls",
            new Input(
                QuestionFlowStepOld.RoofConstruction,
                propertyType: PropertyType.ApartmentFlatOrMaisonette,
                flatType: FlatType.TopFloor,
                wallConstruction: WallConstruction.Solid,
                reference: "ABCDEFGH"
            ),
            QuestionFlowStepOld.SolidWallsInsulated),
        new(
            "Roof construction goes back to wall construction if user was not asked about their floor and has other walls",
            new Input(
                QuestionFlowStepOld.RoofConstruction,
                propertyType: PropertyType.ApartmentFlatOrMaisonette,
                flatType: FlatType.TopFloor,
                wallConstruction: WallConstruction.DoNotKnow,
                reference: "ABCDEFGH"
            ),
            QuestionFlowStepOld.WallConstruction),
        new(
            "Changing roof construction goes back to summary",
            new Input(
                QuestionFlowStepOld.RoofConstruction,
                "ABCDEFGH",
                entryPoint: QuestionFlowStepOld.RoofConstruction
            ),
            QuestionFlowStepOld.AnswerSummary),
        new(
            "Loft space goes back to roof construction",
            new Input(
                QuestionFlowStepOld.LoftSpace,
                "ABCDEFGH"
            ),
            QuestionFlowStepOld.RoofConstruction),
        new(
            "Changing loft space goes back to summary",
            new Input(
                QuestionFlowStepOld.LoftSpace,
                "ABCDEFGH",
                entryPoint: QuestionFlowStepOld.LoftSpace
            ),
            QuestionFlowStepOld.AnswerSummary),
        new(
            "Loft access goes back to loft space",
            new Input(
                QuestionFlowStepOld.LoftAccess,
                "ABCDEFGH"
            ),
            QuestionFlowStepOld.LoftSpace),
        new(
            "Changing loft access goes back to summary",
            new Input(
                QuestionFlowStepOld.LoftAccess,
                "ABCDEFGH",
                entryPoint: QuestionFlowStepOld.LoftAccess
            ),
            QuestionFlowStepOld.AnswerSummary),
        new(
            "Roof insulated goes back to loft access",
            new Input(
                QuestionFlowStepOld.RoofInsulated,
                "ABCDEFGH"
            ),
            QuestionFlowStepOld.LoftAccess),
        new(
            "Changing roof insulated goes back to summary",
            new Input(
                QuestionFlowStepOld.RoofInsulated,
                "ABCDEFGH",
                entryPoint: QuestionFlowStepOld.RoofInsulated
            ),
            QuestionFlowStepOld.AnswerSummary),
        new(
            "Glazing type goes back to roof construction if the user has flat roof",
            new Input(
                QuestionFlowStepOld.GlazingType,
                "ABCDEFGH",
                propertyType: PropertyType.House,
                roofConstruction: RoofConstruction.Flat
            ),
            QuestionFlowStepOld.RoofConstruction),
        new(
            "Glazing type goes back to loft space if the user does not have flat roof nor loft space",
            new Input(
                QuestionFlowStepOld.GlazingType,
                "ABCDEFGH",
                propertyType: PropertyType.House,
                roofConstruction: RoofConstruction.Pitched,
                loftSpace: LoftSpace.No
            ),
            QuestionFlowStepOld.LoftSpace),
        new(
            "Glazing type goes back to loft access if the user does not have flat roof and has inaccessible loft space",
            new Input(
                QuestionFlowStepOld.GlazingType,
                "ABCDEFGH",
                propertyType: PropertyType.House,
                roofConstruction: RoofConstruction.Pitched,
                loftSpace: LoftSpace.Yes,
                accessibleLoft: LoftAccess.No
            ),
            QuestionFlowStepOld.LoftAccess),
        new(
            "Glazing type goes back to roof insulation if the user does not have flat roof but has accessible loft space",
            new Input(
                QuestionFlowStepOld.GlazingType,
                "ABCDEFGH",
                propertyType: PropertyType.House,
                roofConstruction: RoofConstruction.Pitched,
                loftSpace: LoftSpace.Yes,
                accessibleLoft: LoftAccess.Yes
            ),
            QuestionFlowStepOld.RoofInsulated),
        new(
            "Glazing type goes back to floor construction if the user was not asked about their roof",
            new Input(
                QuestionFlowStepOld.GlazingType,
                propertyType: PropertyType.ApartmentFlatOrMaisonette,
                flatType: FlatType.GroundFloor,
                reference: "ABCDEFGH"
            ),
            QuestionFlowStepOld.FloorConstruction),
        new(
            "Glazing type goes back to wall construction if the user was not asked about neither their roof or floor",
            new Input(
                QuestionFlowStepOld.GlazingType,
                propertyType: PropertyType.ApartmentFlatOrMaisonette,
                flatType: FlatType.MiddleFloor,
                reference: "ABCDEFGH"
            ),
            QuestionFlowStepOld.WallConstruction),
        new(
            "Changing glazing type goes back to summary",
            new Input(
                QuestionFlowStepOld.GlazingType,
                "ABCDEFGH",
                entryPoint: QuestionFlowStepOld.GlazingType
            ),
            QuestionFlowStepOld.AnswerSummary),
        new(
            "Outdoor space goes back to glazing type",
            new Input(
                QuestionFlowStepOld.OutdoorSpace,
                "ABCDEFGH"
            ),
            QuestionFlowStepOld.GlazingType),
        new(
            "Changing outdoor space goes back to summary",
            new Input(
                QuestionFlowStepOld.OutdoorSpace,
                "ABCDEFGH",
                entryPoint: QuestionFlowStepOld.OutdoorSpace
            ),
            QuestionFlowStepOld.AnswerSummary),
        new(
            "Heating type goes back to outdoor space",
            new Input(
                QuestionFlowStepOld.HeatingType,
                "ABCDEFGH"
            ),
            QuestionFlowStepOld.OutdoorSpace),
        new(
            "Changing heating type goes back to summary",
            new Input(
                QuestionFlowStepOld.HeatingType,
                "ABCDEFGH",
                entryPoint: QuestionFlowStepOld.HeatingType
            ),
            QuestionFlowStepOld.AnswerSummary),
        new(
            "Other heating type goes back to outdoor space",
            new Input(
                QuestionFlowStepOld.OtherHeatingType,
                "ABCDEFGH"
            ),
            QuestionFlowStepOld.HeatingType),
        new(
            "Changing other heating type goes back to summary",
            new Input(
                QuestionFlowStepOld.OtherHeatingType,
                "ABCDEFGH",
                entryPoint: QuestionFlowStepOld.OtherHeatingType
            ),
            QuestionFlowStepOld.AnswerSummary),
        new(
            "Hot water cylinder goes back to heating type",
            new Input(
                QuestionFlowStepOld.HotWaterCylinder,
                "ABCDEFGH"
            ),
            QuestionFlowStepOld.HeatingType),
        new(
            "Changing hot water cylinder goes back to summary",
            new Input(
                QuestionFlowStepOld.HotWaterCylinder,
                "ABCDEFGH",
                entryPoint: QuestionFlowStepOld.HotWaterCylinder
            ),
            QuestionFlowStepOld.AnswerSummary),
        new(
            "Number of occupants goes back to hot water cylinder if user has boiler",
            new Input(
                QuestionFlowStepOld.NumberOfOccupants,
                "ABCDEFGH",
                heatingType: HeatingType.GasBoiler
            ),
            QuestionFlowStepOld.HotWaterCylinder),
        new(
            "Number of occupants goes back to other heating type if the user selected so",
            new Input(
                QuestionFlowStepOld.NumberOfOccupants,
                "ABCDEFGH",
                heatingType: HeatingType.Other
            ),
            QuestionFlowStepOld.OtherHeatingType),
        new(
            "Number of occupants goes back to heating type if the user does not have a boiler",
            new Input(
                QuestionFlowStepOld.NumberOfOccupants,
                "ABCDEFGH",
                heatingType: HeatingType.HeatPump
            ),
            QuestionFlowStepOld.HeatingType),
        new(
            "Changing number of occupants goes back to summary",
            new Input(
                QuestionFlowStepOld.NumberOfOccupants,
                "ABCDEFGH",
                entryPoint: QuestionFlowStepOld.NumberOfOccupants
            ),
            QuestionFlowStepOld.AnswerSummary),
        new(
            "Heating pattern goes back to number of occupants",
            new Input(
                QuestionFlowStepOld.HeatingPattern,
                "ABCDEFGH"
            ),
            QuestionFlowStepOld.NumberOfOccupants),
        new(
            "Changing heating pattern goes back to summary",
            new Input(
                QuestionFlowStepOld.HeatingPattern,
                "ABCDEFGH",
                entryPoint: QuestionFlowStepOld.HeatingPattern
            ),
            QuestionFlowStepOld.AnswerSummary),
        new(
            "Temperature goes back to heating pattern",
            new Input(
                QuestionFlowStepOld.Temperature,
                "ABCDEFGH"
            ),
            QuestionFlowStepOld.HeatingPattern),
        new(
            "Changing temperature goes back to summary",
            new Input(
                QuestionFlowStepOld.Temperature,
                "ABCDEFGH",
                entryPoint: QuestionFlowStepOld.Temperature
            ),
            QuestionFlowStepOld.AnswerSummary),
        new(
            "Answer summary goes back to temperature",
            new Input(
                QuestionFlowStepOld.AnswerSummary,
                "ABCDEFGH"
            ),
            QuestionFlowStepOld.Temperature),
        new(
            "No recommendations goes back to summary",
            new Input(
                QuestionFlowStepOld.NoRecommendations,
                "ABCDEFGH"
            ),
            QuestionFlowStepOld.AnswerSummary),
        new(
            "Your recommendations goes back to summary",
            new Input(
                QuestionFlowStepOld.YourRecommendations,
                "ABCDEFGH"
            ),
            QuestionFlowStepOld.AnswerSummary)
    };

    private static QuestionFlowServiceTestCase[] ForwardTestCases =
    {
        new(
            "Country continues to service unsuitable if the service is not available",
            new Input(
                QuestionFlowStepOld.Country,
                "ABCDEFGH",
                country: Country.Other
            ),
            QuestionFlowStepOld.ServiceUnsuitable),
        new(
            "Country continues to ownership status",
            new Input(
                QuestionFlowStepOld.Country,
                "ABCDEFGH",
                country: Country.England
            ),
            QuestionFlowStepOld.OwnershipStatus),
        new(
            "Ownership status continues to service unsuitable if user is a private tenant",
            new Input(
                QuestionFlowStepOld.OwnershipStatus,
                "ABCDEFGH",
                OwnershipStatus.PrivateTenancy
            ),
            QuestionFlowStepOld.ServiceUnsuitable),
        new(
            "Ownership status continues to find EPC",
            new Input(
                QuestionFlowStepOld.OwnershipStatus,
                "ABCDEFGH",
                OwnershipStatus.OwnerOccupancy
            ),
            QuestionFlowStepOld.FindEpc),
        new(
            "Find EPC continues to postcode if yes selected",
            new Input(
                QuestionFlowStepOld.FindEpc,
                "ABCDEFGH",
                searchForEpc: SearchForEpc.Yes
            ),
            QuestionFlowStepOld.AskForPostcode),
        new(
            "Find EPC continues to property type if no selected",
            new Input(
                QuestionFlowStepOld.FindEpc,
                "ABCDEFGH",
                searchForEpc: SearchForEpc.No
            ),
            QuestionFlowStepOld.PropertyType),
        new(
            "Postcode continues to confirm address",
            new Input(
                QuestionFlowStepOld.AskForPostcode,
                "ABCDEFGH"
            ),
            QuestionFlowStepOld.ConfirmAddress),
        new(
            "Confirm address continues to no EPC found if no EPC set",
            new Input(
                QuestionFlowStepOld.ConfirmAddress,
                "ABCDEFGH",
                epc: null
            ),
            QuestionFlowStepOld.NoEpcFound),
        new(
            "Confirm address continues to confirm EPC details if EPC added contains property type and age",
            new Input(
                QuestionFlowStepOld.ConfirmAddress,
                "ABCDEFGH",
                epc: new Epc
                {
                    PropertyType = PropertyType.House,
                    HouseType = HouseType.Detached,
                    ConstructionAgeBand = HomeAge.From1950To1966
                }
            ),
            QuestionFlowStepOld.ConfirmEpcDetails),
        new(
            "Confirm address continues to property type if EPC exists but property type is missing",
            new Input(
                QuestionFlowStepOld.ConfirmAddress,
                "ABCDEFGH",
                epc: new Epc
                {
                    ConstructionAgeBand = HomeAge.From1950To1966
                }
            ),
            QuestionFlowStepOld.PropertyType),
        new(
            "Confirm address continues to property type if EPC exists but age is missing",
            new Input(
                QuestionFlowStepOld.ConfirmAddress,
                "ABCDEFGH",
                epc: new Epc()
                {
                    PropertyType = PropertyType.House,
                    HouseType = HouseType.Detached
                }
            ),
            QuestionFlowStepOld.PropertyType),
        new(
            "Confirm EPC details continues to wall construction if epc details confirmed",
            new Input(
                QuestionFlowStepOld.ConfirmEpcDetails,
                "ABCDEFGH",
                epcDetailsConfirmed: EpcDetailsConfirmed.Yes
            ),
            QuestionFlowStepOld.WallConstruction),
        new(
            "Confirm EPC details continues to property type if epc details not confirmed",
            new Input(
                QuestionFlowStepOld.ConfirmEpcDetails,
                "ABCDEFGH",
                epcDetailsConfirmed: EpcDetailsConfirmed.No
            ),
            QuestionFlowStepOld.PropertyType),
        new(
            "No EPC found continues to property type",
            new Input(
                QuestionFlowStepOld.NoEpcFound,
                "ABCDEFGH"
            ),
            QuestionFlowStepOld.PropertyType),
        new(
            "Property type continues to the relevant specific type of property (house)",
            new Input(
                QuestionFlowStepOld.PropertyType,
                "ABCDEFGH",
                propertyType: PropertyType.House
            ),
            QuestionFlowStepOld.HouseType),
        new(
            "Property type continues to the relevant specific type of property (bungalow)",
            new Input(
                QuestionFlowStepOld.PropertyType,
                "ABCDEFGH",
                propertyType: PropertyType.Bungalow
            ),
            QuestionFlowStepOld.BungalowType),
        new(
            "Property type continues to the relevant specific type of property (flat)",
            new Input(
                QuestionFlowStepOld.PropertyType,
                "ABCDEFGH",
                propertyType: PropertyType.ApartmentFlatOrMaisonette
            ),
            QuestionFlowStepOld.FlatType),
        new(
            "House type continues to home age",
            new Input(
                QuestionFlowStepOld.HouseType,
                "ABCDEFGH"
            ),
            QuestionFlowStepOld.HomeAge),
        new(
            "Changing house type continues to check your unchangeable answers",
            new Input(
                QuestionFlowStepOld.HouseType,
                "ABCDEFGH",
                entryPoint: QuestionFlowStepOld.PropertyType
            ),
            QuestionFlowStepOld.CheckYourUnchangeableAnswers),
        new(
            "Bungalow type continues to home age",
            new Input(
                QuestionFlowStepOld.BungalowType,
                "ABCDEFGH"
            ),
            QuestionFlowStepOld.HomeAge),
        new(
            "Changing bungalow type continues to check your unchangeable answers",
            new Input(
                QuestionFlowStepOld.BungalowType,
                "ABCDEFGH",
                entryPoint: QuestionFlowStepOld.PropertyType
            ),
            QuestionFlowStepOld.CheckYourUnchangeableAnswers),
        new(
            "Flat type continues to home age",
            new Input(
                QuestionFlowStepOld.FlatType,
                "ABCDEFGH"
            ),
            QuestionFlowStepOld.HomeAge),
        new(
            "Changing flat type continues to check your unchangeable answers",
            new Input(
                QuestionFlowStepOld.FlatType,
                "ABCDEFGH",
                entryPoint: QuestionFlowStepOld.PropertyType
            ),
            QuestionFlowStepOld.CheckYourUnchangeableAnswers),
        new(
            "Home age continues to check your unchangeable answers",
            new Input(
                QuestionFlowStepOld.HomeAge,
                "ABCDEFGH"
            ),
            QuestionFlowStepOld.CheckYourUnchangeableAnswers),
        new(
            "Changing home age continues to check your unchangeable answers",
            new Input(
                QuestionFlowStepOld.HomeAge,
                "ABCDEFGH",
                entryPoint: QuestionFlowStepOld.HomeAge
            ),
            QuestionFlowStepOld.CheckYourUnchangeableAnswers),
        new(
            "Check your unchangeable answers continues to wall consutruction",
            new Input(
                QuestionFlowStepOld.CheckYourUnchangeableAnswers,
                "ABCDEFGH"
            ),
            QuestionFlowStepOld.WallConstruction),
        new(
            "Wall construction continues to the respective type of wall insulation",
            new Input(
                QuestionFlowStepOld.WallConstruction,
                "ABCDEFGH",
                wallConstruction: WallConstruction.Solid
            ),
            QuestionFlowStepOld.SolidWallsInsulated),
        new(
            "Wall construction continues to floor construction if user's walls are not known and the user can answer floor questions",
            new Input(
                QuestionFlowStepOld.WallConstruction,
                "ABCDEFGH",
                propertyType: PropertyType.House,
                wallConstruction: WallConstruction.Other
            ),
            QuestionFlowStepOld.FloorConstruction),
        new(
            "Wall construction continues to roof construction if user's walls are not known, can not answer floor questions but can answer roof ones",
            new Input(
                QuestionFlowStepOld.WallConstruction,
                "ABCDEFGH",
                propertyType: PropertyType.ApartmentFlatOrMaisonette,
                flatType: FlatType.TopFloor,
                wallConstruction: WallConstruction.Other
            ),
            QuestionFlowStepOld.RoofConstruction),
        new(
            "Wall construction continues to glazing type if user's walls are not known and can not answer neither floor questions nor roof ones",
            new Input(
                QuestionFlowStepOld.WallConstruction,
                "ABCDEFGH",
                propertyType: PropertyType.ApartmentFlatOrMaisonette,
                flatType: FlatType.MiddleFloor,
                wallConstruction: WallConstruction.Other
            ),
            QuestionFlowStepOld.GlazingType),
        new(
            "Changing wall construction continues to summary if walls are not known",
            new Input(
                QuestionFlowStepOld.WallConstruction,
                "ABCDEFGH",
                wallConstruction: WallConstruction.Other,
                entryPoint: QuestionFlowStepOld.WallConstruction
            ),
            QuestionFlowStepOld.AnswerSummary),
        new(
            "Changing cavity walls insulated continues to summary",
            new Input(
                QuestionFlowStepOld.CavityWallsInsulated,
                "ABCDEFGH",
                entryPoint: QuestionFlowStepOld.CavityWallsInsulated
            ),
            QuestionFlowStepOld.AnswerSummary),
        new(
            "Cavity walls insulated continue to solid walls insulated if user has mixed walls",
            new Input(
                QuestionFlowStepOld.CavityWallsInsulated,
                "ABCDEFGH",
                wallConstruction: WallConstruction.Mixed
            ),
            QuestionFlowStepOld.SolidWallsInsulated),
        new(
            "Cavity walls insulated continue to floor construction if user only does not have solid walls and can answer floor questions",
            new Input(
                QuestionFlowStepOld.CavityWallsInsulated,
                "ABCDEFGH",
                propertyType: PropertyType.House,
                wallConstruction: WallConstruction.Cavity
            ),
            QuestionFlowStepOld.FloorConstruction),
        new(
            "Cavity walls insulated continue to roof construction if user only does not have solid walls, can not answer floor questions but can answer roof questions",
            new Input(
                QuestionFlowStepOld.CavityWallsInsulated,
                "ABCDEFGH",
                propertyType: PropertyType.ApartmentFlatOrMaisonette,
                flatType: FlatType.TopFloor,
                wallConstruction: WallConstruction.Cavity
            ),
            QuestionFlowStepOld.RoofConstruction),
        new(
            "Cavity walls insulated continue to glazing type if user only does not have solid walls and can not answer neither floor questions nor roof questions",
            new Input(
                QuestionFlowStepOld.CavityWallsInsulated,
                "ABCDEFGH",
                propertyType: PropertyType.ApartmentFlatOrMaisonette,
                flatType: FlatType.MiddleFloor,
                wallConstruction: WallConstruction.Cavity
            ),
            QuestionFlowStepOld.GlazingType),
        new(
            "Changing solid walls insulated continues to summary",
            new Input(
                QuestionFlowStepOld.SolidWallsInsulated,
                "ABCDEFGH",
                entryPoint: QuestionFlowStepOld.SolidWallsInsulated
            ),
            QuestionFlowStepOld.AnswerSummary),
        new(
            "Solid walls insulated continue to floor construction if user can answer floor questions",
            new Input(
                QuestionFlowStepOld.SolidWallsInsulated,
                "ABCDEFGH",
                propertyType: PropertyType.House
            ),
            QuestionFlowStepOld.FloorConstruction),
        new(
            "Solid walls insulated continue to roof construction if user can not answer floor questions but can answer roof ones",
            new Input(
                QuestionFlowStepOld.SolidWallsInsulated,
                "ABCDEFGH",
                propertyType: PropertyType.ApartmentFlatOrMaisonette,
                flatType: FlatType.TopFloor
            ),
            QuestionFlowStepOld.RoofConstruction),
        new(
            "Solid walls insulated continue to glazing type if user can not answer neither floor questions nor roof ones",
            new Input(
                QuestionFlowStepOld.SolidWallsInsulated,
                "ABCDEFGH",
                propertyType: PropertyType.ApartmentFlatOrMaisonette,
                flatType: FlatType.MiddleFloor
            ),
            QuestionFlowStepOld.GlazingType),
        new(
            "Floor construction continues to floor insulated if the floors are known",
            new Input(
                QuestionFlowStepOld.FloorConstruction,
                "ABCDEFGH",
                floorConstruction: FloorConstruction.Mix
            ),
            QuestionFlowStepOld.FloorInsulated),
        new(
            "Changing floor construction continues to summary if floor are not known",
            new Input(
                QuestionFlowStepOld.FloorConstruction,
                "ABCDEFGH",
                floorConstruction: FloorConstruction.Other,
                entryPoint: QuestionFlowStepOld.FloorConstruction
            ),
            QuestionFlowStepOld.AnswerSummary),
        new(
            "Floor construction continues to roof construction if floors are unknown and the user can answer roof questions",
            new Input(
                QuestionFlowStepOld.FloorConstruction,
                "ABCDEFGH",
                propertyType: PropertyType.House,
                floorConstruction: FloorConstruction.Other
            ),
            QuestionFlowStepOld.RoofConstruction),
        new(
            "Floor construction continues to glazing type if floor are unknown and the user can not answer roof questions",
            new Input(
                QuestionFlowStepOld.FloorConstruction,
                "ABCDEFGH",
                propertyType: PropertyType.ApartmentFlatOrMaisonette,
                flatType: FlatType.MiddleFloor,
                floorConstruction: FloorConstruction.Other
            ),
            QuestionFlowStepOld.GlazingType),
        new(
            "Changing floor insulated continues to summary",
            new Input(
                QuestionFlowStepOld.FloorInsulated,
                "ABCDEFGH",
                entryPoint: QuestionFlowStepOld.FloorInsulated
            ),
            QuestionFlowStepOld.AnswerSummary),
        new(
            "Floor insulated continues to roof construction if user can answer roof questions",
            new Input(
                QuestionFlowStepOld.FloorInsulated,
                "ABCDEFGH",
                propertyType: PropertyType.House
            ),
            QuestionFlowStepOld.RoofConstruction),
        new(
            "Floor insulated continues to glazing type if user can not answer roof questions",
            new Input(
                QuestionFlowStepOld.FloorInsulated,
                "ABCDEFGH",
                propertyType: PropertyType.ApartmentFlatOrMaisonette,
                flatType: FlatType.MiddleFloor
            ),
            QuestionFlowStepOld.GlazingType),
        new(
            "Roof construction continues to loft space if roof is pitched",
            new Input(
                QuestionFlowStepOld.RoofConstruction,
                "ABCDEFGH",
                roofConstruction: RoofConstruction.Pitched
            ),
            QuestionFlowStepOld.LoftSpace),
        new(
            "Roof construction continues to loft space if roof is mixed",
            new Input(
                QuestionFlowStepOld.RoofConstruction,
                "ABCDEFGH",
                roofConstruction: RoofConstruction.Mixed
            ),
            QuestionFlowStepOld.LoftSpace),
        new(
            "Changing roof construction continues to summary if roof is not pitched",
            new Input(
                QuestionFlowStepOld.RoofConstruction,
                "ABCDEFGH",
                roofConstruction: RoofConstruction.Flat,
                entryPoint: QuestionFlowStepOld.RoofConstruction
            ),
            QuestionFlowStepOld.AnswerSummary),
        new(
            "Roof construction continues to glazing type space if roof is not pitched",
            new Input(
                QuestionFlowStepOld.RoofConstruction,
                "ABCDEFGH",
                roofConstruction: RoofConstruction.Flat
            ),
            QuestionFlowStepOld.GlazingType),
        new(
            "Loft space continues to loft access if user has it",
            new Input(
                QuestionFlowStepOld.LoftSpace,
                "ABCDEFGH",
                loftSpace: LoftSpace.Yes
            ),
            QuestionFlowStepOld.LoftAccess),
        new(
            "Changing loft space continues to answer summary if user does not have loft space",
            new Input(
                QuestionFlowStepOld.LoftSpace,
                "ABCDEFGH",
                loftSpace: LoftSpace.No,
                entryPoint: QuestionFlowStepOld.LoftSpace
            ),
            QuestionFlowStepOld.AnswerSummary),
        new(
            "Loft space continues to glazing type if user does not have it",
            new Input(
                QuestionFlowStepOld.LoftSpace,
                "ABCDEFGH",
                loftSpace: LoftSpace.No
            ),
            QuestionFlowStepOld.GlazingType),
        new(
            "Loft access continues to roof insulation if user has it",
            new Input(
                QuestionFlowStepOld.LoftAccess,
                "ABCDEFGH",
                accessibleLoft: LoftAccess.Yes
            ),
            QuestionFlowStepOld.RoofInsulated),
        new(
            "Changing loft access continues to roof insulation if user has it",
            new Input(
                QuestionFlowStepOld.LoftAccess,
                "ABCDEFGH",
                accessibleLoft: LoftAccess.Yes,
                entryPoint: QuestionFlowStepOld.LoftAccess
            ),
            QuestionFlowStepOld.RoofInsulated),
        new(
            "Changing loft access continues to answer summary if user does not have it",
            new Input(
                QuestionFlowStepOld.LoftAccess,
                "ABCDEFGH",
                accessibleLoft: LoftAccess.No,
                entryPoint: QuestionFlowStepOld.LoftAccess
            ),
            QuestionFlowStepOld.AnswerSummary),
        new(
            "Loft access continues to glazing type if user does not have it",
            new Input(
                QuestionFlowStepOld.LoftAccess,
                "ABCDEFGH",
                loftSpace: LoftSpace.No
            ),
            QuestionFlowStepOld.GlazingType),
        new(
            "Roof insulated continues to glazing type",
            new Input(
                QuestionFlowStepOld.RoofInsulated,
                "ABCDEFGH"
            ),
            QuestionFlowStepOld.GlazingType),
        new(
            "Changing roof insulated continues to summary",
            new Input(
                QuestionFlowStepOld.RoofInsulated,
                "ABCDEFGH",
                entryPoint: QuestionFlowStepOld.RoofInsulated
            ),
            QuestionFlowStepOld.AnswerSummary),
        new(
            "Glazing type continues to outdoor space",
            new Input(
                QuestionFlowStepOld.GlazingType,
                "ABCDEFGH"
            ),
            QuestionFlowStepOld.OutdoorSpace),
        new(
            "Changing glazing type continues to summary",
            new Input(
                QuestionFlowStepOld.GlazingType,
                "ABCDEFGH",
                entryPoint: QuestionFlowStepOld.GlazingType
            ),
            QuestionFlowStepOld.AnswerSummary),
        new(
            "Outdoor space continues to heating type",
            new Input(
                QuestionFlowStepOld.OutdoorSpace,
                "ABCDEFGH"
            ),
            QuestionFlowStepOld.HeatingType),
        new(
            "Changing outdoor space continues to summary",
            new Input(
                QuestionFlowStepOld.OutdoorSpace,
                "ABCDEFGH",
                entryPoint: QuestionFlowStepOld.OutdoorSpace
            ),
            QuestionFlowStepOld.AnswerSummary),
        new(
            "Heating type continues to other heating type in that scenario",
            new Input(
                QuestionFlowStepOld.HeatingType,
                "ABCDEFGH",
                heatingType: HeatingType.Other
            ),
            QuestionFlowStepOld.OtherHeatingType),
        new(
            "Heating type continues to hot water cylinder if user has a boiler",
            new Input(
                QuestionFlowStepOld.HeatingType,
                "ABCDEFGH",
                heatingType: HeatingType.GasBoiler
            ),
            QuestionFlowStepOld.HotWaterCylinder),
        new(
            "Changing heating type continues to summary if user doesn't have a boiler or other heating type",
            new Input(
                QuestionFlowStepOld.HeatingType,
                "ABCDEFGH",
                heatingType: HeatingType.HeatPump,
                entryPoint: QuestionFlowStepOld.HeatingType
            ),
            QuestionFlowStepOld.AnswerSummary),
        new(
            "Heating type continues to number of occupants if user doesn't have a boiler or other heating",
            new Input(
                QuestionFlowStepOld.HeatingType,
                "ABCDEFGH",
                heatingType: HeatingType.HeatPump
            ),
            QuestionFlowStepOld.NumberOfOccupants),
        new(
            "Other heating type continues to number of occupants",
            new Input(
                QuestionFlowStepOld.OtherHeatingType,
                "ABCDEFGH"
            ),
            QuestionFlowStepOld.NumberOfOccupants),
        new(
            "Changing other heating type continues to summary",
            new Input(
                QuestionFlowStepOld.OtherHeatingType,
                "ABCDEFGH",
                entryPoint: QuestionFlowStepOld.OtherHeatingType
            ),
            QuestionFlowStepOld.AnswerSummary),
        new(
            "Hot water cylinder continues to number of occupants",
            new Input(
                QuestionFlowStepOld.HotWaterCylinder,
                "ABCDEFGH"
            ),
            QuestionFlowStepOld.NumberOfOccupants),
        new(
            "Changing hot water cylinder continues to summary",
            new Input(
                QuestionFlowStepOld.HotWaterCylinder,
                "ABCDEFGH",
                entryPoint: QuestionFlowStepOld.HotWaterCylinder
            ),
            QuestionFlowStepOld.AnswerSummary),
        new(
            "Number of occupants continues to heating pattern",
            new Input(
                QuestionFlowStepOld.NumberOfOccupants,
                "ABCDEFGH"
            ),
            QuestionFlowStepOld.HeatingPattern),
        new(
            "Changing number of occupants continues to summary",
            new Input(
                QuestionFlowStepOld.NumberOfOccupants,
                "ABCDEFGH",
                entryPoint: QuestionFlowStepOld.NumberOfOccupants
            ),
            QuestionFlowStepOld.AnswerSummary),
        new(
            "Heating pattern continues to temperature",
            new Input(
                QuestionFlowStepOld.HeatingPattern,
                "ABCDEFGH"
            ),
            QuestionFlowStepOld.Temperature),
        new(
            "Changing heating pattern continues to summary",
            new Input(
                QuestionFlowStepOld.HeatingPattern,
                "ABCDEFGH",
                entryPoint: QuestionFlowStepOld.HeatingPattern
            ),
            QuestionFlowStepOld.AnswerSummary),
        new(
            "Temperature continues to summary",
            new Input(
                QuestionFlowStepOld.Temperature,
                "ABCDEFGH"
            ),
            QuestionFlowStepOld.AnswerSummary),
        new(
            "Answer summary continues to your recommendations if user has at least one",
            new Input(
                QuestionFlowStepOld.AnswerSummary,
                "ABCDEFGH",
                propertyRecommendations: new List<PropertyRecommendation> {new()}
            ),
            QuestionFlowStepOld.YourRecommendations),
        new(
            "Answer summary continues to no recommendations if user has none",
            new Input(
                QuestionFlowStepOld.AnswerSummary,
                "ABCDEFGH",
                propertyRecommendations: new List<PropertyRecommendation>()),
            QuestionFlowStepOld.NoRecommendations)
    };

    private static QuestionFlowServiceTestCase[] SkipTestCases =
    {
        new(
            "Postcode skips to property type",
            new Input(
                QuestionFlowStepOld.AskForPostcode,
                "ABCDEFGH"
            ),
            QuestionFlowStepOld.PropertyType)
    };

    public class QuestionFlowServiceTestCase
    {
        public readonly string Description;
        public readonly QuestionFlowStepOld ExpectedOutput;
        public readonly Input Input;

        public QuestionFlowServiceTestCase(
            string description,
            Input input,
            QuestionFlowStepOld expectedOutput
        )
        {
            Description = description;
            Input = input;
            ExpectedOutput = expectedOutput;
        }

        public override string ToString()
        {
            return Description;
        }
    }

    public class Input
    {
        public readonly QuestionFlowStepOld Page;
        public readonly PropertyData PropertyData;
        public QuestionFlowStepOld? EntryPoint;

        public Input(
            QuestionFlowStepOld page,
            string reference = null,
            OwnershipStatus? ownershipStatus = null,
            Country? country = null,
            Epc epc = null,
            SearchForEpc? searchForEpc = null,
            EpcDetailsConfirmed? epcDetailsConfirmed = null,
            PropertyType? propertyType = null,
            HouseType? houseType = null,
            BungalowType? bungalowType = null,
            FlatType? flatType = null,
            YearBuilt? yearBuilt = null,
            WallConstruction? wallConstruction = null,
            CavityWallsInsulated? cavityWallsInsulated = null,
            SolidWallsInsulated? solidWallsInsulated = null,
            FloorConstruction? floorConstruction = null,
            FloorInsulated? floorInsulated = null,
            RoofConstruction? roofConstruction = null,
            LoftSpace? loftSpace = null,
            LoftAccess? accessibleLoft = null,
            RoofInsulated? roofInsulated = null,
            HasOutdoorSpace? hasOutdoorSpace = null,
            GlazingType? glazingType = null,
            HeatingType? heatingType = null,
            OtherHeatingType? otherHeatingType = null,
            HasHotWaterCylinder? hasHotWaterCylinder = null,
            int? numberOfOccupants = null,
            HeatingPattern? heatingPattern = null,
            int? hoursOfHeatingMorning = null,
            int? hoursOfHeatingEvening = null,
            decimal? temperature = null,
            QuestionFlowStepOld? entryPoint = null,
            List<PropertyRecommendation> propertyRecommendations = null)
        {
            Page = page;
            PropertyData = new PropertyData
            {
                Reference = reference,
                OwnershipStatus = ownershipStatus,
                Country = country,
                Epc = epc,
                SearchForEpc = searchForEpc,
                EpcDetailsConfirmed = epcDetailsConfirmed,
                PropertyType = propertyType,
                HouseType = houseType,
                BungalowType = bungalowType,
                FlatType = flatType,
                YearBuilt = yearBuilt,
                WallConstruction = wallConstruction,
                CavityWallsInsulated = cavityWallsInsulated,
                SolidWallsInsulated = solidWallsInsulated,
                FloorConstruction = floorConstruction,
                FloorInsulated = floorInsulated,
                RoofConstruction = roofConstruction,
                LoftSpace = loftSpace,
                LoftAccess = accessibleLoft,
                RoofInsulated = roofInsulated,
                HasOutdoorSpace = hasOutdoorSpace,
                GlazingType = glazingType,
                HeatingType = heatingType,
                OtherHeatingType = otherHeatingType,
                HasHotWaterCylinder = hasHotWaterCylinder,
                NumberOfOccupants = numberOfOccupants,
                HeatingPattern = heatingPattern,
                HoursOfHeatingMorning = hoursOfHeatingMorning,
                HoursOfHeatingEvening = hoursOfHeatingEvening,
                Temperature = temperature,
                PropertyRecommendations = propertyRecommendations
            };
            EntryPoint = entryPoint;
        }
    }
}