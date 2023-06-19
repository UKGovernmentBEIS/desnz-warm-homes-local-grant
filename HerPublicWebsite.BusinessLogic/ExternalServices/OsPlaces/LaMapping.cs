namespace HerPublicWebsite.BusinessLogic.ExternalServices.OsPlaces;

// Sometimes, local authorities merge and we need to map the old custodian codes
public class LaMapping
{
    public IEnumerable<string> OldCustodianCodes { get; }
    public string NewCustodianCode { get; }

    public LaMapping(IEnumerable<string> oldCustodianCodes, string newCustodianCode)
    {
        OldCustodianCodes = oldCustodianCodes;
        NewCustodianCode = newCustodianCode;
    }
}
