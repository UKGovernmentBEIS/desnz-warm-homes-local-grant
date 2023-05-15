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
            "Manual address goes back to Address",
            new Input(
                QuestionFlowStep.ManualAddress
            ),
            QuestionFlowStep.Address),
        new(
            "Household income goes back to Address if UPRN found",
            new Input(
                QuestionFlowStep.HouseholdIncome, uprn: "100023336956"
            ),
            QuestionFlowStep.Address),
        new(
            "Household income goes back to Manual address if no UPRN found",
            new Input(
                QuestionFlowStep.HouseholdIncome, uprn: null
            ),
            QuestionFlowStep.ManualAddress),
        new(
            "Household income goes back to Gas boiler",
            new Input(
                QuestionFlowStep.HouseholdIncome
            ),
            QuestionFlowStep.GasBoiler),
        new(
            "Service unsuitable goes back to the country you came from",
            new Input(
                QuestionFlowStep.ServiceUnsuitable,
                country: Country.Other
            ),
            QuestionFlowStep.Country),
        new(
            "Service unsuitable goes back to ownership status if country is filled in",
            new Input(
                QuestionFlowStep.ServiceUnsuitable,
                country: Country.England
            ),
            QuestionFlowStep.OwnershipStatus),
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
            "Address selection continues to household income",
            new Input(
                QuestionFlowStep.SelectAddress
            ),
            QuestionFlowStep.HouseholdIncome),
        new(
            "Manual address continues to household income",
            new Input(
                QuestionFlowStep.ManualAddress
            ),
            QuestionFlowStep.HouseholdIncome),
        new(
            "Household income continues to check answers",
            new Input(
                QuestionFlowStep.HouseholdIncome
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
            QuestionFlowStep? entryPoint = null)
        {
            Page = page;
            Questionnaire = new Questionnaire
            {
                HasGasBoiler = hasGasBoiler,
                Uprn = uprn,
                OwnershipStatus = ownershipStatus,
                Country = country,
            };
            EntryPoint = entryPoint;
        }
    }
}
