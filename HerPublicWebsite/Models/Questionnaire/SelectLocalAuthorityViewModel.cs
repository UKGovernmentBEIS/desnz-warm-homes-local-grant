using System.Collections.Generic;
using System.Linq;
using GovUkDesignSystem.Attributes.ValidationAttributes;
using HerPublicWebsite.BusinessLogic.Models;

namespace HerPublicWebsite.Models.Questionnaire;

public class SelectLocalAuthorityViewModel : QuestionFlowViewModel
{
    public Dictionary<string, List<LocalAuthorityDetails>> LocalAuthoritiesByInitial => LocalAuthorityData
        .LocalAuthorityDetailsByCustodianCode
        .Where(kvp => string.IsNullOrEmpty(SearchTerm) || kvp.Value.Name.ToLower().Contains(SearchTerm.ToLower()))
        .ToList()
        .GroupBy(kvp => kvp.Value.Name[0].ToString().ToUpper())
        .ToDictionary(
            group => group.Key,
            group => group.Select(kvp => new LocalAuthorityDetails(kvp.Value.Name, kvp.Key)).ToList());
    public string SearchTerm { get; set; }
    
    public record class LocalAuthorityDetails(string Name, string CustodianCode);
}
