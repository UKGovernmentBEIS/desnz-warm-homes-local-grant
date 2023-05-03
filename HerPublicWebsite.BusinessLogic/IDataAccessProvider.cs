using HerPublicWebsite.BusinessLogic.Models;

namespace HerPublicWebsite.BusinessLogic;

public interface IDataAccessProvider
{
    Task<ReferralRequest> AddReferralRequestAsync(ReferralRequest referralRequest);
    Task<IEnumerable<ReferralRequest>> GetUnsubmittedReferralRequestsAsync();
}