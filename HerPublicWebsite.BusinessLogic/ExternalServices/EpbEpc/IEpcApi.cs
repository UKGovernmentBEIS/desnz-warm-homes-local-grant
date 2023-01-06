using HerPublicWebsite.BusinessLogic.Models;

namespace HerPublicWebsite.BusinessLogic.ExternalServices.EpbEpc
{
    public interface IEpcApi
    {
        public Task<List<EpcSearchResult>> GetEpcsInformationForPostcodeAndBuildingNameOrNumber(string postcode,
            string buildingNameOrNumber = null);
        public Task<Epc> GetEpcForId(string epcId);
    }
}