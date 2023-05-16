using System.Collections.Generic;
using System.Linq;
using System.Net;
using GovUkDesignSystem.Attributes.ValidationAttributes;
using HerPublicWebsite.BusinessLogic.Models;
using HerPublicWebsite.Extensions;

namespace HerPublicWebsite.Models.Questionnaire;

public class ReviewEpcViewModel : QuestionFlowViewModel
{
    public enum YesOrNo
    {
        Yes,
        No
    }

    [GovUkValidateRequired(ErrorMessageIfMissing = "Select whether this EPC is correct for your property")]
    public YesOrNo? EpcIsCorrect {get; set;}
    
    public string AddressRawHtml { get; set; }
    public string EpcBand { get; set; }
    public string ValidFrom { get; set; }
    public string ValidUntil { get; set; }
    public string PropertyType { get; set; }

    public ReviewEpcViewModel() {}
    
    public ReviewEpcViewModel(EpcDetails epcDetails, bool? epcIsCorrect)
    {
        var addressParts = new List<string>
            {
                epcDetails.AddressLine1,
                epcDetails.AddressLine2,
                epcDetails.AddressLine3,
                epcDetails.AddressLine4,
                epcDetails.AddressTown,
                epcDetails.AddressPostcode
            }
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .Select(WebUtility.HtmlEncode);

        AddressRawHtml = string.Join("<br/>", addressParts);
        EpcBand = epcDetails.EpcRating.ToString();
        ValidFrom = epcDetails.LodgementDate != null ? epcDetails.LodgementDate.Value.ToString("dd MMMM yyyy") : "Unknown";
        ValidUntil = epcDetails.ExpiryDate != null ? epcDetails.ExpiryDate.Value.ToString("dd MMMM yyyy") : "Unknown";

        PropertyType =
            (epcDetails.HouseType?.ForDisplay() ??
             epcDetails.BungalowType?.ForDisplay() ??
             epcDetails.FlatType?.ForDisplay() ??
             "") + " " + (epcDetails.PropertyType?.ForDisplay() ?? "Unknown");

        EpcIsCorrect = epcIsCorrect == null ? null : (epcIsCorrect.Value ? YesOrNo.Yes : YesOrNo.No);
    }
}
