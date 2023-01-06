using GovUkDesignSystem.Attributes.DataBinding;
using GovUkDesignSystem.Attributes.ValidationAttributes;
using GovUkDesignSystem.ModelBinders;
using Microsoft.AspNetCore.Mvc;

namespace HerPublicWebsite.Models.EnergyEfficiency
{
    public class TemperatureViewModel : QuestionFlowViewModel
    {
        [ModelBinder(typeof(GovUkMandatoryDecimalBinder))]
        [GovUkDataBindingMandatoryDecimalErrorText("Enter a number between 5 and 35, or skip this question", "The temperature")]
        [GovUkValidateDecimalRange("The temperature", 5, 35)]
        public decimal? Temperature { get; set; }
        public string Reference { get; set; }
    }
}