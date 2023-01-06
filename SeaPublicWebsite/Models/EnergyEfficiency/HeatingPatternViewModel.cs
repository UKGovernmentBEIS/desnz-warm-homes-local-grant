using GovUkDesignSystem.Attributes.DataBinding;
using GovUkDesignSystem.Attributes.ValidationAttributes;
using GovUkDesignSystem.ModelBinders;
using Microsoft.AspNetCore.Mvc;
using SeaPublicWebsite.BusinessLogic.Models.Enums;

namespace SeaPublicWebsite.Models.EnergyEfficiency
{
    public class HeatingPatternViewModel : QuestionFlowViewModel
    {
        [GovUkValidateRequired(ErrorMessageIfMissing = "Select heating pattern")]
        public HeatingPattern? HeatingPattern { get; set; }
        
        [ModelBinder(typeof(GovUkOptionalIntBinder))]
        [GovUkDataBindingOptionalIntErrorText("Number of hours in the morning")]
        [GovUkValidateRequiredIf(ErrorMessageIfMissing = "Enter number of hours in the morning", IsRequiredPropertyName = nameof(IsRequiredHoursOfHeating))]
        [GovUkValidateIntRange("Number of hours", 0, 12)]
        public int? HoursOfHeatingMorning { get; set; }
        
        [ModelBinder(typeof(GovUkOptionalIntBinder))]
        [GovUkDataBindingOptionalIntErrorText("Number of hours in the afternoon and evening")]
        [GovUkValidateRequiredIf(ErrorMessageIfMissing = "Enter number of hours in the afternoon and evening", IsRequiredPropertyName = nameof(IsRequiredHoursOfHeating))]
        [GovUkValidateIntRange("Number of hours", 0, 12)]
        public int? HoursOfHeatingEvening { get; set; }

        public string Reference { get; set; }

        public bool IsRequiredHoursOfHeating => HeatingPattern == BusinessLogic.Models.Enums.HeatingPattern.Other;
    }
}
