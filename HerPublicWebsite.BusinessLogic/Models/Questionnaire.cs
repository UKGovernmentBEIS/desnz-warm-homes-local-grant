using HerPublicWebsite.BusinessLogic.Models.Enums;

namespace HerPublicWebsite.BusinessLogic.Models;

public record Questionnaire
{
    public Country? Country { get; set; }
    public OwnershipStatus? OwnershipStatus { get; set; }

    public string AddressLine1 { get; set; }
    public string AddressLine2 { get; set; }
    public string AddressTown { get; set; }
    public string AddressCounty { get; set; }
    public string AddressPostcode { get; set; }
    
    public string CustodianCode { get; set; }
    
    public string Uprn { get; set; } // Should be populated for most questionnaires, but not 100% guaranteed

    public EpcDetails EpcDetails { get; set; }
    public bool? EpcDetailsAreCorrect { get; set; }
    public bool? IsLsoaProperty { get; set; }
    public HasGasBoiler? HasGasBoiler { get; set; }
    public IncomeBand? IncomeBand { get; set; }

    public DateTime ReferralCreated { get; set; }

    public bool IsEligibleForHug2 { get; set; }
    public string Hug2ReferralId { get; set; }

    public ContactDetails ContactDetails { get; set; }

    public bool EpcTooHigh => 
        EpcDetails is { EpcRating: EpcRating.A or EpcRating.B or EpcRating.C };
}
