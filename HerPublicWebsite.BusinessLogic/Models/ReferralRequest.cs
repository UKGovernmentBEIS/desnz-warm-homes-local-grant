using HerPublicWebsite.BusinessLogic.Models.Enums;

namespace HerPublicWebsite.BusinessLogic.Models;

public class ReferralRequest
{
    public int Id { get; set; }
    public string AddressLine1 { get; set; }
    public string AddressLine2 { get; set; }
    public string AddressLine3 { get; set; }
    public string AddressTown { get; set; }
    public string AddressPostcode { get; set; }
    
    public int CustodianCode { get; set; }
    
    public string Uprn { get; set; } // Should be populated for most questionnaires, but not 100% guaranteed

    public EpcRating EpcRating { get; set; } = EpcRating.Unknown;
    public bool IsLsoaProperty { get; set; }
    public HasGasBoiler HasGasBoiler { get; set; } = HasGasBoiler.Unknown;
    public IncomeBand IncomeBand { get; set; }
    
    public DateTime RequestDate { get; set; }

    public bool ReferralCreated { get; set; } = false;
    
    public ContactDetails ContactDetails { get; set; }

    public ReferralRequest()
    {
    }

    public ReferralRequest(Questionnaire questionnaire)
    {
        AddressLine1 = questionnaire.AddressLine1;
        AddressLine2 = questionnaire.AddressLine2;
        AddressLine3 = questionnaire.AddressLine3;
        AddressTown = questionnaire.AddressTown;
        AddressPostcode = questionnaire.AddressPostcode;
        CustodianCode = questionnaire.CustodianCode;
        Uprn = questionnaire.Uprn;
        EpcRating = questionnaire.EpcRating;
        IsLsoaProperty = questionnaire.IsLsoaProperty;
        HasGasBoiler = questionnaire.HasGasBoiler;
        IncomeBand = questionnaire.IncomeBand;
        ContactDetails = questionnaire.ContactDetails;
    }
}