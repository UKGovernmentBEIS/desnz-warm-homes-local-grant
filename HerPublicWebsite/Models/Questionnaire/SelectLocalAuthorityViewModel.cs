using System.Collections.Generic;
using System.Linq;
using GovUkDesignSystem.Attributes.ValidationAttributes;
using Microsoft.AspNetCore.Mvc;
using HerPublicWebsite.BusinessLogic.Models;

namespace HerPublicWebsite.Models.Questionnaire;

public class SelectLocalAuthorityViewModel : QuestionFlowViewModel
{
    public Dictionary<string, List<LocalAuthorityDetails>> LocalAuthoritiesByInitial { get; set; }

    public SelectLocalAuthorityViewModel()
    {
        LocalAuthoritiesByInitial = LocalAuthorityData
            .LocalAuthorityDetailsByCustodianCode
            .ToList()
            .GroupBy(kvp => kvp.Value.Name[0].ToString().ToUpper())
            .ToDictionary(
                group => group.Key,
                group => group.Select(kvp => new LocalAuthorityDetails(kvp.Value.Name, kvp.Key)).ToList());
    }
    
    public record class LocalAuthorityDetails(string Name, string CustodianCode);
}
