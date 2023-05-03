using HerPublicWebsite.BusinessLogic.Models;

namespace HerPublicWebsite.BusinessLogic;

public interface IDataAccessProvider
{
    Task<ReferralRequest> PersistNewReferralRequestAsync(ReferralRequest referralRequest);
    Task<IEnumerable<ReferralRequest>> GetUnsubmittedReferralRequestsAsync();
    Task PersistAllChangesAsync();
}