using FluentAssertions;
using NUnit.Framework;
using HerPublicWebsite.BusinessLogic.Models;
using HerPublicWebsite.BusinessLogic.Models.Enums;
using HerPublicWebsite.BusinessLogic.Services;

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
            "Country goes back to start",
            new Input(
                QuestionFlowStep.Country
            ),
            QuestionFlowStep.Start),
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
            "Service unsuitable goes back to the country you came from",
            new Input(
                QuestionFlowStep.ServiceUnsuitable,
                country: Country.Other
            ),
            QuestionFlowStep.Country),
        new(
            "Service unsuitable goes back to ownership status if user is not owner occupier",
            new Input(
                QuestionFlowStep.ServiceUnsuitable,
                country: Country.England,
                ownershipStatus: OwnershipStatus.PrivateTenancy
            ),
            QuestionFlowStep.OwnershipStatus),
    };

    private static QuestionFlowServiceTestCase[] ForwardTestCases =
    {
        new(
            "Country continues to service unsuitable if the service is not available",
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
            "Ownership status continues to service unsuitable if user is not owner occupier",
            new Input(
                QuestionFlowStep.OwnershipStatus,
                OwnershipStatus.PrivateTenancy
            ),
            QuestionFlowStep.ServiceUnsuitable),
        new(
            "Ownership status continues to address",
            new Input(
                QuestionFlowStep.OwnershipStatus,
                OwnershipStatus.OwnerOccupancy
            ),
            QuestionFlowStep.Address),
        new(
            "Address continues to address selection",
            new Input(
                QuestionFlowStep.Address
            ),
            QuestionFlowStep.SelectAddress),
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
            OwnershipStatus? ownershipStatus = null,
            Country? country = null,
            QuestionFlowStep? entryPoint = null)
        {
            Page = page;
            Questionnaire = new Questionnaire
            {
                OwnershipStatus = ownershipStatus,
                Country = country,
            };
            EntryPoint = entryPoint;
        }
    }
}
