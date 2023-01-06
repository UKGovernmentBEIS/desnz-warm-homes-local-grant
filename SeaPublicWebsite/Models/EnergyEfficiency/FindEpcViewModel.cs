using GovUkDesignSystem.Attributes.ValidationAttributes;
using SeaPublicWebsite.BusinessLogic.Models.Enums;

namespace SeaPublicWebsite.Models.EnergyEfficiency;

public class FindEpcViewModel : QuestionFlowViewModel
{
    [GovUkValidateRequired(ErrorMessageIfMissing =
        "Confirm whether or not you would like to search for your EPC before continuing")]
    public SearchForEpc? FindEpc { get; set; }

    public string Reference { get; set; }
}