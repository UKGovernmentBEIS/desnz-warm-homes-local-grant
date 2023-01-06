using GovUkDesignSystem.Attributes.ValidationAttributes;
using GovUkDesignSystem.ModelBinders;
using Microsoft.AspNetCore.Mvc;

namespace SeaPublicWebsite.Models.EnergyEfficiency
{
    public class YourSavedRecommendationsEmailViewModel : QuestionFlowViewModel
    {
        public string Reference { get; set; }
        [GovUkValidateRequired(ErrorMessageIfMissing = "Enter a valid email address")]
        public string EmailAddress { get; set; }
        public bool EmailSent { get; set; }
        public string PostAction { get; set; }
    }
}