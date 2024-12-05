using System.Collections.Generic;
using GovUkDesignSystem.Attributes.ValidationAttributes;
using Microsoft.AspNetCore.Mvc;
using WhlgPublicWebsite.BusinessLogic.Models;
using WhlgPublicWebsite.Models.Enums;

namespace WhlgPublicWebsite.Models.Questionnaire;

public class SelectAddressViewModel : QuestionFlowViewModel
{
    public List<Address> Addresses { get; set; }
    [GovUkValidateRequiredIf(IsRequiredPropertyName = nameof(IsMultipleAddresses), ErrorMessageIfMissing = "Select your address or click \"Address not listed above\"")]
    public string SelectedAddressIndex { get; set; }

    [GovUkValidateRequiredIf(IsRequiredPropertyName = nameof(IsSingleAddress), ErrorMessageIfMissing = "Select yes if it is your address")]
    public YesOrNo? IsAddressCorrect { get; set; }

    public bool IsSingleAddress { get; set; }

    public bool IsMultipleAddresses { get; set; }

}
