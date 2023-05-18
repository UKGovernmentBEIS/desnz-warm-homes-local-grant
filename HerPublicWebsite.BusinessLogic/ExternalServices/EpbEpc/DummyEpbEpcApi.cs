using HerPublicWebsite.BusinessLogic.Models;
using HerPublicWebsite.BusinessLogic.Models.Enums;

namespace HerPublicWebsite.BusinessLogic.ExternalServices.EpbEpc
{
    // Temporary implementation that always returns a high EPC rating
    public class DummyEpbEpcApi : IEpcApi
    {
        public DummyEpbEpcApi()
        {
            
        }
        
        public async Task<EpcDetails> EpcFromUprnAsync(string uprn)
        {
            return await Task.FromResult(new EpcDetails
            {
                AddressLine1 = "1 Address",
                AddressLine2 = "Dummy line 2",
                AddressLine3 = "Dummy line 3",
                AddressLine4 = "Dummy line 4",
                AddressTown = "Dummytown",
                AddressPostcode = "AB1 2CD",
                EpcRating = EpcRating.C,
                LodgementDate = new DateTime(2016, 03, 20),
                ExpiryDate = new DateTime(2026, 03, 20),
                PropertyType = PropertyType.House,
                HouseType = HouseType.Terraced
            });
        }
    }
}
