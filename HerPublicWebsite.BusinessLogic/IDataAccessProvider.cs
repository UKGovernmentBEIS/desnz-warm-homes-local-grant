using HerPublicWebsite.BusinessLogic.Models;

namespace HerPublicWebsite.BusinessLogic;

public interface IDataAccessProvider
{
    Task<ReferralRequest> PersistNewReferralRequestAsync(ReferralRequest referralRequest);
    Task<ReferralRequest> UpdateReferralRequestByIdWithFollowUpSentAsync(int id);
    Task PersistNotificationConsentAsync(string referralId, NotificationDetails notificationDetails);
    Task<IList<ReferralRequest>> GetUnsubmittedReferralRequestsAsync();
    Task<IList<ReferralRequest>> GetReferralRequestsWithNoFollowUpBetweenDates(DateTime startDate, DateTime endDate);
    Task<IList<ReferralRequest>> GetReferralRequestsByCustodianAndRequestDateAsync(string custodianCode, int month, int year);
    Task<AnonymisedReport> PersistAnonymisedReportAsync(AnonymisedReport report);
    Task<PerReferralReport> PersistPerReferralReportAsync(PerReferralReport report);
    Task PersistAllChangesAsync();
    Task<ReferralRequestFollowUp> PersistReferralFollowUpToken(ReferralRequestFollowUp referralRequestFollowUp);
    Task<ReferralRequestFollowUp> GetReferralFollowUpByToken(string token);
    Task<ReferralRequestFollowUp> UpdateReferralFollowUpByTokenWithWasFollowedUp(string token, bool wasFollowedUp);
}
