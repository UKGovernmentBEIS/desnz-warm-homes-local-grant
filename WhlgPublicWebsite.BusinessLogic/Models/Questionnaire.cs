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

    public string AddressLine1 { get; set; }
    public string AddressLine2 { get; set; }
    public string AddressTown { get; set; }
    public string AddressCounty { get; set; }
    public string AddressPostcode { get; set; }

    public string CustodianCode { get; set; }
    public bool? LocalAuthorityConfirmed { get; set; }

    public string Uprn { get; set; } // Should be populated for most questionnaires, but not 100% guaranteed

    public EpcDetails EpcDetails { get; set; }
    public EpcConfirmation? EpcDetailsAreCorrect { get; set; }
    public bool? IsLsoaProperty { get; set; }
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

    public bool IsEligibleForWhlg =>
        (IncomeIsTooHigh, EpcIsTooHigh, Country, OwnershipStatus) is
        (false, false, Enums.Country.England, Enums.OwnershipStatus.OwnerOccupancy);

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
            or Enums.IncomeBand.GreaterThan36000) && IsLsoaProperty is not true;
#pragma warning restore CS0618


    public bool IncomeBandIsValid =>
        CustodianCode is not null
        && IncomeBand is not null
        && LocalAuthorityData.LocalAuthorityDetailsByCustodianCode[CustodianCode].IncomeBandOptions
            .Contains(IncomeBand.Value);

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