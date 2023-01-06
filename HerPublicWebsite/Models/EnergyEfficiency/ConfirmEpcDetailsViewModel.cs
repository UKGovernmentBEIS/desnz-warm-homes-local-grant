using GovUkDesignSystem.Attributes.ValidationAttributes;
using HerPublicWebsite.BusinessLogic.Models;
using HerPublicWebsite.BusinessLogic.Models.Enums;

namespace HerPublicWebsite.Models.EnergyEfficiency
{
    public class ConfirmEpcDetailsViewModel : QuestionFlowViewModel
    {
        public string SelectedEpcId { get; set; }
        
        public Epc Epc { get; set; }
        public string Reference { get; set; }
        [GovUkValidateRequired(ErrorMessageIfMissing = "Confirm whether or not this certificate belongs to your address before continuing")]
        public EpcDetailsConfirmed? EpcDetailsConfirmed { get; set; }
    }
}