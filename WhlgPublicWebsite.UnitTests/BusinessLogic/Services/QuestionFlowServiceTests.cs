using System;
using FluentAssertions;
using WhlgPublicWebsite.BusinessLogic.Models;
using WhlgPublicWebsite.BusinessLogic.Models.Enums;
using WhlgPublicWebsite.BusinessLogic.Services.QuestionFlow;
using NUnit.Framework;

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
            "Ineligible Wales goes back to country",
            new Input(
                QuestionFlowStep.IneligibleWales
            ),
            QuestionFlowStep.Country),
        new(
            "Ineligible Scotland goes back to country",
            new Input(
                QuestionFlowStep.IneligibleScotland
            ),
            QuestionFlowStep.Country),
        new(
            "Ineligible Northern Ireland goes back to country",
            new Input(
                QuestionFlowStep.IneligibleNorthernIreland
            ),
            QuestionFlowStep.Country),
        new(
            "Ownership status goes back to Country",
            new Input(
                QuestionFlowStep.OwnershipStatus
            ),
            QuestionFlowStep.Country),
        new(
            "Ineligible tenure goes back to ownership status",
            new Input(
                QuestionFlowStep.IneligibleTenure
            ),
            QuestionFlowStep.OwnershipStatus),
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
            "Not taking part goes back to Address if UPRN found",
            new Input(
                QuestionFlowStep.NotTakingPart, uprn: "100023336956"
            ),
            QuestionFlowStep.Address),
        new(
            "Not taking part goes back to confirm local authority if no UPRN found",
            new Input(
                QuestionFlowStep.NotTakingPart, uprn: null
            ),
            QuestionFlowStep.ConfirmLocalAuthority),
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
            "Household income goes back to address if EPC is too high, but expired",
            new Input(
                QuestionFlowStep.HouseholdIncome, uprn: "100023336956", epcRating: EpcRating.B,
                epcExpiry: DateTime.MinValue
            ),
            QuestionFlowStep.Address),
        new(
            "Check answers goes back to household income",
            new Input(
                QuestionFlowStep.CheckAnswers
            ),
            QuestionFlowStep.HouseholdIncome),
        new(
            "Eligible goes back to check answers",
            new Input(
                QuestionFlowStep.Eligible
            ),
            QuestionFlowStep.CheckAnswers),
        new(
            "Confirmation goes back to eligible",
            new Input(
                QuestionFlowStep.Confirmation
            ),
            QuestionFlowStep.Eligible),
        new(
            "Ineligible goes back to check answers",
            new Input(
                QuestionFlowStep.Ineligible
            ),
            QuestionFlowStep.CheckAnswers),
        new(
            "No consent goes back to eligible",
            new Input(
                QuestionFlowStep.NoConsent
            ),
            QuestionFlowStep.Eligible),
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
            "Ineligible Wales goes back to country if was changing answer",
            new Input(
                QuestionFlowStep.IneligibleWales,
                entryPoint: QuestionFlowStep.Country
            ),
            QuestionFlowStep.Country),
        new(
            "Ineligible Scotland goes back to country if was changing answer",
            new Input(
                QuestionFlowStep.IneligibleScotland,
                entryPoint: QuestionFlowStep.Country
            ),
            QuestionFlowStep.Country),
        new(
            "Ineligible Northern Ireland goes back to country if was changing answer",
            new Input(
                QuestionFlowStep.IneligibleNorthernIreland,
                entryPoint: QuestionFlowStep.Country
            ),
            QuestionFlowStep.Country),
        new(
            "Ownership status goes back to check answers if was changing answer",
            new Input(
                QuestionFlowStep.OwnershipStatus,
                entryPoint: QuestionFlowStep.OwnershipStatus
            ),
            QuestionFlowStep.CheckAnswers),
        new(
            "Ineligible tenure goes back to ownership status if was changing answer",
            new Input(
                QuestionFlowStep.IneligibleTenure,
                entryPoint: QuestionFlowStep.OwnershipStatus
            ),
            QuestionFlowStep.OwnershipStatus),
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
            "Manual address goes back to Address if was changing address answer",
            new Input(
                QuestionFlowStep.ManualAddress,
                entryPoint: QuestionFlowStep.Address
            ),
            QuestionFlowStep.Address),
        new(
            "Select local authority goes back to Manual address if was changing answer",
            new Input(
                QuestionFlowStep.SelectLocalAuthority,
                entryPoint: QuestionFlowStep.Address
            ),
            QuestionFlowStep.ManualAddress),
        new(
            "Confirm local authority goes back to select local authority if was changing answer",
            new Input(
                QuestionFlowStep.ConfirmLocalAuthority,
                entryPoint: QuestionFlowStep.Address
            ),
            QuestionFlowStep.SelectLocalAuthority),
        new(
            "Not taking part goes back to Address if UPRN found if was changing answer",
            new Input(
                QuestionFlowStep.NotTakingPart, uprn: "100023336956",
                entryPoint: QuestionFlowStep.Address
            ),
            QuestionFlowStep.Address),
        new(
            "Not taking part goes back to confirm local authority if no UPRN found if was changing answer",
            new Input(
                QuestionFlowStep.NotTakingPart, uprn: null,
                entryPoint: QuestionFlowStep.Address
            ),
            QuestionFlowStep.ConfirmLocalAuthority),
        new(
            "Household income goes back to check answers if was changing answer",
            new Input(
                QuestionFlowStep.HouseholdIncome,
                entryPoint: QuestionFlowStep.HouseholdIncome
            ),
            QuestionFlowStep.CheckAnswers)
    };

    private static QuestionFlowServiceTestCase[] ForwardTestCases =
    {
        new(
            "Gas boiler continues to direct to ECO if the user has a boiler",
            new Input(
                QuestionFlowStep.GasBoiler,
                HasGasBoiler.Yes
            ),
            QuestionFlowStep.DirectToEco),
        new(
            "Gas boiler continues to country if the user doesn't have a boiler",
            new Input(
                QuestionFlowStep.GasBoiler,
                HasGasBoiler.No
            ),
            QuestionFlowStep.Country),
        new(
            "Country continues to ineligible Wales if the country is Wales",
            new Input(
                QuestionFlowStep.Country,
                country: Country.Wales
            ),
            QuestionFlowStep.IneligibleWales),
        new(
            "Country continues to ineligible Scotland if the country is Scotland",
            new Input(
                QuestionFlowStep.Country,
                country: Country.Scotland
            ),
            QuestionFlowStep.IneligibleScotland),
        new(
            "Country continues to ineligible Northern Ireland if the country is Northern Ireland",
            new Input(
                QuestionFlowStep.Country,
                country: Country.NorthernIreland
            ),
            QuestionFlowStep.IneligibleNorthernIreland),
        new(
            "Country continues to ownership status",
            new Input(
                QuestionFlowStep.Country,
                country: Country.England
            ),
            QuestionFlowStep.OwnershipStatus),
        new(
            "Ownership status continues to ineligible tenure if user is private tenant",
            new Input(
                QuestionFlowStep.OwnershipStatus,
                ownershipStatus: OwnershipStatus.PrivateTenancy
            ),
            QuestionFlowStep.IneligibleTenure),
        new(
            "Ownership status continues to ineligible tenure if user is landlord",
            new Input(
                QuestionFlowStep.OwnershipStatus,
                ownershipStatus: OwnershipStatus.Landlord
            ),
            QuestionFlowStep.IneligibleTenure),
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
            "Address selection continues to not taking part if local authority is not participating",
            new Input(
                QuestionFlowStep.SelectAddress,
                epcRating: EpcRating.D,
                custodianCode: "9052"
            ),
            QuestionFlowStep.NotTakingPart),
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
            "Address selection continues to household income if EPC is high, but expired",
            new Input(
                QuestionFlowStep.SelectAddress,
                epcRating: EpcRating.C,
                epcExpiry: DateTime.MinValue
            ),
            QuestionFlowStep.HouseholdIncome),
        new(
            "Review EPC continues to household income if EPC is incorrect",
            new Input(
                QuestionFlowStep.ReviewEpc,
                epcDetailsAreCorrect: EpcConfirmation.No
            ),
            QuestionFlowStep.HouseholdIncome),
        new(
            "Review EPC continues to household income if EPC is unsure",
            new Input(
                QuestionFlowStep.ReviewEpc,
                epcDetailsAreCorrect: EpcConfirmation.Unknown
            ),
            QuestionFlowStep.HouseholdIncome),
        new(
            "Review EPC continues to household income if EPC is correct",
            new Input(
                QuestionFlowStep.ReviewEpc,
                epcDetailsAreCorrect: EpcConfirmation.Yes
            ),
            QuestionFlowStep.HouseholdIncome),
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
            "Confirm local authority continues to not taking part if authority is not taking part",
            new Input(
                QuestionFlowStep.ConfirmLocalAuthority,
                localAuthorityIsCorrect: true,
                custodianCode: "9052"
            ),
            QuestionFlowStep.NotTakingPart),
        new(
            "Household income continues to check answers",
            new Input(
                QuestionFlowStep.HouseholdIncome
            ),
            QuestionFlowStep.CheckAnswers),
        new(
            "Check answers continues to eligible if eligible",
            new Input(
                QuestionFlowStep.CheckAnswers,
                HasGasBoiler.No,
                OwnershipStatus.OwnerOccupancy,
                Country.England,
                epcDetailsAreCorrect: EpcConfirmation.No,
                incomeBand: IncomeBand.UnderOrEqualTo36000
            ),
            QuestionFlowStep.Eligible),
        new(
            "Check answers continues to ineligible if ineligible",
            new Input(
                QuestionFlowStep.CheckAnswers,
                HasGasBoiler.Yes
            ),
            QuestionFlowStep.Ineligible),
        new(
            "Eligible continues to confirmation",
            new Input(
                QuestionFlowStep.Eligible
            ),
            QuestionFlowStep.Confirmation),
        new(
            "Confirmation continues to confirmation",
            new Input(
                QuestionFlowStep.Confirmation
            ),
            QuestionFlowStep.Confirmation),
        new(
            "Ineligible continues to ineligible",
            new Input(
                QuestionFlowStep.Ineligible
            ),
            QuestionFlowStep.Ineligible),
        new(
            "Gas boiler continues to direct to ECO if the user has a boiler and was changing answer",
            new Input(
                QuestionFlowStep.GasBoiler,
                HasGasBoiler.Yes,
                entryPoint: QuestionFlowStep.GasBoiler
            ),
            QuestionFlowStep.DirectToEco),
        new(
            "Gas boiler returns to check answers if user doesn't have a boiler, and was changing answer",
            new Input(
                QuestionFlowStep.GasBoiler,
                HasGasBoiler.No,
                entryPoint: QuestionFlowStep.GasBoiler
            ),
            QuestionFlowStep.CheckAnswers),
        new(
            "Country continues to ineligible Wales if the country is Wales and was changing answer",
            new Input(
                QuestionFlowStep.Country,
                country: Country.Wales,
                entryPoint: QuestionFlowStep.Country
            ),
            QuestionFlowStep.IneligibleWales),
        new(
            "Country continues to ineligible Scotland if the country is Scotland and was changing answer",
            new Input(
                QuestionFlowStep.Country,
                country: Country.Scotland,
                entryPoint: QuestionFlowStep.Country
            ),
            QuestionFlowStep.IneligibleScotland),
        new(
            "Country continues to ineligible Northern Ireland if the country is Northern Ireland and was changing answer",
            new Input(
                QuestionFlowStep.Country,
                country: Country.NorthernIreland,
                entryPoint: QuestionFlowStep.Country
            ),
            QuestionFlowStep.IneligibleNorthernIreland),
        new(
            "Country returns to check answers if the country is England was changing answer",
            new Input(
                QuestionFlowStep.Country,
                country: Country.England,
                entryPoint: QuestionFlowStep.Country
            ),
            QuestionFlowStep.CheckAnswers),
        new(
            "Ownership status continues to ineligible tenure if user is private tenant and was changing answer",
            new Input(
                QuestionFlowStep.OwnershipStatus,
                ownershipStatus: OwnershipStatus.PrivateTenancy,
                entryPoint: QuestionFlowStep.OwnershipStatus
            ),
            QuestionFlowStep.IneligibleTenure),
        new(
            "Ownership status continues to ineligible tenure if user is landlord and was changing answer",
            new Input(
                QuestionFlowStep.OwnershipStatus,
                ownershipStatus: OwnershipStatus.Landlord,
                entryPoint: QuestionFlowStep.OwnershipStatus
            ),
            QuestionFlowStep.IneligibleTenure),
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
            "Address selection continues to not taking part if local authority is not participating and was changing answer",
            new Input(
                QuestionFlowStep.SelectAddress,
                epcRating: EpcRating.D,
                custodianCode: "9052",
                entryPoint: QuestionFlowStep.Address
            ),
            QuestionFlowStep.NotTakingPart),
        new(
            "Address selection returns to check answers income if EPC is low and was changing answer",
            new Input(
                QuestionFlowStep.SelectAddress,
                epcRating: EpcRating.D,
                entryPoint: QuestionFlowStep.Address
            ),
            QuestionFlowStep.CheckAnswers),
        // disabling this test as we don't have live local authority on WHLG with income bands based on £34,000 at the moment
        // new(
        //     "Address selection continues to household income if authority is correct but income band is invalid and was changing answer",
        //     new Input(
        //         QuestionFlowStep.SelectAddress,
        //         epcRating: EpcRating.D,
        //         entryPoint: QuestionFlowStep.Address,
        //         incomeBand: IncomeBand.GreaterThan31000,
        //         custodianCode: "505" // Cambridge has income bands based on £34,000
        //     ),
        //     QuestionFlowStep.HouseholdIncome),
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
                epcDetailsAreCorrect: EpcConfirmation.No,
                entryPoint: QuestionFlowStep.Address
            ),
            QuestionFlowStep.CheckAnswers),
        new(
            "Review EPC returns to check answers if EPC is unsure and was changing answer",
            new Input(
                QuestionFlowStep.ReviewEpc,
                epcDetailsAreCorrect: EpcConfirmation.Unknown,
                entryPoint: QuestionFlowStep.Address
            ),
            QuestionFlowStep.CheckAnswers),
        // disabling this test as we don't have live local authority on WHLG with income bands based on £34,000 at the moment
        // new(
        //     "Review EPC continues to household income if authority is correct but income band is invalid and was changing answer",
        //     new Input(
        //         QuestionFlowStep.ReviewEpc,
        //         epcDetailsAreCorrect: EpcConfirmation.Yes,
        //         entryPoint: QuestionFlowStep.Address,
        //         incomeBand: IncomeBand.GreaterThan31000,
        //         custodianCode: "505" // Cambridge has income bands based on £34,000
        //     ),
        //     QuestionFlowStep.HouseholdIncome),
        new(
            "Review EPC continues to check answers if EPC is correct and was changing answer",
            new Input(
                QuestionFlowStep.ReviewEpc,
                epcDetailsAreCorrect: EpcConfirmation.Yes,
                entryPoint: QuestionFlowStep.Address
            ),
            QuestionFlowStep.CheckAnswers),
        new(
            "Manual address continues to select local authority if was changing answer",
            new Input(
                QuestionFlowStep.ManualAddress,
                entryPoint: QuestionFlowStep.Address
            ),
            QuestionFlowStep.SelectLocalAuthority),
        new(
            "Select local authority continues to confirm local authority if was changing answer",
            new Input(
                QuestionFlowStep.SelectLocalAuthority,
                entryPoint: QuestionFlowStep.Address
            ),
            QuestionFlowStep.ConfirmLocalAuthority),
        new(
            "Confirm local authority continues to select local authority if authority is incorrect and was changing answer",
            new Input(
                QuestionFlowStep.ConfirmLocalAuthority,
                localAuthorityIsCorrect: false,
                entryPoint: QuestionFlowStep.Address
            ),
            QuestionFlowStep.SelectLocalAuthority),
        // disabling this test as we don't have live local authority on WHLG with income bands based on £34,000 at the moment
        // new(
        //     "Confirm local authority continues to household income if authority is correct but income band is invalid and was changing answer",
        //     new Input(
        //         QuestionFlowStep.ConfirmLocalAuthority,
        //         localAuthorityIsCorrect: true,
        //         entryPoint: QuestionFlowStep.Address,
        //         incomeBand: IncomeBand.GreaterThan31000,
        //         custodianCode: "505" // Cambridge has income bands based on £34,000
        //     ),
        //     QuestionFlowStep.HouseholdIncome),
        new(
            "Confirm local authority continues to check answers if authority is correct if was changing answer",
            new Input(
                QuestionFlowStep.ConfirmLocalAuthority,
                localAuthorityIsCorrect: true,
                entryPoint: QuestionFlowStep.Address
            ),
            QuestionFlowStep.CheckAnswers),
        new(
            "Confirm local authority continues to not taking part if authority is not taking part if was changing answer",
            new Input(
                QuestionFlowStep.ConfirmLocalAuthority,
                localAuthorityIsCorrect: true,
                custodianCode: "9052",
                entryPoint: QuestionFlowStep.Address
            ),
            QuestionFlowStep.NotTakingPart),
        new(
            "Household income continues to check answers if was changing answer",
            new Input(
                QuestionFlowStep.HouseholdIncome,
                entryPoint: QuestionFlowStep.HouseholdIncome
            ),
            QuestionFlowStep.CheckAnswers)
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
            EpcConfirmation epcDetailsAreCorrect = EpcConfirmation.Yes,
            EpcRating? epcRating = null,
            DateTime? epcExpiry = null,
            IncomeBand? incomeBand = IncomeBand.UnderOrEqualTo36000,
            bool localAuthorityIsCorrect = false,
            string custodianCode = "3505", // Babergh has income threshold of £36,000 and is live
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
                EpcDetails =
                    epcRating == null ? null : new EpcDetails { EpcRating = epcRating, ExpiryDate = epcExpiry },
                IncomeBand = incomeBand,
                LocalAuthorityConfirmed = localAuthorityIsCorrect,
                CustodianCode = custodianCode
            };
            EntryPoint = entryPoint;
        }
    }
}