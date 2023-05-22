using HerPublicWebsite.BusinessLogic.Models;

namespace HerPublicWebsite.BusinessLogic.ExternalServices.OsPlaces;

public class OsPlacesApi : IOsPlacesApi
{
    public async Task<List<Address>> GetAddressesAsync(string postcode, string buildingNumber)
    {
        //TODO BEISHER-248: Actually implement API here
        return new List<Address> {
            new () {
                DisplayAddress = "82 My House, My Area, A Special Location, LB6 420",
                AddressLine1 = "82 My House",
                AddressLine2 = "My Area",
                Postcode = "NW5 1TL",
                County = "A County",
                Town = "Camdoon",
                Uprn = "000038153332",
                LocalCustodianCode = "2323",
            },
            new ()
            {
                DisplayAddress = "88 My House, My Area, A Special Location, LB6 420",
                AddressLine1 = "88 My House",
                AddressLine2 = "My Area",
                Postcode = "PL13 2FN",
                County = "A County",
                Town = "Camdoon",
                Uprn = "100070672776",
                LocalCustodianCode = "2323",
            }
        };
    }
}
