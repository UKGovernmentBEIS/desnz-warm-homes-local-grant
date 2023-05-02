using HerPublicWebsite.BusinessLogic.Models;

namespace HerPublicWebsite.Data;

public interface IDataAccessProvider
{
    Task<ReferralRequest> AddReferralRequestAsync(ReferralRequest referralRequest);
    Task<IEnumerable<ReferralRequest>> GetUnsubmittedReferralRequestsAsync();
}