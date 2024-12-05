using System.Collections.Generic;
using System.Linq;
using System.Net;
using GovUkDesignSystem.Attributes.ValidationAttributes;
using WhlgPublicWebsite.Extensions;
using WhlgPublicWebsite.Models.Enums;
using WhlgPublicWebsite.BusinessLogic.Models;
using WhlgPublicWebsite.BusinessLogic.Models.Enums;

namespace WhlgPublicWebsite.Models.Questionnaire;

public class ReviewEpcViewModel : QuestionFlowViewModel
{
    [GovUkValidateRequired(ErrorMessageIfMissing = "Select whether this EPC is correct for your property")]
    public EpcConfirmation? EpcIsCorrect {get; set;}
    
    public string AddressRawHtml { get; set; }
    public string EpcBand { get; set; }
    public string ValidFrom { get; set; }
    public string ValidUntil { get; set; }
    public string PropertyType { get; set; }

    public ReviewEpcViewModel() {}
    
    public ReviewEpcViewModel(EpcDetails epcDetails, EpcConfirmation? epcIsCorrect)
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
        ValidFrom = epcDetails.LodgementDate is not null ? epcDetails.LodgementDate.Value.ToString("dd MMMM yyyy") : "Unknown";
        ValidUntil = epcDetails.ExpiryDate is not null ? epcDetails.ExpiryDate.Value.ToString("dd MMMM yyyy") : "Unknown";

        var propertyDetails = epcDetails.HouseType?.ForDisplay() ??
                              epcDetails.BungalowType?.ForDisplay() ??
                              epcDetails.FlatType?.ForDisplay() ??
                              "";
        var propertyType = epcDetails.PropertyType?.ForDisplay() ?? "Unknown";
        PropertyType = $"{propertyDetails} {propertyType}";

        EpcIsCorrect = epcIsCorrect;
    }
}
