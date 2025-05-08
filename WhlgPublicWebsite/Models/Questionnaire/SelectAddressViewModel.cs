using System.Collections.Generic;
using GovUkDesignSystem.Attributes.ValidationAttributes;
using GovUkDesignSystem.ModelBinders;
using Microsoft.AspNetCore.Mvc;
using WhlgPublicWebsite.BusinessLogic.Models;
using WhlgPublicWebsite.Models.Enums;

namespace WhlgPublicWebsite.Models.Questionnaire;

public class SelectAddressViewModel : QuestionFlowViewModel
{
    public List<Address> Addresses { get; set; }
    [GovUkValidateRequiredIf(IsRequiredPropertyName = nameof(IsMultipleAddresses), ErrorMessageIfMissing = "Select your address or click \"Address not listed above\"")]
    [ModelBinder(typeof(GovUkSelectBinder))] // ensure the placeholder is interpreted as null
    public string SelectedAddressIndex { get; set; }

    [GovUkValidateRequiredIf(IsRequiredPropertyName = nameof(IsSingleAddress), ErrorMessageIfMissing = "Select yes if it is your address")]
    public YesOrNo? IsAddressCorrect { get; set; }

    public bool IsSingleAddress { get; set; }

    public bool IsMultipleAddresses { get; set; }

    public string AddressInputPlaceholder => Addresses is not null
        ? $"{Addresses.Count} {(Addresses.Count == 1 ? "Address" : "Addresses")} found"
        : string.Empty;
}
