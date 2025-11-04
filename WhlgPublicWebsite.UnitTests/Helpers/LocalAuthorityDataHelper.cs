using System.Linq;
using WhlgPublicWebsite.BusinessLogic.Models;

namespace Tests.Helpers;

public static class LocalAuthorityDataHelper
{
    /// <summary>
    /// Finds first LA regardless of consortium with the given status and returns its custodian code
    /// </summary>
    public static string GetExampleCustodianCodeForStatus(LocalAuthorityData.LocalAuthorityStatus status)
    {
        return LocalAuthorityData.LocalAuthorityDetailsByCustodianCode
            .First(entry => entry.Value.Status == status)
            .Key;
    }

    /// <summary>
    /// Finds first LA with the given status and consortium and returns its custodian code
    /// </summary>
    /// add a cref 
    /// <param name="consortiumName">Consortium name or null for no consortium. See <see cref="ConsortiumNames"/>.</param>
    public static string GetExampleCustodianCodeForStatusAndConsortium(LocalAuthorityData.LocalAuthorityStatus status,
        string consortiumName)
    {
        return LocalAuthorityData.LocalAuthorityDetailsByCustodianCode
            .First(entry => entry.Value.Status == status && entry.Value.Consortium == consortiumName)
            .Key;
    }
}