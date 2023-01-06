using GovUkDesignSystem.Attributes.ValidationAttributes;
using SeaPublicWebsite.BusinessLogic.Models;
using SeaPublicWebsite.BusinessLogic.Models.Enums;

namespace SeaPublicWebsite.Models.EnergyEfficiency
{
    public class ConfirmSingleAddressViewModel : QuestionFlowViewModel
    {
        public EpcSearchResult EpcSearchResult { get; set; }
        public string Reference { get; set; }
        [GovUkValidateRequired(ErrorMessageIfMissing = "Confirm whether or not this certificate belongs to your address before continuing")]
        public EpcAddressConfirmed? EpcAddressConfirmed { get; set; }
        public string Postcode { get; set; }
        public string Number { get; set; }
        public string EpcId { get; set; }
    }
}