using WhlgPublicWebsite.BusinessLogic.Extensions;
using WhlgPublicWebsite.BusinessLogic.Models.Enums;

namespace WhlgPublicWebsite.BusinessLogic.Models;

public class AnonymisedReport
{
    public DateTime? SubmissionDate { get; set; }
    public string PostcodeFirstHalf { get; set; }
    public bool IsLsoaProperty { get; set; }
    public EpcRating EpcRating { get; set; }
    public DateTime? EpcLodgementDate { get; set; }
    public bool IsEligible { get; set; }
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
        IncomeBand = questionnaire.IncomeBand!.Value;
        IsEligible = questionnaire.IsEligibleForWhlg;
        OwnershipStatus = questionnaire.OwnershipStatus!.Value;
    }
}
