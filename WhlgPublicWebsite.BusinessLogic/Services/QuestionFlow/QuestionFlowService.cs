using WhlgPublicWebsite.BusinessLogic.Models;
using WhlgPublicWebsite.BusinessLogic.Models.Enums;

namespace WhlgPublicWebsite.BusinessLogic.Services.QuestionFlow;

public interface IQuestionFlowService
{
    public QuestionFlowStep PreviousStep(QuestionFlowStep page, Questionnaire questionnaire,
        QuestionFlowStep? entryPoint = null);

    public QuestionFlowStep NextStep(QuestionFlowStep page, Questionnaire questionnaire,
        QuestionFlowStep? entryPoint = null);
}

public class QuestionFlowService : IQuestionFlowService
{
    public QuestionFlowStep PreviousStep(
        QuestionFlowStep page,
        Questionnaire questionnaire,
        QuestionFlowStep? entryPoint = null)
    {
        return page switch
        {
            QuestionFlowStep.Country => CountryBackDestination(entryPoint),
            QuestionFlowStep.IneligibleWales => IneligibleWalesBackDestination(),
            QuestionFlowStep.IneligibleScotland => IneligibleScotlandBackDestination(),
            QuestionFlowStep.IneligibleNorthernIreland => IneligibleNorthernIrelandBackDestination(),
            QuestionFlowStep.OwnershipStatus => OwnershipStatusBackDestination(entryPoint),
            QuestionFlowStep.IneligibleTenure => IneligibleTenureBackDestination(),
            QuestionFlowStep.Address => AddressBackDestination(entryPoint),
            QuestionFlowStep.SelectAddress => SelectAddressBackDestination(),
            QuestionFlowStep.ReviewEpc => ReviewEpcBackDestination(),
            QuestionFlowStep.NotParticipating => NotParticipatingBackDestination(questionnaire),
            QuestionFlowStep.NoFunding => NoFundingBackDestination(questionnaire),
            QuestionFlowStep.NoLongerParticipating => NoLongerParticipatingBackDestination(questionnaire),
            QuestionFlowStep.TakingFutureReferrals => TakingFutureReferralsBackDestination(),
            QuestionFlowStep.Pending => PendingBackDestination(),
            QuestionFlowStep.ManualAddress => ManualAddressBackDestination(),
            QuestionFlowStep.SelectLocalAuthority => SelectLocalAuthorityBackDestination(),
            QuestionFlowStep.ConfirmLocalAuthority => ConfirmLocalAuthorityBackDestination(),
            QuestionFlowStep.HouseholdIncome => HouseholdIncomeBackDestination(questionnaire, entryPoint),
            QuestionFlowStep.CheckAnswers => CheckAnswersBackDestination(),
            QuestionFlowStep.Eligible => EligibleBackDestination(questionnaire),
            QuestionFlowStep.Confirmation => ConfirmationBackDestination(),
            QuestionFlowStep.NoConsent => NoConsentBackDestination(),
            QuestionFlowStep.Ineligible => IneligibleBackDestination(questionnaire),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public QuestionFlowStep NextStep(QuestionFlowStep page, Questionnaire questionnaire,
        QuestionFlowStep? entryPoint = null)
    {
        return page switch
        {
            QuestionFlowStep.Country => CountryForwardDestination(questionnaire, entryPoint),
            QuestionFlowStep.OwnershipStatus => OwnershipStatusForwardDestination(questionnaire, entryPoint),
            QuestionFlowStep.Address => AddressForwardDestination(),
            QuestionFlowStep.SelectAddress => SelectAddressForwardDestination(questionnaire, entryPoint),
            QuestionFlowStep.ReviewEpc => ReviewEpcForwardDestination(questionnaire, entryPoint),
            QuestionFlowStep.ManualAddress => ManualAddressForwardDestination(),
            QuestionFlowStep.SelectLocalAuthority => SelectLocalAuthorityForwardDestination(),
            QuestionFlowStep.ConfirmLocalAuthority =>
                ConfirmLocalAuthorityForwardDestination(questionnaire, entryPoint),
            QuestionFlowStep.NotParticipating => NotParticipatingForwardDestination(),
            QuestionFlowStep.NoFunding => NoFundingForwardDestination(),
            QuestionFlowStep.NoLongerParticipating => NoLongerParticipatingForwardDestination(),
            QuestionFlowStep.TakingFutureReferrals =>
                TakingFutureReferralsForwardDestination(),
            QuestionFlowStep.Pending => PendingForwardDestination(),
            QuestionFlowStep.HouseholdIncome => HouseholdIncomeForwardDestination(questionnaire),
            QuestionFlowStep.CheckAnswers => CheckAnswersForwardDestination(questionnaire),
            QuestionFlowStep.Eligible => EligibleForwardDestination(),
            QuestionFlowStep.Confirmation => ConfirmationForwardDestination(),
            QuestionFlowStep.Ineligible => IneligibleForwardDestination(),
            _ => throw new ArgumentOutOfRangeException(nameof(page), page, null)
        };
    }

    private QuestionFlowStep CountryBackDestination(QuestionFlowStep? entryPoint)
    {
        return entryPoint switch
        {
            QuestionFlowStep.Country => QuestionFlowStep.CheckAnswers,
            _ => QuestionFlowStep.Start
        };
    }

    private QuestionFlowStep IneligibleWalesBackDestination()
    {
        return QuestionFlowStep.Country;
    }

    private QuestionFlowStep IneligibleScotlandBackDestination()
    {
        return QuestionFlowStep.Country;
    }

    private QuestionFlowStep IneligibleNorthernIrelandBackDestination()
    {
        return QuestionFlowStep.Country;
    }

    private QuestionFlowStep OwnershipStatusBackDestination(QuestionFlowStep? entryPoint)
    {
        return entryPoint switch
        {
            QuestionFlowStep.OwnershipStatus => QuestionFlowStep.CheckAnswers,
            _ => QuestionFlowStep.Country
        };
    }

    private QuestionFlowStep IneligibleTenureBackDestination()
    {
        return QuestionFlowStep.OwnershipStatus;
    }

    private QuestionFlowStep AddressBackDestination(QuestionFlowStep? entryPoint)
    {
        return entryPoint switch
        {
            QuestionFlowStep.Address => QuestionFlowStep.CheckAnswers,
            _ => QuestionFlowStep.OwnershipStatus
        };
    }

    private QuestionFlowStep SelectAddressBackDestination()
    {
        return QuestionFlowStep.Address;
    }

    private QuestionFlowStep ManualAddressBackDestination()
    {
        return QuestionFlowStep.Address;
    }

    private QuestionFlowStep SelectLocalAuthorityBackDestination()
    {
        return QuestionFlowStep.ManualAddress;
    }

    private QuestionFlowStep ConfirmLocalAuthorityBackDestination()
    {
        return QuestionFlowStep.SelectLocalAuthority;
    }

    private QuestionFlowStep LaStatusSwitchBackDestionation(Questionnaire questionnaire)
    {
        return questionnaire.Uprn == null ? QuestionFlowStep.ConfirmLocalAuthority : QuestionFlowStep.Address;
    }

    private QuestionFlowStep ReviewEpcBackDestination()
    {
        // it is never possible that the user needs to go back to confirm authority
        // since the manual flow will never find an EPC
        return QuestionFlowStep.Address;
    }

    private QuestionFlowStep NoFundingBackDestination(Questionnaire questionnaire)
    {
        return LaStatusSwitchBackDestionation(questionnaire);
    }

    private QuestionFlowStep NotParticipatingBackDestination(Questionnaire questionnaire)
    {
        return LaStatusSwitchBackDestionation(questionnaire);
    }

    private QuestionFlowStep NoLongerParticipatingBackDestination(Questionnaire questionnaire)
    {
        return LaStatusSwitchBackDestionation(questionnaire);
    }

    private QuestionFlowStep HouseholdIncomeBackDestination(Questionnaire questionnaire, QuestionFlowStep? entryPoint)
    {
        if (entryPoint is QuestionFlowStep.HouseholdIncome)
            return QuestionFlowStep.CheckAnswers;
        if (questionnaire.FoundEpcBandIsTooHigh)
            return QuestionFlowStep.ReviewEpc;

        return LaStatusSwitchBackDestionation(questionnaire);
    }

    private QuestionFlowStep IneligibleBackDestination(Questionnaire questionnaire)
    {
        return questionnaire.IncomeIsTooHigh ? QuestionFlowStep.HouseholdIncome : QuestionFlowStep.ReviewEpc;
    }

    private QuestionFlowStep CheckAnswersBackDestination()
    {
        return QuestionFlowStep.HouseholdIncome;
    }

    private QuestionFlowStep TakingFutureReferralsBackDestination()
    {
        return QuestionFlowStep.CheckAnswers;
    }

    private QuestionFlowStep PendingBackDestination()
    {
        return QuestionFlowStep.CheckAnswers;
    }

    private QuestionFlowStep EligibleBackDestination(Questionnaire questionnaire)
    {
        return questionnaire.LocalAuthorityStatus switch
        {
            LocalAuthorityData.LocalAuthorityStatus.Pending => QuestionFlowStep.Pending,
            LocalAuthorityData.LocalAuthorityStatus.TakingFutureReferrals => QuestionFlowStep.TakingFutureReferrals,
            _ => QuestionFlowStep.CheckAnswers
        };
    }

    private QuestionFlowStep NoConsentBackDestination()
    {
        return QuestionFlowStep.Eligible;
    }

    private QuestionFlowStep ConfirmationBackDestination()
    {
        return QuestionFlowStep.Eligible;
    }

    private QuestionFlowStep CountryForwardDestination(Questionnaire questionnaire, QuestionFlowStep? entryPoint)
    {
        return (entryPoint, questionnaire.Country) switch
        {
            (_, Country.Wales) => QuestionFlowStep.IneligibleWales,
            (_, Country.Scotland) => QuestionFlowStep.IneligibleScotland,
            (_, Country.NorthernIreland) => QuestionFlowStep.IneligibleNorthernIreland,
            (QuestionFlowStep.Country, _) => QuestionFlowStep.CheckAnswers,
            _ => QuestionFlowStep.OwnershipStatus
        };
    }

    private QuestionFlowStep OwnershipStatusForwardDestination(Questionnaire questionnaire,
        QuestionFlowStep? entryPoint)
    {
        return (entryPoint, questionnaire.OwnershipStatus) switch
        {
            (_, not OwnershipStatus.OwnerOccupancy) => QuestionFlowStep.IneligibleTenure,
            (QuestionFlowStep.OwnershipStatus, _) => QuestionFlowStep.CheckAnswers,
            _ => QuestionFlowStep.Address
        };
    }

    private QuestionFlowStep AddressForwardDestination()
    {
        return QuestionFlowStep.SelectAddress;
    }

    private QuestionFlowStep LaStatusSwitchForwardDestination(Questionnaire questionnaire, QuestionFlowStep? entryPoint)
    {
        if (questionnaire.LocalAuthorityStatus is LocalAuthorityData.LocalAuthorityStatus.NoFunding)
            return QuestionFlowStep.NoFunding;
#pragma warning disable CS0618 // Type or member is obsolete
        if (questionnaire.LocalAuthorityStatus is LocalAuthorityData.LocalAuthorityStatus.NotParticipating)
#pragma warning restore CS0618 // Type or member is obsolete
            return QuestionFlowStep.NotParticipating;
#pragma warning disable CS0618 // Type or member is obsolete
        if (questionnaire.LocalAuthorityStatus is LocalAuthorityData.LocalAuthorityStatus.NoLongerParticipating)
#pragma warning restore CS0618 // Type or member is obsolete
            return QuestionFlowStep.NoLongerParticipating;
        if (questionnaire.FoundEpcBandIsTooHigh)
            return QuestionFlowStep.ReviewEpc;
        // If the LA has changed and the income band the user selected previously is no longer valid then we don't
        // go back to the check your answers page as the user will need to select a new income band.
        if (entryPoint is QuestionFlowStep.Address && questionnaire.IncomeBandIsValid)
            return QuestionFlowStep.CheckAnswers;

        return QuestionFlowStep.HouseholdIncome;
    }

    private QuestionFlowStep SelectAddressForwardDestination(Questionnaire questionnaire, QuestionFlowStep? entryPoint)
    {
        return LaStatusSwitchForwardDestination(questionnaire, entryPoint);
    }

    private QuestionFlowStep ManualAddressForwardDestination()
    {
        return QuestionFlowStep.SelectLocalAuthority;
    }

    private QuestionFlowStep SelectLocalAuthorityForwardDestination()
    {
        return QuestionFlowStep.ConfirmLocalAuthority;
    }

    private QuestionFlowStep ConfirmLocalAuthorityForwardDestination(Questionnaire questionnaire,
        QuestionFlowStep? entryPoint)
    {
        if (questionnaire.LocalAuthorityConfirmed is not true)
            return QuestionFlowStep.SelectLocalAuthority;

        return LaStatusSwitchForwardDestination(questionnaire, entryPoint);
    }

    private QuestionFlowStep NoFundingForwardDestination()
    {
        return QuestionFlowStep.NoFunding;
    }

    private QuestionFlowStep NotParticipatingForwardDestination()
    {
        return QuestionFlowStep.NotParticipating;
    }

    private QuestionFlowStep NoLongerParticipatingForwardDestination()
    {
        return QuestionFlowStep.NoLongerParticipating;
    }

    private QuestionFlowStep ReviewEpcForwardDestination(Questionnaire questionnaire, QuestionFlowStep? entryPoint)
    {
        if (questionnaire.EpcIsTooHigh)
            return QuestionFlowStep.Ineligible;

        // If the LA has changed and the income band the user selected previously is no longer valid then we don't
        // go back to the check your answers page as the user will need to select a new income band.
        if (entryPoint is QuestionFlowStep.Address && questionnaire.IncomeBandIsValid)
            return QuestionFlowStep.CheckAnswers;

        return QuestionFlowStep.HouseholdIncome;
    }

    private QuestionFlowStep HouseholdIncomeForwardDestination(Questionnaire questionnaire)
    {
        return questionnaire.IncomeIsTooHigh ? QuestionFlowStep.Ineligible : QuestionFlowStep.CheckAnswers;
    }

    private QuestionFlowStep CheckAnswersForwardDestination(Questionnaire questionnaire)
    {
        return questionnaire.LocalAuthorityStatus switch
        {
            LocalAuthorityData.LocalAuthorityStatus.Pending => QuestionFlowStep.Pending,
            LocalAuthorityData.LocalAuthorityStatus.TakingFutureReferrals => QuestionFlowStep.TakingFutureReferrals,
            _ => QuestionFlowStep.Eligible
        };
    }

    private QuestionFlowStep PendingForwardDestination()
    {
        return QuestionFlowStep.Eligible;
    }

    private QuestionFlowStep TakingFutureReferralsForwardDestination()
    {
        return QuestionFlowStep.Eligible;
    }

    private QuestionFlowStep EligibleForwardDestination()
    {
        return QuestionFlowStep.Confirmation;
    }

    private QuestionFlowStep ConfirmationForwardDestination()
    {
        return QuestionFlowStep.Confirmation;
    }

    private QuestionFlowStep IneligibleForwardDestination()
    {
        return QuestionFlowStep.Ineligible;
    }
}