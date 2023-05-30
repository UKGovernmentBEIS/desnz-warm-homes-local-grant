using HerPublicWebsite.BusinessLogic.Models;

namespace HerPublicWebsite.BusinessLogic;

public interface IDataAccessProvider
{
    Task<ReferralRequest> PersistNewReferralRequestAsync(ReferralRequest referralRequest);
    Task PersistNotificationConsentAsync(string referralId, NotificationDetails notificationDetails);
    Task<IList<ReferralRequest>> GetUnsubmittedReferralRequestsAsync();
    Task<IList<ReferralRequest>> GetReferralRequestsByCustodianAndRequestDateAsync(string custodianCode, int month, int year);
    Task PersistAllChangesAsync();
}
