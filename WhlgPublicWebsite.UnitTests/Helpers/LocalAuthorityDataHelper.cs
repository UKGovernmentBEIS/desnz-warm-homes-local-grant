using System.Linq;
using WhlgPublicWebsite.BusinessLogic.Models;

namespace Tests.Helpers;

public static class LocalAuthorityDataHelper
{
    public static string GetExampleCustodianCodeForStatus(LocalAuthorityData.Hug2Status status)
    {
        return LocalAuthorityData.LocalAuthorityDetailsByCustodianCode
            .First(entry => entry.Value.Status == status)
            .Key;
    }
}