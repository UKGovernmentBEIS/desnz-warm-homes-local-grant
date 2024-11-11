using HerPublicWebsite.BusinessLogic.Models;

namespace HerPublicWebsite.BusinessLogic;

public interface IDataAccessProvider
{
    Task<ReferralRequest> PersistNewReferralRequestAsync(ReferralRequest referralRequest);
    Task<ReferralRequest> UpdateReferralRequestByIdWithFollowUpSentAsync(int id);
    Task PersistNotificationConsentAsync(string referralId, NotificationDetails notificationDetails);
    Task<IList<ReferralRequest>> GetUnsubmittedReferralRequestsForCurrentGrantAsync();
    Task<IList<ReferralRequest>> GetAllCurrentGrantReferralRequests();
    Task<IList<ReferralRequest>> GetCurrentGrantReferralRequestsBetweenDates(DateTime startDate, DateTime endDate);
    Task<IList<ReferralRequest>> GetCurrentGrantReferralRequestsWithNoFollowUpBetweenDates(DateTime startDate, DateTime endDate);
    Task<IList<ReferralRequest>> GetCurrentGrantReferralRequestsByCustodianAndRequestDateAsync(string custodianCode, int month, int year);
    Task<AnonymisedReport> PersistAnonymisedReportAsync(AnonymisedReport report);
    Task<PerReferralReport> PersistPerReferralReportAsync(PerReferralReport report);
    Task PersistAllChangesAsync();
    Task<ReferralRequestFollowUp> PersistReferralFollowUpToken(ReferralRequestFollowUp referralRequestFollowUp);
    Task<ReferralRequestFollowUp> GetReferralFollowUpByToken(string token);
    Task<ReferralRequestFollowUp> UpdateReferralFollowUpByTokenWithWasFollowedUp(string token, bool wasFollowedUp);
    Task<Session> PersistSession(Session session);
    Task RecordEligiblityAndJourneyCompletion(int sessionId, bool? isEligible);
}
