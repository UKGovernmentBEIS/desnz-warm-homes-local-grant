using System.Collections.Generic;
using System.Linq;
using HerPublicWebsite.BusinessLogic.Models;

namespace HerPublicWebsite.Models.Questionnaire;

public class SelectLocalAuthorityViewModel : QuestionFlowViewModel
{
    public Dictionary<string, List<LocalAuthorityDetails>> LocalAuthoritiesByInitial { get; set; }
    public string SearchTerm { get; set; }

    public SelectLocalAuthorityViewModel(string filter)
    {
        LocalAuthoritiesByInitial = LocalAuthorityData
            .LocalAuthorityDetailsByCustodianCode
            .Where(kvp => kvp.Value.Name.Contains(filter))
            .ToList()
            .GroupBy(kvp => kvp.Value.Name[0].ToString().ToUpper())
            .ToDictionary(
                group => group.Key,
                group => group.Select(kvp => new LocalAuthorityDetails(kvp.Value.Name, kvp.Key)).ToList());
    }

    public SelectLocalAuthorityViewModel() : this("") { }
    
    public record class LocalAuthorityDetails(string Name, string CustodianCode);
}
