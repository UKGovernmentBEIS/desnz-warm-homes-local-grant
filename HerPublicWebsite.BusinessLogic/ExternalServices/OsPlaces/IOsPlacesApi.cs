using HerPublicWebsite.BusinessLogic.Models;

namespace HerPublicWebsite.BusinessLogic.ExternalServices.OsPlaces
{
    public interface IOsPlacesApi
    {
        public Task<List<Address>> GetAddressesAsync(string postcode, string buildingNameOrNumber);
    }
}
