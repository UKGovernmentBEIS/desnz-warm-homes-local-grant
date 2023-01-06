using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
using SeaPublicWebsite.BusinessLogic.Models;
using SeaPublicWebsite.BusinessLogic.Models.Enums;
using SeaPublicWebsite.BusinessLogic.Services;

namespace Tests.BusinessLogic.Services;

[TestFixture]
public class QuestionFlowServiceTests
{
    [SetUp]
    public void Setup()
    {
        QuestionFlowService = new QuestionFlowService();
    }

    private IQuestionFlowService QuestionFlowService;

    [TestCaseSource(nameof(BackTestCases))]
    public void RunBackLinkTestCases(QuestionFlowServiceTestCase testCase)
    {
        // Act
        var output = QuestionFlowService.PreviousStep(
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
        var output = QuestionFlowService.NextStep(
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
        var output = QuestionFlowService.SkipDestination(
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
                QuestionFlowStep.NewOrReturningUser
            ),
            QuestionFlowStep.Start),
        new(
            "Country goes back to new or returning user",
            new Input(
                QuestionFlowStep.Country
            ),
            QuestionFlowStep.NewOrReturningUser),
        new(
            "Ownership status goes back to Country",
            new Input(
                QuestionFlowStep.OwnershipStatus,
                "ABCDEFGH"
            ),
            QuestionFlowStep.Country),
        new(
            "Service unsuitable goes back to the country you came from",
            new Input(
                QuestionFlowStep.ServiceUnsuitable,
                "ABCDEFGH",
                country: Country.Other
            ),
            QuestionFlowStep.Country),
        new(
            "Service unsuitable goes back to ownership status if user is a private tenant",
            new Input(
                QuestionFlowStep.ServiceUnsuitable,
                "ABCDEFGH",
                country: Country.England,
                ownershipStatus: OwnershipStatus.PrivateTenancy
            ),
            QuestionFlowStep.OwnershipStatus),
        new(
            "Find EPC goes back to ownership status",
            new Input(
                QuestionFlowStep.FindEpc,
                "ABCDEFGH"
            ),
            QuestionFlowStep.OwnershipStatus),
        new(
            "Postcode goes back to find EPC",
            new Input(
                QuestionFlowStep.AskForPostcode,
                "ABCDEFGH"
            ),
            QuestionFlowStep.FindEpc),
        new(
            "Confirm address goes back to postcode",
            new Input(
                QuestionFlowStep.ConfirmAddress,
                "ABCDEFGH"
            ),
            QuestionFlowStep.AskForPostcode),
        new(
            "Confirm EPC details goes back to ask for postcode",
            new Input(
                QuestionFlowStep.ConfirmEpcDetails,
                "ABCDEFGH"
            ),
            QuestionFlowStep.AskForPostcode),
        new(
            "No EPC found goes back to postcode",
            new Input(
                QuestionFlowStep.NoEpcFound,
                "ABCDEFGH"
            ),
            QuestionFlowStep.AskForPostcode),
        new(
            "Property type goes back to confirm EPC details if epc exists with property type and age",
            new Input(
                QuestionFlowStep.PropertyType,
                "ABCDEFGH",
                epc: new Epc
                {
                    PropertyType = PropertyType.House,
                    HouseType = HouseType.Detached,
                    ConstructionAgeBand = HomeAge.From1950To1966
                }
            ),
            QuestionFlowStep.ConfirmEpcDetails),
        new(
            "Property type goes back to postcode if epc doesn't contain property type",
            new Input(
                QuestionFlowStep.PropertyType,
                "ABCDEFGH",
                epc: new Epc
                {
                    ConstructionAgeBand = HomeAge.From1950To1966
                }
            ),
            QuestionFlowStep.AskForPostcode),
        new(
            "Property type goes back to postcode if epc doesn't contain property age",
            new Input(
                QuestionFlowStep.PropertyType,
                "ABCDEFGH",
                epc: new Epc()
                {
                    PropertyType = PropertyType.House,
                    HouseType = HouseType.Detached
                }
            ),
            QuestionFlowStep.AskForPostcode),
        new(
            "Property type goes back to no EPC found if searched for EPC but no EPC set",
            new Input(
                QuestionFlowStep.PropertyType,
                "ABCDEFGH",
                epc: null,
                searchForEpc: SearchForEpc.Yes
            ),
            QuestionFlowStep.NoEpcFound),
        new(
            "Property type goes back to find EPC if didn't search for EPC",
            new Input(
                QuestionFlowStep.PropertyType,
                "ABCDEFGH",
                searchForEpc: SearchForEpc.No
            ),
            QuestionFlowStep.FindEpc),
        new(
            "Changing property type goes back to check your unchangeable answers",
            new Input(
                QuestionFlowStep.PropertyType,
                "ABCDEFGH",
                entryPoint: QuestionFlowStep.PropertyType
            ),
            QuestionFlowStep.CheckYourUnchangeableAnswers),
        new(
            "House type goes back to property type",
            new Input(
                QuestionFlowStep.HouseType,
                "ABCDEFGH"
            ),
            QuestionFlowStep.PropertyType),
        new(
            "Bungalow type goes back to property type",
            new Input(
                QuestionFlowStep.BungalowType,
                "ABCDEFGH"
            ),
            QuestionFlowStep.PropertyType),
        new(
            "Flat type goes back to property type",
            new Input(
                QuestionFlowStep.FlatType,
                "ABCDEFGH"
            ),
            QuestionFlowStep.PropertyType),
        new(
            "Home age goes back to the property type it came from (house)",
            new Input(
                QuestionFlowStep.HomeAge,
                "ABCDEFGH",
                propertyType: PropertyType.House
            ),
            QuestionFlowStep.HouseType),
        new(
            "Home age goes back to the property type it came from (bungalow)",
            new Input(
                QuestionFlowStep.HomeAge,
                "ABCDEFGH",
                propertyType: PropertyType.Bungalow
            ),
            QuestionFlowStep.BungalowType),
        new(
            "Home age goes back to the property type it came from (flat)",
            new Input(
                QuestionFlowStep.HomeAge,
                "ABCDEFGH",
                propertyType: PropertyType.ApartmentFlatOrMaisonette
            ),
            QuestionFlowStep.FlatType),
        new(
            "Changing home age goes back to check your unchangeable answers",
            new Input(
                QuestionFlowStep.HomeAge,
                "ABCDEFGH",
                entryPoint: QuestionFlowStep.HomeAge
            ),
            QuestionFlowStep.CheckYourUnchangeableAnswers),
        new(
            "Wall construction goes back to check your unchangeable answers if EPC details were not confirmed",
            new Input(
                QuestionFlowStep.WallConstruction,
                "ABCDEFGH",
                epcDetailsConfirmed: EpcDetailsConfirmed.No

            ),
            QuestionFlowStep.CheckYourUnchangeableAnswers),
        new(
            "Wall construction goes back to confirm EPC details if EPC details were confirmed",
            new Input(
                QuestionFlowStep.WallConstruction,
                "ABCDEFGH",
                epcDetailsConfirmed: EpcDetailsConfirmed.Yes
            ),
            QuestionFlowStep.ConfirmEpcDetails),
        new(
            "Changing wall construction goes back to summary",
            new Input(
                QuestionFlowStep.WallConstruction,
                "ABCDEFGH",
                entryPoint: QuestionFlowStep.WallConstruction
            ),
            QuestionFlowStep.AnswerSummary),
        new(
            "Cavity walls insulated goes back to wall construction",
            new Input(
                QuestionFlowStep.CavityWallsInsulated,
                "ABCDEFGH"
            ),
            QuestionFlowStep.WallConstruction),
        new(
            "Changing cavity walls insulated goes back to summary",
            new Input(
                QuestionFlowStep.CavityWallsInsulated,
                "ABCDEFGH",
                entryPoint: QuestionFlowStep.CavityWallsInsulated
            ),
            QuestionFlowStep.AnswerSummary),
        new(
            "Solid walls insulated goes back to cavity walls insulated if user has mixed walls",
            new Input(
                QuestionFlowStep.SolidWallsInsulated,
                "ABCDEFGH",
                wallConstruction: WallConstruction.Mixed
            ),
            QuestionFlowStep.CavityWallsInsulated),
        new(
            "Solid walls insulated goes back to wall construction if user does not have mixed walls",
            new Input(
                QuestionFlowStep.SolidWallsInsulated,
                "ABCDEFGH",
                wallConstruction: WallConstruction.Solid
            ),
            QuestionFlowStep.WallConstruction),
        new(
            "Changing solid walls insulated goes back to summary",
            new Input(
                QuestionFlowStep.SolidWallsInsulated,
                "ABCDEFGH",
                entryPoint: QuestionFlowStep.SolidWallsInsulated
            ),
            QuestionFlowStep.AnswerSummary),
        new(
            "Floor construction goes back to the wall insulation the user answered last (solid walls)",
            new Input(
                QuestionFlowStep.FloorConstruction,
                "ABCDEFGH",
                wallConstruction: WallConstruction.Solid
            ),
            QuestionFlowStep.SolidWallsInsulated),
        new(
            "Floor construction goes back to the wall insulation the user answered last (cavity walls)",
            new Input(
                QuestionFlowStep.FloorConstruction,
                "ABCDEFGH",
                wallConstruction: WallConstruction.Cavity
            ),
            QuestionFlowStep.CavityWallsInsulated),
        new(
            "Floor construction goes back to wall construction if user has neither cavity not solid walls",
            new Input(
                QuestionFlowStep.FloorConstruction,
                "ABCDEFGH",
                wallConstruction: WallConstruction.Other
            ),
            QuestionFlowStep.WallConstruction),
        new(
            "Changing floor construction goes back to summary",
            new Input(
                QuestionFlowStep.FloorConstruction,
                "ABCDEFGH",
                entryPoint: QuestionFlowStep.FloorConstruction
            ),
            QuestionFlowStep.AnswerSummary),
        new(
            "Floor insulated goes back to floor construction",
            new Input(
                QuestionFlowStep.FloorInsulated,
                "ABCDEFGH"
            ),
            QuestionFlowStep.FloorConstruction),
        new(
            "Changing floor insulated goes back to summary",
            new Input(
                QuestionFlowStep.FloorInsulated,
                "ABCDEFGH",
                entryPoint: QuestionFlowStep.FloorInsulated
            ),
            QuestionFlowStep.AnswerSummary),
        new(
            "Roof construction goes back to floor insulation if the user has timber or concrete floors",
            new Input(
                QuestionFlowStep.RoofConstruction,
                propertyType: PropertyType.House,
                floorConstruction: FloorConstruction.SuspendedTimber,
                reference: "ABCDEFGH"
            ),
            QuestionFlowStep.FloorInsulated),
        new(
            "Roof construction goes back to floor construction if the user has different floors",
            new Input(
                QuestionFlowStep.RoofConstruction,
                propertyType: PropertyType.House,
                floorConstruction: FloorConstruction.Other,
                reference: "ABCDEFGH"
            ),
            QuestionFlowStep.FloorConstruction),
        new(
            "Roof construction goes back to wall insulation if user was not asked about their floor but has insulated walls",
            new Input(
                QuestionFlowStep.RoofConstruction,
                propertyType: PropertyType.ApartmentFlatOrMaisonette,
                flatType: FlatType.TopFloor,
                wallConstruction: WallConstruction.Solid,
                reference: "ABCDEFGH"
            ),
            QuestionFlowStep.SolidWallsInsulated),
        new(
            "Roof construction goes back to wall construction if user was not asked about their floor and has other walls",
            new Input(
                QuestionFlowStep.RoofConstruction,
                propertyType: PropertyType.ApartmentFlatOrMaisonette,
                flatType: FlatType.TopFloor,
                wallConstruction: WallConstruction.DoNotKnow,
                reference: "ABCDEFGH"
            ),
            QuestionFlowStep.WallConstruction),
        new(
            "Changing roof construction goes back to summary",
            new Input(
                QuestionFlowStep.RoofConstruction,
                "ABCDEFGH",
                entryPoint: QuestionFlowStep.RoofConstruction
            ),
            QuestionFlowStep.AnswerSummary),
        new(
            "Loft space goes back to roof construction",
            new Input(
                QuestionFlowStep.LoftSpace,
                "ABCDEFGH"
            ),
            QuestionFlowStep.RoofConstruction),
        new(
            "Changing loft space goes back to summary",
            new Input(
                QuestionFlowStep.LoftSpace,
                "ABCDEFGH",
                entryPoint: QuestionFlowStep.LoftSpace
            ),
            QuestionFlowStep.AnswerSummary),
        new(
            "Loft access goes back to loft space",
            new Input(
                QuestionFlowStep.LoftAccess,
                "ABCDEFGH"
            ),
            QuestionFlowStep.LoftSpace),
        new(
            "Changing loft access goes back to summary",
            new Input(
                QuestionFlowStep.LoftAccess,
                "ABCDEFGH",
                entryPoint: QuestionFlowStep.LoftAccess
            ),
            QuestionFlowStep.AnswerSummary),
        new(
            "Roof insulated goes back to loft access",
            new Input(
                QuestionFlowStep.RoofInsulated,
                "ABCDEFGH"
            ),
            QuestionFlowStep.LoftAccess),
        new(
            "Changing roof insulated goes back to summary",
            new Input(
                QuestionFlowStep.RoofInsulated,
                "ABCDEFGH",
                entryPoint: QuestionFlowStep.RoofInsulated
            ),
            QuestionFlowStep.AnswerSummary),
        new(
            "Glazing type goes back to roof construction if the user has flat roof",
            new Input(
                QuestionFlowStep.GlazingType,
                "ABCDEFGH",
                propertyType: PropertyType.House,
                roofConstruction: RoofConstruction.Flat
            ),
            QuestionFlowStep.RoofConstruction),
        new(
            "Glazing type goes back to loft space if the user does not have flat roof nor loft space",
            new Input(
                QuestionFlowStep.GlazingType,
                "ABCDEFGH",
                propertyType: PropertyType.House,
                roofConstruction: RoofConstruction.Pitched,
                loftSpace: LoftSpace.No
            ),
            QuestionFlowStep.LoftSpace),
        new(
            "Glazing type goes back to loft access if the user does not have flat roof and has inaccessible loft space",
            new Input(
                QuestionFlowStep.GlazingType,
                "ABCDEFGH",
                propertyType: PropertyType.House,
                roofConstruction: RoofConstruction.Pitched,
                loftSpace: LoftSpace.Yes,
                accessibleLoft: LoftAccess.No
            ),
            QuestionFlowStep.LoftAccess),
        new(
            "Glazing type goes back to roof insulation if the user does not have flat roof but has accessible loft space",
            new Input(
                QuestionFlowStep.GlazingType,
                "ABCDEFGH",
                propertyType: PropertyType.House,
                roofConstruction: RoofConstruction.Pitched,
                loftSpace: LoftSpace.Yes,
                accessibleLoft: LoftAccess.Yes
            ),
            QuestionFlowStep.RoofInsulated),
        new(
            "Glazing type goes back to floor construction if the user was not asked about their roof",
            new Input(
                QuestionFlowStep.GlazingType,
                propertyType: PropertyType.ApartmentFlatOrMaisonette,
                flatType: FlatType.GroundFloor,
                reference: "ABCDEFGH"
            ),
            QuestionFlowStep.FloorConstruction),
        new(
            "Glazing type goes back to wall construction if the user was not asked about neither their roof or floor",
            new Input(
                QuestionFlowStep.GlazingType,
                propertyType: PropertyType.ApartmentFlatOrMaisonette,
                flatType: FlatType.MiddleFloor,
                reference: "ABCDEFGH"
            ),
            QuestionFlowStep.WallConstruction),
        new(
            "Changing glazing type goes back to summary",
            new Input(
                QuestionFlowStep.GlazingType,
                "ABCDEFGH",
                entryPoint: QuestionFlowStep.GlazingType
            ),
            QuestionFlowStep.AnswerSummary),
        new(
            "Outdoor space goes back to glazing type",
            new Input(
                QuestionFlowStep.OutdoorSpace,
                "ABCDEFGH"
            ),
            QuestionFlowStep.GlazingType),
        new(
            "Changing outdoor space goes back to summary",
            new Input(
                QuestionFlowStep.OutdoorSpace,
                "ABCDEFGH",
                entryPoint: QuestionFlowStep.OutdoorSpace
            ),
            QuestionFlowStep.AnswerSummary),
        new(
            "Heating type goes back to outdoor space",
            new Input(
                QuestionFlowStep.HeatingType,
                "ABCDEFGH"
            ),
            QuestionFlowStep.OutdoorSpace),
        new(
            "Changing heating type goes back to summary",
            new Input(
                QuestionFlowStep.HeatingType,
                "ABCDEFGH",
                entryPoint: QuestionFlowStep.HeatingType
            ),
            QuestionFlowStep.AnswerSummary),
        new(
            "Other heating type goes back to outdoor space",
            new Input(
                QuestionFlowStep.OtherHeatingType,
                "ABCDEFGH"
            ),
            QuestionFlowStep.HeatingType),
        new(
            "Changing other heating type goes back to summary",
            new Input(
                QuestionFlowStep.OtherHeatingType,
                "ABCDEFGH",
                entryPoint: QuestionFlowStep.OtherHeatingType
            ),
            QuestionFlowStep.AnswerSummary),
        new(
            "Hot water cylinder goes back to heating type",
            new Input(
                QuestionFlowStep.HotWaterCylinder,
                "ABCDEFGH"
            ),
            QuestionFlowStep.HeatingType),
        new(
            "Changing hot water cylinder goes back to summary",
            new Input(
                QuestionFlowStep.HotWaterCylinder,
                "ABCDEFGH",
                entryPoint: QuestionFlowStep.HotWaterCylinder
            ),
            QuestionFlowStep.AnswerSummary),
        new(
            "Number of occupants goes back to hot water cylinder if user has boiler",
            new Input(
                QuestionFlowStep.NumberOfOccupants,
                "ABCDEFGH",
                heatingType: HeatingType.GasBoiler
            ),
            QuestionFlowStep.HotWaterCylinder),
        new(
            "Number of occupants goes back to other heating type if the user selected so",
            new Input(
                QuestionFlowStep.NumberOfOccupants,
                "ABCDEFGH",
                heatingType: HeatingType.Other
            ),
            QuestionFlowStep.OtherHeatingType),
        new(
            "Number of occupants goes back to heating type if the user does not have a boiler",
            new Input(
                QuestionFlowStep.NumberOfOccupants,
                "ABCDEFGH",
                heatingType: HeatingType.HeatPump
            ),
            QuestionFlowStep.HeatingType),
        new(
            "Changing number of occupants goes back to summary",
            new Input(
                QuestionFlowStep.NumberOfOccupants,
                "ABCDEFGH",
                entryPoint: QuestionFlowStep.NumberOfOccupants
            ),
            QuestionFlowStep.AnswerSummary),
        new(
            "Heating pattern goes back to number of occupants",
            new Input(
                QuestionFlowStep.HeatingPattern,
                "ABCDEFGH"
            ),
            QuestionFlowStep.NumberOfOccupants),
        new(
            "Changing heating pattern goes back to summary",
            new Input(
                QuestionFlowStep.HeatingPattern,
                "ABCDEFGH",
                entryPoint: QuestionFlowStep.HeatingPattern
            ),
            QuestionFlowStep.AnswerSummary),
        new(
            "Temperature goes back to heating pattern",
            new Input(
                QuestionFlowStep.Temperature,
                "ABCDEFGH"
            ),
            QuestionFlowStep.HeatingPattern),
        new(
            "Changing temperature goes back to summary",
            new Input(
                QuestionFlowStep.Temperature,
                "ABCDEFGH",
                entryPoint: QuestionFlowStep.Temperature
            ),
            QuestionFlowStep.AnswerSummary),
        new(
            "Answer summary goes back to temperature",
            new Input(
                QuestionFlowStep.AnswerSummary,
                "ABCDEFGH"
            ),
            QuestionFlowStep.Temperature),
        new(
            "No recommendations goes back to summary",
            new Input(
                QuestionFlowStep.NoRecommendations,
                "ABCDEFGH"
            ),
            QuestionFlowStep.AnswerSummary),
        new(
            "Your recommendations goes back to summary",
            new Input(
                QuestionFlowStep.YourRecommendations,
                "ABCDEFGH"
            ),
            QuestionFlowStep.AnswerSummary)
    };

    private static QuestionFlowServiceTestCase[] ForwardTestCases =
    {
        new(
            "Country continues to service unsuitable if the service is not available",
            new Input(
                QuestionFlowStep.Country,
                "ABCDEFGH",
                country: Country.Other
            ),
            QuestionFlowStep.ServiceUnsuitable),
        new(
            "Country continues to ownership status",
            new Input(
                QuestionFlowStep.Country,
                "ABCDEFGH",
                country: Country.England
            ),
            QuestionFlowStep.OwnershipStatus),
        new(
            "Ownership status continues to service unsuitable if user is a private tenant",
            new Input(
                QuestionFlowStep.OwnershipStatus,
                "ABCDEFGH",
                OwnershipStatus.PrivateTenancy
            ),
            QuestionFlowStep.ServiceUnsuitable),
        new(
            "Ownership status continues to find EPC",
            new Input(
                QuestionFlowStep.OwnershipStatus,
                "ABCDEFGH",
                OwnershipStatus.OwnerOccupancy
            ),
            QuestionFlowStep.FindEpc),
        new(
            "Find EPC continues to postcode if yes selected",
            new Input(
                QuestionFlowStep.FindEpc,
                "ABCDEFGH",
                searchForEpc: SearchForEpc.Yes
            ),
            QuestionFlowStep.AskForPostcode),
        new(
            "Find EPC continues to property type if no selected",
            new Input(
                QuestionFlowStep.FindEpc,
                "ABCDEFGH",
                searchForEpc: SearchForEpc.No
            ),
            QuestionFlowStep.PropertyType),
        new(
            "Postcode continues to confirm address",
            new Input(
                QuestionFlowStep.AskForPostcode,
                "ABCDEFGH"
            ),
            QuestionFlowStep.ConfirmAddress),
        new(
            "Confirm address continues to no EPC found if no EPC set",
            new Input(
                QuestionFlowStep.ConfirmAddress,
                "ABCDEFGH",
                epc: null
            ),
            QuestionFlowStep.NoEpcFound),
        new(
            "Confirm address continues to confirm EPC details if EPC added contains property type and age",
            new Input(
                QuestionFlowStep.ConfirmAddress,
                "ABCDEFGH",
                epc: new Epc
                {
                    PropertyType = PropertyType.House,
                    HouseType = HouseType.Detached,
                    ConstructionAgeBand = HomeAge.From1950To1966
                }
            ),
            QuestionFlowStep.ConfirmEpcDetails),
        new(
            "Confirm address continues to property type if EPC exists but property type is missing",
            new Input(
                QuestionFlowStep.ConfirmAddress,
                "ABCDEFGH",
                epc: new Epc
                {
                    ConstructionAgeBand = HomeAge.From1950To1966
                }
            ),
            QuestionFlowStep.PropertyType),
        new(
            "Confirm address continues to property type if EPC exists but age is missing",
            new Input(
                QuestionFlowStep.ConfirmAddress,
                "ABCDEFGH",
                epc: new Epc()
                {
                    PropertyType = PropertyType.House,
                    HouseType = HouseType.Detached
                }
            ),
            QuestionFlowStep.PropertyType),
        new(
            "Confirm EPC details continues to wall construction if epc details confirmed",
            new Input(
                QuestionFlowStep.ConfirmEpcDetails,
                "ABCDEFGH",
                epcDetailsConfirmed: EpcDetailsConfirmed.Yes
            ),
            QuestionFlowStep.WallConstruction),
        new(
            "Confirm EPC details continues to property type if epc details not confirmed",
            new Input(
                QuestionFlowStep.ConfirmEpcDetails,
                "ABCDEFGH",
                epcDetailsConfirmed: EpcDetailsConfirmed.No
            ),
            QuestionFlowStep.PropertyType),
        new(
            "No EPC found continues to property type",
            new Input(
                QuestionFlowStep.NoEpcFound,
                "ABCDEFGH"
            ),
            QuestionFlowStep.PropertyType),
        new(
            "Property type continues to the relevant specific type of property (house)",
            new Input(
                QuestionFlowStep.PropertyType,
                "ABCDEFGH",
                propertyType: PropertyType.House
            ),
            QuestionFlowStep.HouseType),
        new(
            "Property type continues to the relevant specific type of property (bungalow)",
            new Input(
                QuestionFlowStep.PropertyType,
                "ABCDEFGH",
                propertyType: PropertyType.Bungalow
            ),
            QuestionFlowStep.BungalowType),
        new(
            "Property type continues to the relevant specific type of property (flat)",
            new Input(
                QuestionFlowStep.PropertyType,
                "ABCDEFGH",
                propertyType: PropertyType.ApartmentFlatOrMaisonette
            ),
            QuestionFlowStep.FlatType),
        new(
            "House type continues to home age",
            new Input(
                QuestionFlowStep.HouseType,
                "ABCDEFGH"
            ),
            QuestionFlowStep.HomeAge),
        new(
            "Changing house type continues to check your unchangeable answers",
            new Input(
                QuestionFlowStep.HouseType,
                "ABCDEFGH",
                entryPoint: QuestionFlowStep.PropertyType
            ),
            QuestionFlowStep.CheckYourUnchangeableAnswers),
        new(
            "Bungalow type continues to home age",
            new Input(
                QuestionFlowStep.BungalowType,
                "ABCDEFGH"
            ),
            QuestionFlowStep.HomeAge),
        new(
            "Changing bungalow type continues to check your unchangeable answers",
            new Input(
                QuestionFlowStep.BungalowType,
                "ABCDEFGH",
                entryPoint: QuestionFlowStep.PropertyType
            ),
            QuestionFlowStep.CheckYourUnchangeableAnswers),
        new(
            "Flat type continues to home age",
            new Input(
                QuestionFlowStep.FlatType,
                "ABCDEFGH"
            ),
            QuestionFlowStep.HomeAge),
        new(
            "Changing flat type continues to check your unchangeable answers",
            new Input(
                QuestionFlowStep.FlatType,
                "ABCDEFGH",
                entryPoint: QuestionFlowStep.PropertyType
            ),
            QuestionFlowStep.CheckYourUnchangeableAnswers),
        new(
            "Home age continues to check your unchangeable answers",
            new Input(
                QuestionFlowStep.HomeAge,
                "ABCDEFGH"
            ),
            QuestionFlowStep.CheckYourUnchangeableAnswers),
        new(
            "Changing home age continues to check your unchangeable answers",
            new Input(
                QuestionFlowStep.HomeAge,
                "ABCDEFGH",
                entryPoint: QuestionFlowStep.HomeAge
            ),
            QuestionFlowStep.CheckYourUnchangeableAnswers),
        new(
            "Check your unchangeable answers continues to wall consutruction",
            new Input(
                QuestionFlowStep.CheckYourUnchangeableAnswers,
                "ABCDEFGH"
            ),
            QuestionFlowStep.WallConstruction),
        new(
            "Wall construction continues to the respective type of wall insulation",
            new Input(
                QuestionFlowStep.WallConstruction,
                "ABCDEFGH",
                wallConstruction: WallConstruction.Solid
            ),
            QuestionFlowStep.SolidWallsInsulated),
        new(
            "Wall construction continues to floor construction if user's walls are not known and the user can answer floor questions",
            new Input(
                QuestionFlowStep.WallConstruction,
                "ABCDEFGH",
                propertyType: PropertyType.House,
                wallConstruction: WallConstruction.Other
            ),
            QuestionFlowStep.FloorConstruction),
        new(
            "Wall construction continues to roof construction if user's walls are not known, can not answer floor questions but can answer roof ones",
            new Input(
                QuestionFlowStep.WallConstruction,
                "ABCDEFGH",
                propertyType: PropertyType.ApartmentFlatOrMaisonette,
                flatType: FlatType.TopFloor,
                wallConstruction: WallConstruction.Other
            ),
            QuestionFlowStep.RoofConstruction),
        new(
            "Wall construction continues to glazing type if user's walls are not known and can not answer neither floor questions nor roof ones",
            new Input(
                QuestionFlowStep.WallConstruction,
                "ABCDEFGH",
                propertyType: PropertyType.ApartmentFlatOrMaisonette,
                flatType: FlatType.MiddleFloor,
                wallConstruction: WallConstruction.Other
            ),
            QuestionFlowStep.GlazingType),
        new(
            "Changing wall construction continues to summary if walls are not known",
            new Input(
                QuestionFlowStep.WallConstruction,
                "ABCDEFGH",
                wallConstruction: WallConstruction.Other,
                entryPoint: QuestionFlowStep.WallConstruction
            ),
            QuestionFlowStep.AnswerSummary),
        new(
            "Changing cavity walls insulated continues to summary",
            new Input(
                QuestionFlowStep.CavityWallsInsulated,
                "ABCDEFGH",
                entryPoint: QuestionFlowStep.CavityWallsInsulated
            ),
            QuestionFlowStep.AnswerSummary),
        new(
            "Cavity walls insulated continue to solid walls insulated if user has mixed walls",
            new Input(
                QuestionFlowStep.CavityWallsInsulated,
                "ABCDEFGH",
                wallConstruction: WallConstruction.Mixed
            ),
            QuestionFlowStep.SolidWallsInsulated),
        new(
            "Cavity walls insulated continue to floor construction if user only does not have solid walls and can answer floor questions",
            new Input(
                QuestionFlowStep.CavityWallsInsulated,
                "ABCDEFGH",
                propertyType: PropertyType.House,
                wallConstruction: WallConstruction.Cavity
            ),
            QuestionFlowStep.FloorConstruction),
        new(
            "Cavity walls insulated continue to roof construction if user only does not have solid walls, can not answer floor questions but can answer roof questions",
            new Input(
                QuestionFlowStep.CavityWallsInsulated,
                "ABCDEFGH",
                propertyType: PropertyType.ApartmentFlatOrMaisonette,
                flatType: FlatType.TopFloor,
                wallConstruction: WallConstruction.Cavity
            ),
            QuestionFlowStep.RoofConstruction),
        new(
            "Cavity walls insulated continue to glazing type if user only does not have solid walls and can not answer neither floor questions nor roof questions",
            new Input(
                QuestionFlowStep.CavityWallsInsulated,
                "ABCDEFGH",
                propertyType: PropertyType.ApartmentFlatOrMaisonette,
                flatType: FlatType.MiddleFloor,
                wallConstruction: WallConstruction.Cavity
            ),
            QuestionFlowStep.GlazingType),
        new(
            "Changing solid walls insulated continues to summary",
            new Input(
                QuestionFlowStep.SolidWallsInsulated,
                "ABCDEFGH",
                entryPoint: QuestionFlowStep.SolidWallsInsulated
            ),
            QuestionFlowStep.AnswerSummary),
        new(
            "Solid walls insulated continue to floor construction if user can answer floor questions",
            new Input(
                QuestionFlowStep.SolidWallsInsulated,
                "ABCDEFGH",
                propertyType: PropertyType.House
            ),
            QuestionFlowStep.FloorConstruction),
        new(
            "Solid walls insulated continue to roof construction if user can not answer floor questions but can answer roof ones",
            new Input(
                QuestionFlowStep.SolidWallsInsulated,
                "ABCDEFGH",
                propertyType: PropertyType.ApartmentFlatOrMaisonette,
                flatType: FlatType.TopFloor
            ),
            QuestionFlowStep.RoofConstruction),
        new(
            "Solid walls insulated continue to glazing type if user can not answer neither floor questions nor roof ones",
            new Input(
                QuestionFlowStep.SolidWallsInsulated,
                "ABCDEFGH",
                propertyType: PropertyType.ApartmentFlatOrMaisonette,
                flatType: FlatType.MiddleFloor
            ),
            QuestionFlowStep.GlazingType),
        new(
            "Floor construction continues to floor insulated if the floors are known",
            new Input(
                QuestionFlowStep.FloorConstruction,
                "ABCDEFGH",
                floorConstruction: FloorConstruction.Mix
            ),
            QuestionFlowStep.FloorInsulated),
        new(
            "Changing floor construction continues to summary if floor are not known",
            new Input(
                QuestionFlowStep.FloorConstruction,
                "ABCDEFGH",
                floorConstruction: FloorConstruction.Other,
                entryPoint: QuestionFlowStep.FloorConstruction
            ),
            QuestionFlowStep.AnswerSummary),
        new(
            "Floor construction continues to roof construction if floors are unknown and the user can answer roof questions",
            new Input(
                QuestionFlowStep.FloorConstruction,
                "ABCDEFGH",
                propertyType: PropertyType.House,
                floorConstruction: FloorConstruction.Other
            ),
            QuestionFlowStep.RoofConstruction),
        new(
            "Floor construction continues to glazing type if floor are unknown and the user can not answer roof questions",
            new Input(
                QuestionFlowStep.FloorConstruction,
                "ABCDEFGH",
                propertyType: PropertyType.ApartmentFlatOrMaisonette,
                flatType: FlatType.MiddleFloor,
                floorConstruction: FloorConstruction.Other
            ),
            QuestionFlowStep.GlazingType),
        new(
            "Changing floor insulated continues to summary",
            new Input(
                QuestionFlowStep.FloorInsulated,
                "ABCDEFGH",
                entryPoint: QuestionFlowStep.FloorInsulated
            ),
            QuestionFlowStep.AnswerSummary),
        new(
            "Floor insulated continues to roof construction if user can answer roof questions",
            new Input(
                QuestionFlowStep.FloorInsulated,
                "ABCDEFGH",
                propertyType: PropertyType.House
            ),
            QuestionFlowStep.RoofConstruction),
        new(
            "Floor insulated continues to glazing type if user can not answer roof questions",
            new Input(
                QuestionFlowStep.FloorInsulated,
                "ABCDEFGH",
                propertyType: PropertyType.ApartmentFlatOrMaisonette,
                flatType: FlatType.MiddleFloor
            ),
            QuestionFlowStep.GlazingType),
        new(
            "Roof construction continues to loft space if roof is pitched",
            new Input(
                QuestionFlowStep.RoofConstruction,
                "ABCDEFGH",
                roofConstruction: RoofConstruction.Pitched
            ),
            QuestionFlowStep.LoftSpace),
        new(
            "Roof construction continues to loft space if roof is mixed",
            new Input(
                QuestionFlowStep.RoofConstruction,
                "ABCDEFGH",
                roofConstruction: RoofConstruction.Mixed
            ),
            QuestionFlowStep.LoftSpace),
        new(
            "Changing roof construction continues to summary if roof is not pitched",
            new Input(
                QuestionFlowStep.RoofConstruction,
                "ABCDEFGH",
                roofConstruction: RoofConstruction.Flat,
                entryPoint: QuestionFlowStep.RoofConstruction
            ),
            QuestionFlowStep.AnswerSummary),
        new(
            "Roof construction continues to glazing type space if roof is not pitched",
            new Input(
                QuestionFlowStep.RoofConstruction,
                "ABCDEFGH",
                roofConstruction: RoofConstruction.Flat
            ),
            QuestionFlowStep.GlazingType),
        new(
            "Loft space continues to loft access if user has it",
            new Input(
                QuestionFlowStep.LoftSpace,
                "ABCDEFGH",
                loftSpace: LoftSpace.Yes
            ),
            QuestionFlowStep.LoftAccess),
        new(
            "Changing loft space continues to answer summary if user does not have loft space",
            new Input(
                QuestionFlowStep.LoftSpace,
                "ABCDEFGH",
                loftSpace: LoftSpace.No,
                entryPoint: QuestionFlowStep.LoftSpace
            ),
            QuestionFlowStep.AnswerSummary),
        new(
            "Loft space continues to glazing type if user does not have it",
            new Input(
                QuestionFlowStep.LoftSpace,
                "ABCDEFGH",
                loftSpace: LoftSpace.No
            ),
            QuestionFlowStep.GlazingType),
        new(
            "Loft access continues to roof insulation if user has it",
            new Input(
                QuestionFlowStep.LoftAccess,
                "ABCDEFGH",
                accessibleLoft: LoftAccess.Yes
            ),
            QuestionFlowStep.RoofInsulated),
        new(
            "Changing loft access continues to roof insulation if user has it",
            new Input(
                QuestionFlowStep.LoftAccess,
                "ABCDEFGH",
                accessibleLoft: LoftAccess.Yes,
                entryPoint: QuestionFlowStep.LoftAccess
            ),
            QuestionFlowStep.RoofInsulated),
        new(
            "Changing loft access continues to answer summary if user does not have it",
            new Input(
                QuestionFlowStep.LoftAccess,
                "ABCDEFGH",
                accessibleLoft: LoftAccess.No,
                entryPoint: QuestionFlowStep.LoftAccess
            ),
            QuestionFlowStep.AnswerSummary),
        new(
            "Loft access continues to glazing type if user does not have it",
            new Input(
                QuestionFlowStep.LoftAccess,
                "ABCDEFGH",
                loftSpace: LoftSpace.No
            ),
            QuestionFlowStep.GlazingType),
        new(
            "Roof insulated continues to glazing type",
            new Input(
                QuestionFlowStep.RoofInsulated,
                "ABCDEFGH"
            ),
            QuestionFlowStep.GlazingType),
        new(
            "Changing roof insulated continues to summary",
            new Input(
                QuestionFlowStep.RoofInsulated,
                "ABCDEFGH",
                entryPoint: QuestionFlowStep.RoofInsulated
            ),
            QuestionFlowStep.AnswerSummary),
        new(
            "Glazing type continues to outdoor space",
            new Input(
                QuestionFlowStep.GlazingType,
                "ABCDEFGH"
            ),
            QuestionFlowStep.OutdoorSpace),
        new(
            "Changing glazing type continues to summary",
            new Input(
                QuestionFlowStep.GlazingType,
                "ABCDEFGH",
                entryPoint: QuestionFlowStep.GlazingType
            ),
            QuestionFlowStep.AnswerSummary),
        new(
            "Outdoor space continues to heating type",
            new Input(
                QuestionFlowStep.OutdoorSpace,
                "ABCDEFGH"
            ),
            QuestionFlowStep.HeatingType),
        new(
            "Changing outdoor space continues to summary",
            new Input(
                QuestionFlowStep.OutdoorSpace,
                "ABCDEFGH",
                entryPoint: QuestionFlowStep.OutdoorSpace
            ),
            QuestionFlowStep.AnswerSummary),
        new(
            "Heating type continues to other heating type in that scenario",
            new Input(
                QuestionFlowStep.HeatingType,
                "ABCDEFGH",
                heatingType: HeatingType.Other
            ),
            QuestionFlowStep.OtherHeatingType),
        new(
            "Heating type continues to hot water cylinder if user has a boiler",
            new Input(
                QuestionFlowStep.HeatingType,
                "ABCDEFGH",
                heatingType: HeatingType.GasBoiler
            ),
            QuestionFlowStep.HotWaterCylinder),
        new(
            "Changing heating type continues to summary if user doesn't have a boiler or other heating type",
            new Input(
                QuestionFlowStep.HeatingType,
                "ABCDEFGH",
                heatingType: HeatingType.HeatPump,
                entryPoint: QuestionFlowStep.HeatingType
            ),
            QuestionFlowStep.AnswerSummary),
        new(
            "Heating type continues to number of occupants if user doesn't have a boiler or other heating",
            new Input(
                QuestionFlowStep.HeatingType,
                "ABCDEFGH",
                heatingType: HeatingType.HeatPump
            ),
            QuestionFlowStep.NumberOfOccupants),
        new(
            "Other heating type continues to number of occupants",
            new Input(
                QuestionFlowStep.OtherHeatingType,
                "ABCDEFGH"
            ),
            QuestionFlowStep.NumberOfOccupants),
        new(
            "Changing other heating type continues to summary",
            new Input(
                QuestionFlowStep.OtherHeatingType,
                "ABCDEFGH",
                entryPoint: QuestionFlowStep.OtherHeatingType
            ),
            QuestionFlowStep.AnswerSummary),
        new(
            "Hot water cylinder continues to number of occupants",
            new Input(
                QuestionFlowStep.HotWaterCylinder,
                "ABCDEFGH"
            ),
            QuestionFlowStep.NumberOfOccupants),
        new(
            "Changing hot water cylinder continues to summary",
            new Input(
                QuestionFlowStep.HotWaterCylinder,
                "ABCDEFGH",
                entryPoint: QuestionFlowStep.HotWaterCylinder
            ),
            QuestionFlowStep.AnswerSummary),
        new(
            "Number of occupants continues to heating pattern",
            new Input(
                QuestionFlowStep.NumberOfOccupants,
                "ABCDEFGH"
            ),
            QuestionFlowStep.HeatingPattern),
        new(
            "Changing number of occupants continues to summary",
            new Input(
                QuestionFlowStep.NumberOfOccupants,
                "ABCDEFGH",
                entryPoint: QuestionFlowStep.NumberOfOccupants
            ),
            QuestionFlowStep.AnswerSummary),
        new(
            "Heating pattern continues to temperature",
            new Input(
                QuestionFlowStep.HeatingPattern,
                "ABCDEFGH"
            ),
            QuestionFlowStep.Temperature),
        new(
            "Changing heating pattern continues to summary",
            new Input(
                QuestionFlowStep.HeatingPattern,
                "ABCDEFGH",
                entryPoint: QuestionFlowStep.HeatingPattern
            ),
            QuestionFlowStep.AnswerSummary),
        new(
            "Temperature continues to summary",
            new Input(
                QuestionFlowStep.Temperature,
                "ABCDEFGH"
            ),
            QuestionFlowStep.AnswerSummary),
        new(
            "Answer summary continues to your recommendations if user has at least one",
            new Input(
                QuestionFlowStep.AnswerSummary,
                "ABCDEFGH",
                propertyRecommendations: new List<PropertyRecommendation> {new()}
            ),
            QuestionFlowStep.YourRecommendations),
        new(
            "Answer summary continues to no recommendations if user has none",
            new Input(
                QuestionFlowStep.AnswerSummary,
                "ABCDEFGH",
                propertyRecommendations: new List<PropertyRecommendation>()),
            QuestionFlowStep.NoRecommendations)
    };

    private static QuestionFlowServiceTestCase[] SkipTestCases =
    {
        new(
            "Postcode skips to property type",
            new Input(
                QuestionFlowStep.AskForPostcode,
                "ABCDEFGH"
            ),
            QuestionFlowStep.PropertyType)
    };

    public class QuestionFlowServiceTestCase
    {
        public readonly string Description;
        public readonly QuestionFlowStep ExpectedOutput;
        public readonly Input Input;

        public QuestionFlowServiceTestCase(
            string description,
            Input input,
            QuestionFlowStep expectedOutput
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
        public readonly QuestionFlowStep Page;
        public readonly PropertyData PropertyData;
        public QuestionFlowStep? EntryPoint;

        public Input(
            QuestionFlowStep page,
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
            QuestionFlowStep? entryPoint = null,
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