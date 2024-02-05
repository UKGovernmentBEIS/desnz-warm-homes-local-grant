using HerPublicWebsite.BusinessLogic.Models.Enums;

namespace HerPublicWebsite.BusinessLogic.Models;

public class ReferralRequest
{
    public int Id { get; set; }
    public string AddressLine1 { get; set; }
    public string AddressLine2 { get; set; }
    public string AddressTown { get; set; }
    public string AddressCounty { get; set; }
    public string AddressPostcode { get; set; }

    public string CustodianCode { get; set; }

    public string Uprn { get; set; } // Should be populated for most questionnaires, but not 100% guaranteed

    public EpcRating EpcRating { get; set; } = EpcRating.Unknown;

    public EpcConfirmation? EpcConfirmation { get; set; }

    public DateTime? EpcLodgementDate { get; set; }
    public bool IsLsoaProperty { get; set; }
    public HasGasBoiler HasGasBoiler { get; set; }
    public IncomeBand IncomeBand { get; set; }
    
    public string FullName { get; set; }
    public string ContactEmailAddress { get; set; }
    public string ContactTelephone { get; set; }

    public DateTime RequestDate { get; set; }

    public bool ReferralWrittenToCsv { get; set; }
    
    public bool FollowUpEmailSent { get; set; }
    
    public ReferralRequestFollowUp? FollowUp {get; set;}
    
    public string ReferralCode { get; set; }
    
    public ReferralRequest()
    {
    }

    public ReferralRequest(Questionnaire questionnaire)
    {
        AddressLine1 = questionnaire.AddressLine1;
        AddressLine2 = questionnaire.AddressLine2;
        AddressTown = questionnaire.AddressTown;
        AddressCounty = questionnaire.AddressCounty;
        AddressPostcode = questionnaire.AddressPostcode;
        CustodianCode = questionnaire.CustodianCode;
        Uprn = questionnaire.Uprn;
        EpcRating = questionnaire.DisplayEpcRating;
        EpcConfirmation = questionnaire.EpcDetailsAreCorrect;
        EpcLodgementDate = questionnaire.EpcDetails?.LodgementDate;
        IsLsoaProperty = questionnaire.IsLsoaProperty!.Value;
        HasGasBoiler = questionnaire.HasGasBoiler!.Value;
        IncomeBand = questionnaire.IncomeBand!.Value;
        FullName = questionnaire.LaContactName;
        ContactEmailAddress = questionnaire.LaContactEmailAddress;
        ContactTelephone = questionnaire.LaContactTelephone;
        RequestDate = DateTime.Now;
    }

    public void UpdateReferralCode()
    {
        if (Id == 0)
        {
            throw new InvalidOperationException("Cannot generate referral code until referral request has a unique ID");
        }

        ReferralCode = $"HUG2{Id:D7}";
    }
}
