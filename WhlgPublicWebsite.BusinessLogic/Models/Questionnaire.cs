using WhlgPublicWebsite.BusinessLogic.Models.Enums;
using IncomeBandEnum = WhlgPublicWebsite.BusinessLogic.Models.Enums.IncomeBand;
using CountryEnum = WhlgPublicWebsite.BusinessLogic.Models.Enums.Country;
using OwnershipStatusEnum = WhlgPublicWebsite.BusinessLogic.Models.Enums.OwnershipStatus;

namespace WhlgPublicWebsite.BusinessLogic.Models;

public record Questionnaire
{
    public int? SessionId { get; set; }

    public Country? Country { get; set; }
    public OwnershipStatus? OwnershipStatus { get; set; }

    public string PostcodeSearched { get; set; }
    public string AddressLine1 { get; set; }
    public string AddressLine2 { get; set; }
    public string AddressTown { get; set; }
    public string AddressCounty { get; set; }
    public string AddressPostcode { get; set; }

    public string CustodianCode { get; set; }
    public bool LocalAuthorityAutomaticallyMatched { get; set; }
    public bool? LocalAuthorityConfirmed { get; set; }

    public string Uprn { get; set; } // Should be populated for most questionnaires, but not 100% guaranteed

    public EpcDetails EpcDetails { get; set; }
    public EpcConfirmation? EpcDetailsAreCorrect { get; set; }

    /// <summary>
    /// Whether the postcode is in IMD postcode deciles 1-2.
    /// If so, the income band is not used to determine eligibility.
    /// See <seealso cref="WhlgPublicWebsite.BusinessLogic.Services.EligiblePostcode.EligiblePostcodeService"/>
    /// </summary>
    public bool? IsImdPostcode { get; set; }

    public bool? AcknowledgedPending { get; set; }
    public bool? AcknowledgedFutureReferral { get; set; }
    public IncomeBand? IncomeBand { get; set; }

    public DateTime ReferralCreated { get; set; }

    public string ReferralCode { get; set; }

    public string LaContactName { get; set; }
    public bool? LaCanContactByEmail { get; set; }
    public bool? LaCanContactByPhone { get; set; }
    public string LaContactEmailAddress { get; set; }
    public string LaContactTelephone { get; set; }

    public bool? NotificationConsent { get; set; }
    public bool? ConfirmationConsent { get; set; }
    public string NotificationEmailAddress { get; set; }

    public string ConfirmationEmailAddress { get; set; }

    public Questionnaire UneditedData { get; set; }

    public string LocalAuthorityName
    {
        get
        {
            if (string.IsNullOrEmpty(CustodianCode) ||
                !LocalAuthorityData.LocalAuthorityDetailsByCustodianCode.ContainsKey(CustodianCode))
            {
                return "unrecognised Local Authority";
            }

            return LocalAuthorityData.LocalAuthorityDetailsByCustodianCode[CustodianCode].Name;
        }
    }

    public string LocalAuthorityWebsite
    {
        get
        {
            if (string.IsNullOrEmpty(CustodianCode) ||
                !LocalAuthorityData.LocalAuthorityDetailsByCustodianCode.ContainsKey(CustodianCode))
            {
                return "unrecognised Local Authority";
            }

            return LocalAuthorityData.LocalAuthorityDetailsByCustodianCode[CustodianCode].WebsiteUrl;
        }
    }

    public LocalAuthorityData.LocalAuthorityStatus? LocalAuthorityStatus
    {
        get
        {
            if (string.IsNullOrEmpty(CustodianCode) ||
                !LocalAuthorityData.LocalAuthorityDetailsByCustodianCode.ContainsKey(CustodianCode))
            {
                return null;
            }

            return LocalAuthorityData.LocalAuthorityDetailsByCustodianCode[CustodianCode].Status;
        }
    }

    public EpcRating EffectiveEpcBand
    {
        get
        {
            if (EpcDetails is null)
            {
                return EpcRating.Unknown;
            }

            if (EpcDetailsAreCorrect is EpcConfirmation.No or EpcConfirmation.Unknown)
            {
                return EpcRating.Unknown;
            }

            if (EpcDetails.ExpiryDate < DateTime.Now)
            {
                return EpcRating.Expired;
            }

            return EpcDetails.EpcRating ?? EpcRating.Unknown;
        }
    }

    public EpcRating DisplayEpcRating
    {
        get
        {
            if (EpcDetails is null)
            {
                return EpcRating.Unknown;
            }

            if (EpcDetails.ExpiryDate < DateTime.Now)
            {
                return EpcRating.Expired;
            }

            return EpcDetails.EpcRating ?? EpcRating.Unknown;
        }
    }

    private EpcRating FoundEpcBand
    {
        get
        {
            if (EpcDetails is null)
            {
                return EpcRating.Unknown;
            }

            if (EpcDetails.ExpiryDate is not null && EpcDetails.ExpiryDate < DateTime.Now)
            {
                return EpcRating.Expired;
            }

            return EpcDetails.EpcRating ?? EpcRating.Unknown;
        }
    }

    public bool FoundEpcBandIsTooHigh =>
        FoundEpcBand is EpcRating.A or EpcRating.B or EpcRating.C;

    public bool EpcIsTooHigh => EffectiveEpcBand is EpcRating.A or EpcRating.B or EpcRating.C;

#pragma warning disable CS0618 // Obsolete Income Bands used to preserve backwards-compatibility
    public bool IncomeIsTooHigh =>
        IncomeBand is (Enums.IncomeBand.GreaterThan31000 or Enums.IncomeBand.GreaterThan34500
            or Enums.IncomeBand.GreaterThan36000) && IsImdPostcode is not true;
#pragma warning restore CS0618


    public bool IncomeBandIsValid =>
        CustodianCode is not null
        && IncomeBand is not null
        && LocalAuthorityData.LocalAuthorityDetailsByCustodianCode[CustodianCode].IncomeBandOptions
            .Contains(IncomeBand.Value);

    /**
     * This function is used as a check of last resort. We normally expect the routing to send to ineligible pages where needed
     * We check this at some key points in the flow to be sure that ineligible referrals aren't submitted
     * e.g. by the user editing the URL
     */
    public QuestionnaireStatus QuestionnaireStatus
    {
        get
        {
            List<RequiredQuestion> unansweredQuestions = [];

            // If the following questions were not answered, we should not let the user submit the form as we cannot determine their eligibility
            if (Country is null)
            {
                unansweredQuestions.Add(RequiredQuestion.Country);
            }

            if (OwnershipStatus is null)
            {
                unansweredQuestions.Add(RequiredQuestion.OwnershipStatus);
            }

            if (AddressLine1 is null)
            {
                unansweredQuestions.Add(RequiredQuestion.Address);
            }

            if (CustodianCode is null || LocalAuthorityConfirmed is null)
            {
                unansweredQuestions.Add(RequiredQuestion.LocalAuthority);
            }

            if (EpcIsTooHigh && EpcDetailsAreCorrect is null)
            {
                unansweredQuestions.Add(RequiredQuestion.Epc);
            }

            if (IncomeBand is null)
            {
                unansweredQuestions.Add(RequiredQuestion.Income);
            }

            if (unansweredQuestions.Count > 0)
            {
                return new IncompleteQuestionnaireStatus
                    { UnansweredQuestions = unansweredQuestions };
            }

            // Answers that mean the user is ineligible and should be returned to the relevant question page
            switch (Country)
            {
                case CountryEnum.Wales:
                    return new IneligibleQuestionnaireStatus { IneligibleFlowStep = QuestionFlowStep.IneligibleWales };
                case CountryEnum.Scotland:
                    return new IneligibleQuestionnaireStatus
                        { IneligibleFlowStep = QuestionFlowStep.IneligibleScotland };
                case CountryEnum.NorthernIreland:
                    return new IneligibleQuestionnaireStatus
                        { IneligibleFlowStep = QuestionFlowStep.IneligibleNorthernIreland };
            }

            if (OwnershipStatus is OwnershipStatusEnum.Landlord or OwnershipStatusEnum.PrivateTenancy)
            {
                return new IneligibleQuestionnaireStatus { IneligibleFlowStep = QuestionFlowStep.IneligibleTenure };
            }

            switch (LocalAuthorityStatus)
            {
                case LocalAuthorityData.LocalAuthorityStatus.NoFunding:
                    return new IneligibleQuestionnaireStatus { IneligibleFlowStep = QuestionFlowStep.NoFunding };
                case LocalAuthorityData.LocalAuthorityStatus.NotParticipating:
                    return new IneligibleQuestionnaireStatus { IneligibleFlowStep = QuestionFlowStep.NotParticipating };
                case LocalAuthorityData.LocalAuthorityStatus.NoLongerParticipating:
                    return new IneligibleQuestionnaireStatus
                        { IneligibleFlowStep = QuestionFlowStep.NoLongerParticipating };
                case LocalAuthorityData.LocalAuthorityStatus.ReferralsPaused:
                    return new IneligibleQuestionnaireStatus { IneligibleFlowStep = QuestionFlowStep.ReferralsPaused };
            }

            if (EpcIsTooHigh || IncomeIsTooHigh)
            {
                return new IneligibleQuestionnaireStatus { IneligibleFlowStep = QuestionFlowStep.Ineligible };
            }

            return new EligibleQuestionnaireStatus();
        }
    }

    public void CreateUneditedData()
    {
        UneditedData = new Questionnaire();
        CopyAnswersTo(UneditedData);
    }

    public void CommitEdits()
    {
        DeleteUneditedData();
    }

    public void RevertToUneditedData()
    {
        UneditedData.CopyAnswersTo(this);
        DeleteUneditedData();
    }

    private void DeleteUneditedData()
    {
        UneditedData = null;
    }

    public void CopyAnswersTo(Questionnaire other)
    {
        foreach (var propertyInfo in GetType().GetProperties())
        {
            if (propertyInfo.Name is nameof(UneditedData))
            {
                continue;
            }

            if (propertyInfo.CanWrite)
            {
                propertyInfo.SetValue(other, propertyInfo.GetValue(this));
            }
        }
    }
}

public abstract class QuestionnaireStatus;

public class IncompleteQuestionnaireStatus : QuestionnaireStatus
{
    public IEnumerable<RequiredQuestion> UnansweredQuestions { get; init; }
}

public class IneligibleQuestionnaireStatus : QuestionnaireStatus
{
    public QuestionFlowStep IneligibleFlowStep { get; init; }
}

public class EligibleQuestionnaireStatus : QuestionnaireStatus;

public enum RequiredQuestion
{
    Country,
    OwnershipStatus,
    Address,
    LocalAuthority,
    Epc,
    Income
}

public static class QuestionnaireStatusHelper
{
    public static string ToErrorMessage(this RequiredQuestion requiredQuestion)
    {
        return requiredQuestion switch
        {
            RequiredQuestion.Country => "Select which country the property is located in",
            RequiredQuestion.OwnershipStatus => "Select your ownership status of the property",
            RequiredQuestion.Address => "Select the address of your property",
            RequiredQuestion.LocalAuthority => "Select the Local Authority the property is located in",
            RequiredQuestion.Epc => "Select whether the EPC is correct for the property",
            RequiredQuestion.Income => "Select your household income",
            _ => throw new ArgumentOutOfRangeException(nameof(requiredQuestion))
        };
    }
}