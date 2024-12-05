namespace WhlgPublicWebsite.BusinessLogic.ExternalServices.OsPlaces;

// Sometimes, local authorities merge and we need to map the old custodian codes
public class LaMapping
{
    public static string GetCurrentCustodianCode(string custodianCode)
    {
        return LaMerges
            .SingleOrDefault(lm => lm.OldCustodianCodes.Contains(custodianCode))
            ?.NewCustodianCode ?? custodianCode;
    }
    
    private class MappingDetails
    {
        public IEnumerable<string> OldCustodianCodes { get; }
        public string NewCustodianCode { get; }

        public MappingDetails(IEnumerable<string> oldCustodianCodes, string newCustodianCode)
        {
            OldCustodianCodes = oldCustodianCodes;
            NewCustodianCode = newCustodianCode;
        }
    }
    
    // The list of custodian code mappings
    private static readonly List<MappingDetails> LaMerges = new()
    {
        // Map:
        //   405 - Aylesbury Vale
        //   410 - South Bucks
        //   415 - Chiltern
        //   425 - Wycombe
        // to 440 - Buckinghamshire
        new MappingDetails(new[] { "405", "410", "415", "425" }, "440"),
        // Map:
        //   905 - Allerdale
        //   915 - Carlisle
        //   920 - Copeland
        // to 940 - Cumberland
        new MappingDetails(new[] { "905", "915", "920" }, "940"),
        // Map:
        //   910 - Barrow-in-Furness
        //   925 - Eden
        //   930 - South Lakeland
        // to 935 - Westmorland and Furness
        new MappingDetails(new[] { "910", "925", "930" }, "935"),
        // Map:
        //   2705 - Craven
        //   2710 - Hambleton
        //   2715 - Harrogate
        //   2720 - Richmondshire
        //   2725 - Ryedale
        //   2730 - Scarborough
        //   2735 - Selby
        // to 2745 - North Yorkshire
        new MappingDetails(new[] { "2705", "2710", "2715", "2720", "2725", "2730", "2735" }, "2745"),
        // Map:
        //   3305 - Mendip
        //   3310 - Sedgemoor
        //   3325 - South Somerset
        //   3330 - Somerset West and Taunton
        // to 3300 - Somerset
        new MappingDetails(new[] { "3305", "3310", "3325", "3330" }, "3300"),
    };
}
