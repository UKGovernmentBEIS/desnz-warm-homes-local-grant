using HerPublicWebsite.BusinessLogic.Models.Enums;
using IncomeBandEnum = HerPublicWebsite.BusinessLogic.Models.Enums.IncomeBand;
using HasGasBoilerEnum = HerPublicWebsite.BusinessLogic.Models.Enums.HasGasBoiler;
using CountryEnum = HerPublicWebsite.BusinessLogic.Models.Enums.Country;
using OwnershipStatusEnum = HerPublicWebsite.BusinessLogic.Models.Enums.OwnershipStatus;

namespace HerPublicWebsite.BusinessLogic.Models;

public record Questionnaire
{
    public CountryEnum? Country { get; set; }
    public OwnershipStatusEnum? OwnershipStatus { get; set; }

    public string AddressLine1 { get; set; }
    public string AddressLine2 { get; set; }
    public string AddressTown { get; set; }
    public string AddressCounty { get; set; }
    public string AddressPostcode { get; set; }

    public string CustodianCode { get; set; }
    public bool? LocalAuthorityConfirmed { get; set; }

    public string Uprn { get; set; } // Should be populated for most questionnaires, but not 100% guaranteed

    public EpcDetails EpcDetails { get; set; }
    public bool? EpcDetailsAreCorrect { get; set; }
    public bool? IsLsoaProperty { get; set; }
    public HasGasBoilerEnum? HasGasBoiler { get; set; }
    public IncomeBandEnum? IncomeBand { get; set; }

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

    public bool IsEligibleForHug2 =>
            (IncomeIsTooHigh, HasGasBoiler, EpcIsTooHigh, Country, OwnershipStatus) is
                (false, not HasGasBoilerEnum.Yes, false, CountryEnum.England, OwnershipStatusEnum.OwnerOccupancy);

    public bool FoundEpcBandIsTooHigh =>
        EpcDetails is { EpcRating: EpcRating.A or EpcRating.B or EpcRating.C};

    public string LocalAuthorityName
    {
        get
        {
            if (string.IsNullOrEmpty(CustodianCode) || !LocalAuthorityData.LocalAuthorityDetailsByCustodianCode.ContainsKey(CustodianCode))
            {
                return "unrecognised local authority";
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
                return "unrecognised local authority";
            }

            return LocalAuthorityData.LocalAuthorityDetailsByCustodianCode[CustodianCode].WebsiteUrl;
        }
    }

    public LocalAuthorityData.Hug2Status? LocalAuthorityHug2Status
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

            if (EpcDetailsAreCorrect is not true)
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

    public bool EpcIsExpired => EpcDetails.ExpiryDate < DateTime.Now;

    public bool EpcIsTooHigh => EffectiveEpcBand is EpcRating.A or EpcRating.B or EpcRating.C;

    public bool IncomeIsTooHigh => IncomeBand is IncomeBandEnum.GreaterThan31000 && IsLsoaProperty is not true;
}
