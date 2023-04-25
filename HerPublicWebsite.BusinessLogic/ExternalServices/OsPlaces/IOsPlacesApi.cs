namespace HerPublicWebsite.BusinessLogic.ExternalServices.OsPlaces
{
    public interface IOsPlacesApi
    {
        public Task GetLocalAuthorityFromPostcode(string postcode); // TODO: Create real EPC methods
    }
}
