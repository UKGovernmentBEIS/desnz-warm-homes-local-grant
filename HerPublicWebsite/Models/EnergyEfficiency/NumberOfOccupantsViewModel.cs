using GovUkDesignSystem.Attributes.DataBinding;
using GovUkDesignSystem.Attributes.ValidationAttributes;
using GovUkDesignSystem.ModelBinders;
using Microsoft.AspNetCore.Mvc;

namespace HerPublicWebsite.Models.EnergyEfficiency
{
    public class NumberOfOccupantsViewModel : QuestionFlowViewModel
    {
        [ModelBinder(typeof(GovUkMandatoryIntBinder))]
        [GovUkDataBindingMandatoryIntErrorText("Enter the number of people who live in the property", "The number of people who live in the property")]
        [GovUkValidateIntRange("The number of people who live in the property", 1, 9)]
        public int? NumberOfOccupants { get; set; }

        public string Reference { get; set; }
    }
}