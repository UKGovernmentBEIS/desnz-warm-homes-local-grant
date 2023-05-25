using FluentAssertions;
using NUnit.Framework;
using HerPublicWebsite.BusinessLogic.Models;
using HerPublicWebsite.BusinessLogic.Models.Enums;
using HerPublicWebsite.BusinessLogic.Services;
using HerPublicWebsite.BusinessLogic.Services.QuestionFlow;

namespace Tests.BusinessLogic.Services;

[TestFixture]
public class QuestionFlowServiceTests
{
    [SetUp]
    public void Setup()
    {
        questionFlowService = new QuestionFlowService();
    }

    private IQuestionFlowService questionFlowService;

    [TestCaseSource(nameof(BackTestCases))]
    public void RunBackLinkTestCases(QuestionFlowServiceTestCase testCase)
    {
        // Act
        var output = questionFlowService.PreviousStep(
            testCase.Input.Page,
            testCase.Input.Questionnaire,
            testCase.Input.EntryPoint);

        // Assert
        output.Should().Be(testCase.ExpectedOutput);
    }

    [TestCaseSource(nameof(ForwardTestCases))]
    public void RunForwardLinkTestCases(QuestionFlowServiceTestCase testCase)
    {
        // Act
        var output = questionFlowService.NextStep(
            testCase.Input.Page,
            testCase.Input.Questionnaire,
            testCase.Input.EntryPoint);

        // Assert
        output.Should().Be(testCase.ExpectedOutput);
    }

    private static QuestionFlowServiceTestCase[] BackTestCases =
    {
        new(
            "Gas boiler goes back to start",
            new Input(
                QuestionFlowStep.GasBoiler
            ),
            QuestionFlowStep.Start),
        new(
            "Direct to ECO goes back to gas boiler",
            new Input(
                QuestionFlowStep.DirectToEco
            ),
            QuestionFlowStep.GasBoiler),
        new(
            "Country goes back to gas boiler",
            new Input(
                QuestionFlowStep.Country
            ),
            QuestionFlowStep.GasBoiler),
        new(
            "Ownership status goes back to Country",
            new Input(
                QuestionFlowStep.OwnershipStatus
            ),
            QuestionFlowStep.Country),
        new(
            "Address goes back to Ownership status",
            new Input(
                QuestionFlowStep.Address
            ),
            QuestionFlowStep.OwnershipStatus),
        new(
            "Select address goes back to Address",
            new Input(
                QuestionFlowStep.SelectAddress
            ),
            QuestionFlowStep.Address),
        new(
            "Review EPC goes back to Address",
            new Input(
                QuestionFlowStep.ReviewEpc
            ),
            QuestionFlowStep.Address),
        new(
            "Manual address goes back to Address",
            new Input(
                QuestionFlowStep.ManualAddress
            ),
            QuestionFlowStep.Address),
        new(
            "Select local authority goes back to Manual address",
            new Input(
                QuestionFlowStep.SelectLocalAuthority
            ),
            QuestionFlowStep.ManualAddress),
        new(
            "Confirm local authority goes back to select local authority",
            new Input(
                QuestionFlowStep.ConfirmLocalAuthority
            ),
            QuestionFlowStep.SelectLocalAuthority),
        new(
            "Household income goes back to Address if UPRN found",
            new Input(
                QuestionFlowStep.HouseholdIncome, uprn: "100023336956"
            ),
            QuestionFlowStep.Address),
        new(
            "Household income goes back to confirm local authority if no UPRN found",
            new Input(
                QuestionFlowStep.HouseholdIncome, uprn: null
            ),
            QuestionFlowStep.ConfirmLocalAuthority),
        new(
            "Household income goes back to review EPC if EPC is too high",
            new Input(
                QuestionFlowStep.HouseholdIncome, epcRating: EpcRating.B
            ),
            QuestionFlowStep.ReviewEpc),
        new(
            "Service unsuitable goes back to the country you came from",
            new Input(
                QuestionFlowStep.ServiceUnsuitable,
                country: Country.Other
            ),
            QuestionFlowStep.Country),
        new(
            "Service unsuitable goes back to ownership status isn't owner occupier",
            new Input(
                QuestionFlowStep.ServiceUnsuitable,
                country: Country.England,
                ownershipStatus: OwnershipStatus.Landlord
            ),
            QuestionFlowStep.OwnershipStatus),
        new(
            "Service unsuitable goes back to review EPC if EPC is filled in",
            new Input(
                QuestionFlowStep.ServiceUnsuitable,
                country: Country.England,
                ownershipStatus: OwnershipStatus.OwnerOccupancy,
                epcDetailsAreCorrect: true
            ),
            QuestionFlowStep.ReviewEpc),
        new(
            "Service unsuitable goes back to boiler question if the service is actually suitable",
            new Input(
                QuestionFlowStep.ServiceUnsuitable,
                country: Country.England,
                ownershipStatus: OwnershipStatus.OwnerOccupancy,
                epcDetailsAreCorrect: false
            ),
            QuestionFlowStep.GasBoiler),
        new(
            "Gas boiler goes back to check answers if was changing answer",
            new Input(
                QuestionFlowStep.GasBoiler,
                entryPoint: QuestionFlowStep.GasBoiler
            ),
            QuestionFlowStep.CheckAnswers),
        new(
            "Direct to ECO goes back to gas boiler if was changing answer",
            new Input(
                QuestionFlowStep.DirectToEco,
                entryPoint: QuestionFlowStep.GasBoiler
            ),
            QuestionFlowStep.GasBoiler),
        new(
            "Country goes back to check answers if was changing answer",
            new Input(
                QuestionFlowStep.Country,
                entryPoint: QuestionFlowStep.Country
            ),
            QuestionFlowStep.CheckAnswers),
        new(
            "Ownership status goes back to check answers if was changing answer",
            new Input(
                QuestionFlowStep.OwnershipStatus,
                entryPoint: QuestionFlowStep.OwnershipStatus
            ),
            QuestionFlowStep.CheckAnswers),
        new(
            "Address goes back to check answers if was changing answer",
            new Input(
                QuestionFlowStep.Address,
                entryPoint: QuestionFlowStep.Address
            ),
            QuestionFlowStep.CheckAnswers),
        new(
            "Select address goes back to Address if was changing answer",
            new Input(
                QuestionFlowStep.SelectAddress,
                entryPoint: QuestionFlowStep.Address
            ),
            QuestionFlowStep.Address),
        new(
            "Review EPC goes back to Address if was changing answer",
            new Input(
                QuestionFlowStep.ReviewEpc,
                entryPoint: QuestionFlowStep.Address
            ),
            QuestionFlowStep.Address),
        new(
            "Manual address goes back to Address if was changing answer",
            new Input(
                QuestionFlowStep.ManualAddress,
                entryPoint: QuestionFlowStep.Address
            ),
            QuestionFlowStep.Address),
        new(
            "Household income goes back to check answers if was changing answer",
            new Input(
                QuestionFlowStep.HouseholdIncome,
                entryPoint: QuestionFlowStep.HouseholdIncome
            ),
            QuestionFlowStep.CheckAnswers),
        new(
            "Service unsuitable goes back to the country you came from if was changing answer",
            new Input(
                QuestionFlowStep.ServiceUnsuitable,
                country: Country.Other,
                entryPoint: QuestionFlowStep.Country
            ),
            QuestionFlowStep.Country),
        new(
            "Service unsuitable goes back to ownership status isn't owner occupier if was changing answer",
            new Input(
                QuestionFlowStep.ServiceUnsuitable,
                country: Country.England,
                ownershipStatus: OwnershipStatus.Landlord,
                entryPoint: QuestionFlowStep.OwnershipStatus
            ),
            QuestionFlowStep.OwnershipStatus),
        new(
            "Service unsuitable goes back to review EPC if EPC is filled in if was changing answer",
            new Input(
                QuestionFlowStep.ServiceUnsuitable,
                country: Country.England,
                ownershipStatus: OwnershipStatus.OwnerOccupancy,
                epcDetailsAreCorrect: true,
                entryPoint: QuestionFlowStep.Address
            ),
            QuestionFlowStep.ReviewEpc),
        new(
            "Service unsuitable goes back to boiler question if the service is actually suitable and was changing answer",
            new Input(
                QuestionFlowStep.ServiceUnsuitable,
                country: Country.England,
                ownershipStatus: OwnershipStatus.OwnerOccupancy,
                epcDetailsAreCorrect: false,
                entryPoint: QuestionFlowStep.GasBoiler
            ),
            QuestionFlowStep.GasBoiler),
    };

    private static QuestionFlowServiceTestCase[] ForwardTestCases =
    {
        new(
            "Gas boiler continues to direct to ECO if the user has a boiler",
            new Input(
                QuestionFlowStep.GasBoiler,
                hasGasBoiler: HasGasBoiler.Yes
            ),
            QuestionFlowStep.DirectToEco),
        new(
            "Gas boiler continues to country if the user doesn't have a boiler",
            new Input(
                QuestionFlowStep.GasBoiler,
                hasGasBoiler: HasGasBoiler.No
            ),
            QuestionFlowStep.Country),
        new(
            "Gas boiler continues to country if the user doesn't know about their boiler",
            new Input(
                QuestionFlowStep.GasBoiler,
                hasGasBoiler: HasGasBoiler.Unknown
            ),
            QuestionFlowStep.Country),
        new(
            "Country continues to service unsuitable if the country is Wales",
            new Input(
                QuestionFlowStep.Country,
                country: Country.Wales
            ),
            QuestionFlowStep.ServiceUnsuitable),
        new(
            "Country continues to service unsuitable if the country is Scotland",
            new Input(
                QuestionFlowStep.Country,
                country: Country.Scotland
            ),
            QuestionFlowStep.ServiceUnsuitable),
        new(
            "Country continues to service unsuitable if the country is Northern Ireland",
            new Input(
                QuestionFlowStep.Country,
                country: Country.NorthernIreland
            ),
            QuestionFlowStep.ServiceUnsuitable),
        new(
            "Country continues to service unsuitable if the country is Other",
            new Input(
                QuestionFlowStep.Country,
                country: Country.Other
            ),
            QuestionFlowStep.ServiceUnsuitable),
        new(
            "Country continues to ownership status",
            new Input(
                QuestionFlowStep.Country,
                country: Country.England
            ),
            QuestionFlowStep.OwnershipStatus),
        new(
            "Ownership status continues to service unsuitable if user is private tenant",
            new Input(
                QuestionFlowStep.OwnershipStatus,
                ownershipStatus: OwnershipStatus.PrivateTenancy
            ),
            QuestionFlowStep.ServiceUnsuitable),
        new(
            "Ownership status continues to service unsuitable if user is landlord",
            new Input(
                QuestionFlowStep.OwnershipStatus,
                ownershipStatus: OwnershipStatus.Landlord
            ),
            QuestionFlowStep.ServiceUnsuitable),
        new(
            "Ownership status continues to address if user is owner occupier",
            new Input(
                QuestionFlowStep.OwnershipStatus,
                ownershipStatus: OwnershipStatus.OwnerOccupancy
            ),
            QuestionFlowStep.Address),
        new(
            "Address continues to address selection",
            new Input(
                QuestionFlowStep.Address
            ),
            QuestionFlowStep.SelectAddress),
        new(
            "Address selection continues to household income if EPC is low",
            new Input(
                QuestionFlowStep.SelectAddress,
                epcRating: EpcRating.D
            ),
            QuestionFlowStep.HouseholdIncome),
        new(
            "Address selection continues to review EPC if EPC is high",
            new Input(
                QuestionFlowStep.SelectAddress,
                epcRating: EpcRating.C
            ),
            QuestionFlowStep.ReviewEpc),
        new(
            "Review EPC continues to household income if EPC is incorrect",
            new Input(
                QuestionFlowStep.ReviewEpc,
                epcDetailsAreCorrect: false
            ),
            QuestionFlowStep.HouseholdIncome),
        new(
            "Review EPC continues to service unsuitable if EPC is correct",
            new Input(
                QuestionFlowStep.ReviewEpc,
                epcDetailsAreCorrect: true
            ),
            QuestionFlowStep.ServiceUnsuitable),
        new(
            "Manual address continues to select local authority",
            new Input(
                QuestionFlowStep.ManualAddress
            ),
            QuestionFlowStep.SelectLocalAuthority),
        new(
            "Select local authority continues to confirm local authority",
            new Input(
                QuestionFlowStep.SelectLocalAuthority
            ),
            QuestionFlowStep.ConfirmLocalAuthority),
        new(
            "Confirm local authority continues to select local authority if authority is incorrect",
            new Input(
                QuestionFlowStep.ConfirmLocalAuthority,
                localAuthorityIsCorrect: false
            ),
            QuestionFlowStep.SelectLocalAuthority),
        new(
            "Confirm local authority continues to household income if authority is correct",
            new Input(
                QuestionFlowStep.ConfirmLocalAuthority,
                localAuthorityIsCorrect: true
            ),
            QuestionFlowStep.HouseholdIncome),
        new(
            "Household income continues to check answers",
            new Input(
                QuestionFlowStep.HouseholdIncome
            ),
            QuestionFlowStep.CheckAnswers),
        new(
            "Gas boiler continues to direct to ECO if the user has a boiler and was changing answer",
            new Input(
                QuestionFlowStep.GasBoiler,
                hasGasBoiler: HasGasBoiler.Yes,
                entryPoint: QuestionFlowStep.GasBoiler
            ),
            QuestionFlowStep.DirectToEco),
        new(
            "Gas boiler returns to check answers if user doesn't have a boiler, and was changing answer",
            new Input(
                QuestionFlowStep.GasBoiler,
                hasGasBoiler: HasGasBoiler.No,
                entryPoint: QuestionFlowStep.GasBoiler
            ),
            QuestionFlowStep.CheckAnswers),
        new(
            "Gas boiler returns to check answers if user doesn't know about their boiler, and was changing answer",
            new Input(
                QuestionFlowStep.GasBoiler,
                hasGasBoiler: HasGasBoiler.Unknown,
                entryPoint: QuestionFlowStep.GasBoiler
            ),
            QuestionFlowStep.CheckAnswers),
        new(
            "Country continues to service unsuitable if the country is Wales and was changing answer",
            new Input(
                QuestionFlowStep.Country,
                country: Country.Wales,
                entryPoint: QuestionFlowStep.Country
            ),
            QuestionFlowStep.ServiceUnsuitable),
        new(
            "Country continues to service unsuitable if the country is Scotland and was changing answer",
            new Input(
                QuestionFlowStep.Country,
                country: Country.Scotland,
                entryPoint: QuestionFlowStep.Country
            ),
            QuestionFlowStep.ServiceUnsuitable),
        new(
            "Country continues to service unsuitable if the country is Northern Ireland and was changing answer",
            new Input(
                QuestionFlowStep.Country,
                country: Country.NorthernIreland,
                entryPoint: QuestionFlowStep.Country
            ),
            QuestionFlowStep.ServiceUnsuitable),
        new(
            "Country continues to service unsuitable if the country is Other and was changing answer",
            new Input(
                QuestionFlowStep.Country,
                country: Country.Other,
                entryPoint: QuestionFlowStep.Country
            ),
            QuestionFlowStep.ServiceUnsuitable),
        new(
            "Country returns to check answers if the country is England was changing answer",
            new Input(
                QuestionFlowStep.Country,
                country: Country.England,
                entryPoint: QuestionFlowStep.Country
            ),
            QuestionFlowStep.CheckAnswers),
        new(
            "Ownership status continues to service unsuitable if user is private tenant and was changing answer",
            new Input(
                QuestionFlowStep.OwnershipStatus,
                ownershipStatus: OwnershipStatus.PrivateTenancy,
                entryPoint: QuestionFlowStep.OwnershipStatus
            ),
            QuestionFlowStep.ServiceUnsuitable),
        new(
            "Ownership status continues to service unsuitable if user is landlord and was changing answer",
            new Input(
                QuestionFlowStep.OwnershipStatus,
                ownershipStatus: OwnershipStatus.Landlord,
                entryPoint: QuestionFlowStep.OwnershipStatus
            ),
            QuestionFlowStep.ServiceUnsuitable),
        new(
            "Ownership status returns to check answers if user is owner occupier and was changing answer",
            new Input(
                QuestionFlowStep.OwnershipStatus,
                ownershipStatus: OwnershipStatus.OwnerOccupancy,
                entryPoint: QuestionFlowStep.OwnershipStatus
            ),
            QuestionFlowStep.CheckAnswers),
        new(
            "Address continues to address selection if was changing answer",
            new Input(
                QuestionFlowStep.Address,
                entryPoint: QuestionFlowStep.Address
            ),
            QuestionFlowStep.SelectAddress),
        new(
            "Address selection returns to check answers income if EPC is low and was changing answer",
            new Input(
                QuestionFlowStep.SelectAddress,
                epcRating: EpcRating.D,
                entryPoint: QuestionFlowStep.Address
            ),
            QuestionFlowStep.CheckAnswers),
        new(
            "Address selection continues to review EPC if EPC is high and was changing answer",
            new Input(
                QuestionFlowStep.SelectAddress,
                epcRating: EpcRating.C,
                entryPoint: QuestionFlowStep.Address
            ),
            QuestionFlowStep.ReviewEpc),
        new(
            "Review EPC returns to check answers if EPC is incorrect and was changing answer",
            new Input(
                QuestionFlowStep.ReviewEpc,
                epcDetailsAreCorrect: false,
                entryPoint: QuestionFlowStep.Address
            ),
            QuestionFlowStep.CheckAnswers),
        new(
            "Review EPC continues to service unsuitable if EPC is correct and was changing answer",
            new Input(
                QuestionFlowStep.ReviewEpc,
                epcDetailsAreCorrect: true,
                entryPoint: QuestionFlowStep.Address
            ),
            QuestionFlowStep.ServiceUnsuitable),
        new(
            "Manual address returns to check answers if was changing answer",
            new Input(
                QuestionFlowStep.ManualAddress,
                entryPoint: QuestionFlowStep.Address
            ),
            QuestionFlowStep.CheckAnswers),
        new(
            "Household income continues to check answers if was changing answer",
            new Input(
                QuestionFlowStep.HouseholdIncome,
                entryPoint: QuestionFlowStep.HouseholdIncome
            ),
            QuestionFlowStep.CheckAnswers),
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
        public readonly Questionnaire Questionnaire;
        public QuestionFlowStep? EntryPoint;

        public Input(
            QuestionFlowStep page,
            HasGasBoiler? hasGasBoiler = null,
            OwnershipStatus? ownershipStatus = null,
            Country? country = null,
            string uprn = null,
            bool epcDetailsAreCorrect = false,
            EpcRating? epcRating = null,
            bool localAuthorityIsCorrect = false,
            QuestionFlowStep? entryPoint = null)
        {
            Page = page;
            Questionnaire = new Questionnaire
            {
                HasGasBoiler = hasGasBoiler,
                Uprn = uprn,
                OwnershipStatus = ownershipStatus,
                Country = country,
                EpcDetailsAreCorrect = epcDetailsAreCorrect,
                EpcDetails = epcRating == null ? null : new EpcDetails { EpcRating = epcRating },
                LocalAuthorityConfirmed = localAuthorityIsCorrect,
            };
            EntryPoint = entryPoint;
        }
    }
}
