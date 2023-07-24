using HerPublicWebsite.BusinessLogic.Models.Enums;
using HerPublicWebsite.BusinessLogic.Extensions;

namespace HerPublicWebsite.BusinessLogic.Models;

public class AnonymisedReport
{
    public DateTime? SubmissionDate { get; set; }
    public string PostcodeFirstHalf { get; set; }
    public bool IsLsoaProperty { get; set; }
    public EpcRating EpcRating { get; set; }
    public DateTime? EpcLodgementDate { get; set; }
    public bool IsEligible { get; set; }
    public HasGasBoiler HasGasBoiler { get; set; }
    public IncomeBand IncomeBand { get; set; }
    public OwnershipStatus OwnershipStatus { get; set; }

    public AnonymisedReport()
    {
    }

    public AnonymisedReport(Questionnaire questionnaire)
    {
        SubmissionDate = DateTime.Now;
        PostcodeFirstHalf = questionnaire.AddressPostcode.NormaliseToUkPostcodeFormat()?.Split(" ")?.FirstOrDefault();
        IsLsoaProperty = questionnaire.IsLsoaProperty!.Value;
        EpcRating = questionnaire.EffectiveEpcBand;
        EpcLodgementDate = questionnaire.EpcDetails?.LodgementDate;
        HasGasBoiler = questionnaire.HasGasBoiler!.Value;
        IncomeBand = questionnaire.IncomeBand!.Value;
        IsEligible = questionnaire.IsEligibleForHug2;
        OwnershipStatus = questionnaire.OwnershipStatus!.Value;
    }
}
