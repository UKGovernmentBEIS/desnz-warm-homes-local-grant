using WhlgPublicWebsite.BusinessLogic.Models;

namespace WhlgPublicWebsite.BusinessLogic.ExternalServices.OsPlaces
{
    public interface IOsPlacesApi
    {
        public Task<List<Address>> GetAddressesAsync(string postcode, string buildingNameOrNumber);
    }
}
