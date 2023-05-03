using HerPublicWebsite.BusinessLogic.Models;

namespace HerPublicWebsite.BusinessLogic.ExternalServices.OsPlaces
{
    public interface IOsPlacesApi
    {
        public Task<List<Address>> GetAddresses(string postcode, string buildingNameOrNumber);
    }
}
