using System;
using FluentAssertions;
using NUnit.Framework;
using Tests.Helpers;
using WhlgPublicWebsite.BusinessLogic.Models;
using WhlgPublicWebsite.BusinessLogic.Models.Enums;
using WhlgPublicWebsite.BusinessLogic.Services.QuestionFlow;

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

    private static readonly string LiveCustodianCode =
        LocalAuthorityDataHelper.GetExampleCustodianCodeForStatus(LocalAuthorityData.LocalAuthorityStatus.Live);

    private static readonly string NoFundingCustodianCode =
        LocalAuthorityDataHelper.GetExampleCustodianCodeForStatus(LocalAuthorityData.LocalAuthorityStatus.NoFunding);

    private static readonly string NotParticipatingCustodianCode =
        LocalAuthorityDataHelper.GetExampleCustodianCodeForStatus(LocalAuthorityData.LocalAuthorityStatus
            .NotParticipating);

    // DESNZ-2080: Reinstate when an LA of ReferralsPaused is added
    // private static readonly string ReferralsPausedCustodianCode =
    //     LocalAuthorityDataHelper.GetExampleCustodianCodeForStatus(LocalAuthorityData.LocalAuthorityStatus.ReferralsPaused);

    private static readonly string PendingCustodianCode =
        LocalAuthorityDataHelper.GetExampleCustodianCodeForStatus(LocalAuthorityData.LocalAuthorityStatus.Pending);
    // DESNZ-1849: Reinstate when an LA of takingFutureReferrals is added
    // private static readonly string TakingFutureReferralsCustodianCode = LocalAuthorityDataHelper.GetExampleCustodianCodeForStatus(LocalAuthorityData.LocalAuthorityStatus.TakingFutureReferrals);

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

    [Test]
    public void NoTakingFutureReferralsLasExist()
    {
        // If this test starts failing, this is because we have written tests to cover LAs of this status but couldn't turn them on yet
        // Do the following:
        // 1. Search the project for "DESNZ-1849" and uncomment the snippets
        // 2. Change this test to .Should().NotThrow()
        // If in the future we lose all LAs of this status, this test will start failing again. To fix, do the reverse of the above.
        Action act = () =>
            LocalAuthorityDataHelper.GetExampleCustodianCodeForStatus(LocalAuthorityData.LocalAuthorityStatus
                .TakingFutureReferrals);
        act.Should().Throw<InvalidOperationException>();
    }

    [Test]
    [Ignore("DESNZ-2080: Cambridge temporarily enabled")]
    public void NoReferralsPausedLasExist()
    {
        // If this test starts failing, this is because we have written tests to cover LAs of this status but couldn't turn them on yet
        // Do the following:
        // 1. Search the project for "DESNZ-2080" and uncomment the snippets
        // 2. Change this test to .Should().NotThrow()
        // If in the future we lose all LAs of this status, this test will start failing again. To fix, do the reverse of the above.
        Action act = () =>
            LocalAuthorityDataHelper.GetExampleCustodianCodeForStatus(LocalAuthorityData.LocalAuthorityStatus
                .ReferralsPaused);
        act.Should().Throw<InvalidOperationException>();
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
            "No funding goes back to Address if LA matched",
            new Input(
                QuestionFlowStep.NoFunding, localAuthorityAutomaticallyMatched: true
            ),
            QuestionFlowStep.Address),
        new(
            "No funding goes back to confirm local authority if no LA matched",
            new Input(
                QuestionFlowStep.NoFunding, localAuthorityAutomaticallyMatched: false
            ),
            QuestionFlowStep.ConfirmLocalAuthority),
        new(
            "Not participating goes back to Address if LA matched",
            new Input(
                QuestionFlowStep.NotParticipating, localAuthorityAutomaticallyMatched: true
            ),
            QuestionFlowStep.Address),
        new(
            "Not participating goes back to confirm local authority if no LA matched",
            new Input(
                QuestionFlowStep.NotParticipating, localAuthorityAutomaticallyMatched: false
            ),
            QuestionFlowStep.ConfirmLocalAuthority),
        new(
            "Referrals paused goes back to Address if LA matched",
            new Input(
                QuestionFlowStep.ReferralsPaused, localAuthorityAutomaticallyMatched: true
            ),
            QuestionFlowStep.Address),
        new(
            "Referrals paused goes back to confirm local authority if no LA matched",
            new Input(
                QuestionFlowStep.ReferralsPaused, localAuthorityAutomaticallyMatched: false
            ),
            QuestionFlowStep.ConfirmLocalAuthority),
        new(
            "Household income goes back to Address if LA matched",
            new Input(
                QuestionFlowStep.HouseholdIncome, localAuthorityAutomaticallyMatched: true
            ),
            QuestionFlowStep.Address),
        new(
            "Household income goes back to confirm local authority if no LA matched",
            new Input(
                QuestionFlowStep.HouseholdIncome, localAuthorityAutomaticallyMatched: false
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
                QuestionFlowStep.HouseholdIncome, localAuthorityAutomaticallyMatched: true, epcRating: EpcRating.B,
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
            "Ineligible goes back to review EPC if EPC is ineligible",
            new Input(
                QuestionFlowStep.Ineligible,
                epcRating: EpcRating.C
            ),
            QuestionFlowStep.ReviewEpc),
        new(
            "Ineligible goes back to household income if income band is ineligible",
            new Input(
                QuestionFlowStep.Ineligible,
                incomeBand: IncomeBand.GreaterThan36000
            ),
            QuestionFlowStep.HouseholdIncome),
        new(
            "No consent goes back to eligible",
            new Input(
                QuestionFlowStep.NoConsent
            ),
            QuestionFlowStep.Eligible),
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
            "No funding goes back to Address if LA matched if was changing answer",
            new Input(
                QuestionFlowStep.NoFunding, localAuthorityAutomaticallyMatched: true,
                entryPoint: QuestionFlowStep.Address
            ),
            QuestionFlowStep.Address),
        new(
            "No funding goes back to confirm local authority if no LA matched if was changing answer",
            new Input(
                QuestionFlowStep.NotParticipating, localAuthorityAutomaticallyMatched: false,
                entryPoint: QuestionFlowStep.Address
            ),
            QuestionFlowStep.ConfirmLocalAuthority),
        new(
            "Not participating goes back to Address if LA matched if was changing answer",
            new Input(
                QuestionFlowStep.NotParticipating, localAuthorityAutomaticallyMatched: true,
                entryPoint: QuestionFlowStep.Address
            ),
            QuestionFlowStep.Address),
        new(
            "Not participating goes back to confirm local authority if no LA matched if was changing answer",
            new Input(
                QuestionFlowStep.NoFunding, localAuthorityAutomaticallyMatched: false,
                entryPoint: QuestionFlowStep.Address
            ),
            QuestionFlowStep.ConfirmLocalAuthority),
        new(
            "Referrals paused goes back to Address if LA matched if was changing answer",
            new Input(
                QuestionFlowStep.ReferralsPaused, localAuthorityAutomaticallyMatched: true,
                entryPoint: QuestionFlowStep.Address
            ),
            QuestionFlowStep.Address),
        new(
            "Referrals paused goes back to confirm local authority if no LA matched if was changing answer",
            new Input(
                QuestionFlowStep.ReferralsPaused, localAuthorityAutomaticallyMatched: false,
                entryPoint: QuestionFlowStep.Address
            ),
            QuestionFlowStep.ConfirmLocalAuthority),
        new(
            "Household income goes back to check answers if was changing answer",
            new Input(
                QuestionFlowStep.HouseholdIncome,
                entryPoint: QuestionFlowStep.HouseholdIncome
            ),
            QuestionFlowStep.CheckAnswers),
        new(
            "Pending goes back to check answers",
            new Input(QuestionFlowStep.Pending),
            QuestionFlowStep.CheckAnswers
        ),
        new(
            "Taking future referrals goes back to check answers",
            new Input(QuestionFlowStep.TakingFutureReferrals),
            QuestionFlowStep.CheckAnswers
        )
    };

    private static QuestionFlowServiceTestCase[] ForwardTestCases =
    {
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
                OwnershipStatus.PrivateTenancy
            ),
            QuestionFlowStep.IneligibleTenure),
        new(
            "Ownership status continues to ineligible tenure if user is landlord",
            new Input(
                QuestionFlowStep.OwnershipStatus,
                OwnershipStatus.Landlord
            ),
            QuestionFlowStep.IneligibleTenure),
        new(
            "Ownership status continues to address if user is owner occupier",
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
        new(
            "Address selection continues to select Local Authority if no LA matched",
            new Input(
                QuestionFlowStep.SelectAddress,
                localAuthorityAutomaticallyMatched: false
            ),
            QuestionFlowStep.SelectLocalAuthority),
        new(
            "Address selection continues to household income if LA matched if EPC is low",
            new Input(
                QuestionFlowStep.SelectAddress,
                epcRating: EpcRating.D,
                localAuthorityAutomaticallyMatched: true
            ),
            QuestionFlowStep.HouseholdIncome),
        new(
            "Address selection continues to review EPC if LA matched if EPC is high",
            new Input(
                QuestionFlowStep.SelectAddress,
                epcRating: EpcRating.C,
                localAuthorityAutomaticallyMatched: true
            ),
            QuestionFlowStep.ReviewEpc),
        new(
            "Address selection continues to household income if LA matched if EPC is high, but expired",
            new Input(
                QuestionFlowStep.SelectAddress,
                epcRating: EpcRating.C,
                epcExpiry: DateTime.MinValue,
                localAuthorityAutomaticallyMatched: true
            ),
            QuestionFlowStep.HouseholdIncome),
        new(
            "Review EPC continues to household income if EPC is incorrect and eligible",
            new Input(
                QuestionFlowStep.ReviewEpc,
                epcDetailsAreCorrect: EpcConfirmation.No,
                epcRating: EpcRating.D
            ),
            QuestionFlowStep.HouseholdIncome),
        new(
            "Review EPC continues to household income if EPC is incorrect and ineligible",
            new Input(
                QuestionFlowStep.ReviewEpc,
                epcDetailsAreCorrect: EpcConfirmation.No,
                epcRating: EpcRating.C
            ),
            QuestionFlowStep.HouseholdIncome),
        new(
            "Review EPC continues to household income if EPC is unsure and eligible",
            new Input(
                QuestionFlowStep.ReviewEpc,
                epcDetailsAreCorrect: EpcConfirmation.Unknown,
                epcRating: EpcRating.D
            ),
            QuestionFlowStep.HouseholdIncome),
        new(
            "Review EPC continues to household income if EPC is unsure and ineligible",
            new Input(
                QuestionFlowStep.ReviewEpc,
                epcDetailsAreCorrect: EpcConfirmation.Unknown,
                epcRating: EpcRating.C
            ),
            QuestionFlowStep.HouseholdIncome),
        new(
            "Review EPC continues to household income if EPC is correct and eligible",
            new Input(
                QuestionFlowStep.ReviewEpc,
                epcDetailsAreCorrect: EpcConfirmation.Yes,
                epcRating: EpcRating.D
            ),
            QuestionFlowStep.HouseholdIncome),
        new(
            "Review EPC continues to ineligible if EPC is correct and ineligible",
            new Input(
                QuestionFlowStep.ReviewEpc,
                epcDetailsAreCorrect: EpcConfirmation.Yes,
                epcRating: EpcRating.C
            ),
            QuestionFlowStep.Ineligible),
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
            "Confirm local authority continues to no funding if authority is no funding",
            new Input(
                QuestionFlowStep.ConfirmLocalAuthority,
                localAuthorityIsCorrect: true,
                custodianCode: NoFundingCustodianCode
            ),
            QuestionFlowStep.NoFunding),
        // DESNZ-2080: Reinstate when an LA of ReferralsPaused is added
        // new(
        //     "Confirm local authority continues to referrals paused if authority is paused",
        //     new Input(
        //         QuestionFlowStep.ConfirmLocalAuthority,
        //         localAuthorityIsCorrect: true,
        //         custodianCode: ReferralsPausedCustodianCode
        //     ),
        //     QuestionFlowStep.NoFunding),
        new(
            "Confirm local authority continues to not participating if authority is participating",
            new Input(
                QuestionFlowStep.ConfirmLocalAuthority,
                localAuthorityIsCorrect: true,
                custodianCode: NotParticipatingCustodianCode
            ),
            QuestionFlowStep.NotParticipating),
        new(
            "Household income continues to check answers if income is eligible",
            new Input(
                QuestionFlowStep.HouseholdIncome,
                incomeBand: IncomeBand.UnderOrEqualTo36000
            ),
            QuestionFlowStep.CheckAnswers),
        new(
            "Household income continues to ineligible if income is ineligible",
            new Input(
                QuestionFlowStep.HouseholdIncome,
                incomeBand: IncomeBand.GreaterThan36000
            ),
            QuestionFlowStep.Ineligible),
        new(
            "Check answers continues to eligible if LA is live",
            new Input(
                QuestionFlowStep.CheckAnswers,
                custodianCode: LiveCustodianCode
            ),
            QuestionFlowStep.Eligible),
        new(
            "Check answers continues to pending if LA is pending",
            new Input(
                QuestionFlowStep.CheckAnswers,
                custodianCode: PendingCustodianCode
            ),
            QuestionFlowStep.Pending),
        // DESNZ-1849: Reinstate when an LA of takingFutureReferrals is added
        // new(
        //     "Check answers continues to taking future referrals if LA is taking future referrals",
        //     new Input(
        //         QuestionFlowStep.CheckAnswers,
        //         custodianCode: TakingFutureReferralsCustodianCode
        //     ),
        //     QuestionFlowStep.TakingFutureReferrals),
        new(
            "Pending continues to eligible",
            new Input(
                QuestionFlowStep.Pending
            ),
            QuestionFlowStep.Eligible),
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
                OwnershipStatus.PrivateTenancy,
                entryPoint: QuestionFlowStep.OwnershipStatus
            ),
            QuestionFlowStep.IneligibleTenure),
        new(
            "Ownership status continues to ineligible tenure if user is landlord and was changing answer",
            new Input(
                QuestionFlowStep.OwnershipStatus,
                OwnershipStatus.Landlord,
                entryPoint: QuestionFlowStep.OwnershipStatus
            ),
            QuestionFlowStep.IneligibleTenure),
        new(
            "Ownership status returns to check answers if user is owner occupier and was changing answer",
            new Input(
                QuestionFlowStep.OwnershipStatus,
                OwnershipStatus.OwnerOccupancy,
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
            "Address selection continues to select Local Authority if no LA matched and was changing answer",
            new Input(
                QuestionFlowStep.SelectAddress,
                localAuthorityAutomaticallyMatched: false,
                entryPoint: QuestionFlowStep.Address
            ),
            QuestionFlowStep.SelectLocalAuthority),
        new(
            "Address selection returns to check answers income if LA matched if EPC is low and was changing answer",
            new Input(
                QuestionFlowStep.SelectAddress,
                epcRating: EpcRating.D,
                localAuthorityAutomaticallyMatched: true,
                entryPoint: QuestionFlowStep.Address
            ),
            QuestionFlowStep.CheckAnswers),
        new(
            "Address selection continues to review EPC if LA matched if EPC is high and was changing answer",
            new Input(
                QuestionFlowStep.SelectAddress,
                epcRating: EpcRating.C,
                localAuthorityAutomaticallyMatched: true,
                entryPoint: QuestionFlowStep.Address
            ),
            QuestionFlowStep.ReviewEpc),
        new(
            "Review EPC returns to check answers if EPC is incorrect and was changing answer",
            new Input(
                QuestionFlowStep.ReviewEpc,
                epcDetailsAreCorrect: EpcConfirmation.No,
                entryPoint: QuestionFlowStep.Address,
                epcRating: EpcRating.D
            ),
            QuestionFlowStep.CheckAnswers),
        new(
            "Review EPC returns to check answers if EPC is unsure and was changing answer",
            new Input(
                QuestionFlowStep.ReviewEpc,
                epcDetailsAreCorrect: EpcConfirmation.Unknown,
                entryPoint: QuestionFlowStep.Address,
                epcRating: EpcRating.D
            ),
            QuestionFlowStep.CheckAnswers),
        new(
            "Review EPC returns to check answers if EPC is correct and eligible and was changing answer",
            new Input(
                QuestionFlowStep.ReviewEpc,
                epcDetailsAreCorrect: EpcConfirmation.Yes,
                entryPoint: QuestionFlowStep.Address,
                epcRating: EpcRating.D
            ),
            QuestionFlowStep.CheckAnswers),
        new(
            "Review EPC continues to ineligible if EPC is correct and ineligible and was changing answer",
            new Input(
                QuestionFlowStep.ReviewEpc,
                epcDetailsAreCorrect: EpcConfirmation.Yes,
                entryPoint: QuestionFlowStep.Address,
                epcRating: EpcRating.C
            ),
            QuestionFlowStep.Ineligible),
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
        new(
            "Confirm local authority returns to check answers if authority is correct if was changing answer",
            new Input(
                QuestionFlowStep.ConfirmLocalAuthority,
                localAuthorityIsCorrect: true,
                entryPoint: QuestionFlowStep.Address
            ),
            QuestionFlowStep.CheckAnswers),
        new(
            "Confirm local authority continues to not participating if authority is not participating if was changing answer",
            new Input(
                QuestionFlowStep.ConfirmLocalAuthority,
                localAuthorityIsCorrect: true,
                custodianCode: NotParticipatingCustodianCode,
                entryPoint: QuestionFlowStep.Address
            ),
            QuestionFlowStep.NotParticipating),
        new(
            "Household income returns to check answers if income is eligible and was changing answer",
            new Input(
                QuestionFlowStep.HouseholdIncome,
                entryPoint: QuestionFlowStep.HouseholdIncome,
                incomeBand: IncomeBand.UnderOrEqualTo36000
            ),
            QuestionFlowStep.CheckAnswers),
        new(
            "Household income continues to ineligible if income is ineligible and was changing answer",
            new Input(
                QuestionFlowStep.HouseholdIncome,
                entryPoint: QuestionFlowStep.HouseholdIncome,
                incomeBand: IncomeBand.GreaterThan36000
            ),
            QuestionFlowStep.Ineligible)
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
            string uprn = null,
            EpcConfirmation epcDetailsAreCorrect = EpcConfirmation.Yes,
            EpcRating? epcRating = null,
            DateTime? epcExpiry = null,
            IncomeBand? incomeBand = IncomeBand.UnderOrEqualTo36000,
            bool localAuthorityIsCorrect = false,
            string custodianCode = null,
            QuestionFlowStep? entryPoint = null,
            bool localAuthorityAutomaticallyMatched = false)
        {
            Page = page;
            Questionnaire = new Questionnaire
            {
                Uprn = uprn,
                OwnershipStatus = ownershipStatus,
                Country = country,
                EpcDetailsAreCorrect = epcDetailsAreCorrect,
                EpcDetails =
                    epcRating == null ? null : new EpcDetails { EpcRating = epcRating, ExpiryDate = epcExpiry },
                IncomeBand = incomeBand,
                LocalAuthorityConfirmed = localAuthorityIsCorrect,
                CustodianCode = custodianCode ??
                                LocalAuthorityDataHelper.GetExampleCustodianCodeForStatus(LocalAuthorityData
                                    .LocalAuthorityStatus.Live),
                LocalAuthorityAutomaticallyMatched = localAuthorityAutomaticallyMatched
            };
            EntryPoint = entryPoint;
        }
    }
}