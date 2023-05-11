using System.Collections.Generic;
using GovUkDesignSystem.Attributes.ValidationAttributes;
using Microsoft.AspNetCore.Mvc;
using HerPublicWebsite.BusinessLogic.Models;

namespace HerPublicWebsite.Models.Questionnaire;

public class SelectAddressViewModel : QuestionFlowViewModel
{
    public List<Address> Addresses {get; set;}
    [GovUkValidateRequired(ErrorMessageIfMissing = "Select your address or click \"Address not listed above\"")]
    public string SelectedAddressIndex {get; set;}
}
