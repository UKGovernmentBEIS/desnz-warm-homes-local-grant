using System.Collections.Generic;
using GovUkDesignSystem.Attributes.ValidationAttributes;
using Microsoft.AspNetCore.Mvc;
using HerPublicWebsite.BusinessLogic.Models;

namespace HerPublicWebsite.Models.Questionnaire;

public class SelectAddressViewModel : QuestionFlowViewModel
{
    public List<Address> Addresses {get; set;}
    [GovUkValidateRequired(ErrorMessageIfMissing = "You must select an address")]
    public string SelectedAddressIndex {get; set;}
}
