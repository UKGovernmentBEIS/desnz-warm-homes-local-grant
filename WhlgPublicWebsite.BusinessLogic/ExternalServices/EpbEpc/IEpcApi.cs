using WhlgPublicWebsite.BusinessLogic.Models;

namespace WhlgPublicWebsite.BusinessLogic.ExternalServices.EpbEpc
{
    public interface IEpcApi
    {
        public Task<EpcDetails> EpcFromUprnAsync(string uprn);
    }
}
