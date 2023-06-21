using HerPublicWebsite.BusinessLogic.Models.Enums;

using HerPublicWebsite.BusinessLogic.Extensions;

namespace HerPublicWebsite.BusinessLogic.Models;

public class AnonymisedReport
{
    public string PostcodeFirstHalf { get; set; }
    public bool IsLsoaProperty { get; set; }
    public EpcRating EpcRating { get; set; }
    public DateTime? EpcLodgementDate { get; set; }
    public bool IsEligible { get; set; }
    public HasGasBoiler HasGasBoiler { get; set; }
    public IncomeBand IncomeBand { get; set; }
    public string CustodianCode { get; set; }
    public OwnershipStatus OwnershipStatus { get; set; }

    public AnonymisedReport()
    {
    }

    public AnonymisedReport(Questionnaire questionnaire)
    {
        PostcodeFirstHalf = questionnaire.AddressPostcode.NormaliseToUkPostcodeFormat().Split(" ").FirstOrDefault();
        IsLsoaProperty = questionnaire.IsLsoaProperty!.Value;
        EpcRating = questionnaire.EffectiveEpcBand;
        EpcLodgementDate = questionnaire.EpcDetails?.LodgementDate;
        HasGasBoiler = questionnaire.HasGasBoiler!.Value;
        IncomeBand = questionnaire.IncomeBand!.Value;
        IsEligible = questionnaire.IsEligibleForHug2;
        CustodianCode = questionnaire.CustodianCode;
        OwnershipStatus = questionnaire.OwnershipStatus!.Value;
    }
}
