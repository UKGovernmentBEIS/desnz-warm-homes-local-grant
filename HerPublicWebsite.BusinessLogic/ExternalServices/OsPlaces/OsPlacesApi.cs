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
                Postcode = "LB6 420",
                County = "A County",
                Town = "Camdoon",
                Uprn = "023232323",
                LocalCustodianCode = "2323",
            },
            new ()
            {
                DisplayAddress = "88 My House, My Area, A Special Location, LB6 420",
                AddressLine1 = "88 My House",
                AddressLine2 = "My Area",
                Postcode = "LB6 420",
                County = "A County",
                Town = "Camdoon",
                LocalCustodianCode = "2323",
            }
        };
    }
}
