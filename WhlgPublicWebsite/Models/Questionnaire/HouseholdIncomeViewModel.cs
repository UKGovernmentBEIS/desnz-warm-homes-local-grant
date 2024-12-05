using System.Collections.Generic;
using GovUkDesignSystem.Attributes.ValidationAttributes;
using HerPublicWebsite.BusinessLogic.Models;
using HerPublicWebsite.BusinessLogic.Models.Enums;

namespace HerPublicWebsite.Models.Questionnaire;

public class HouseholdIncomeViewModel : QuestionFlowViewModel
{
    [GovUkValidateRequired(ErrorMessageIfMissing =
        "Select if your household income is £36,000 or less, or more than £36,000")]
    public IncomeBand? IncomeBand { get; set; }

    public string CustodianCode { get; set; }

    public IEnumerable<IncomeBand> IncomeBandOptions => CustodianCode is null
        ? null
        : LocalAuthorityData.LocalAuthorityDetailsByCustodianCode[CustodianCode].IncomeBandOptions;
}