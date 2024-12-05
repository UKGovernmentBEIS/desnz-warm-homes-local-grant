using WhlgPublicWebsite.BusinessLogic.Models;

namespace WhlgPublicWebsite.BusinessLogic;

public interface IDataAccessProvider
{
    Task<ReferralRequest> PersistNewReferralRequestAsync(ReferralRequest referralRequest);
    Task<ReferralRequest> UpdateReferralRequestByIdWithFollowUpSentAsync(int id);
    Task PersistNotificationConsentAsync(string referralId, NotificationDetails notificationDetails);
    Task<IList<ReferralRequest>> GetHug2UnsubmittedReferralRequestsAsync();
    Task<IList<ReferralRequest>> GetAllHug2ReferralRequests();
    Task<IList<ReferralRequest>> GetHug2ReferralRequestsBetweenDates(DateTime startDate, DateTime endDate);

    Task<IList<ReferralRequest>> GetHug2ReferralRequestsWithNoFollowUpBetweenDates(DateTime startDate,
        DateTime endDate);

    Task<IList<ReferralRequest>> GetHug2ReferralRequestsByCustodianAndRequestDateAsync(string custodianCode,
        int month, int year);

    Task<AnonymisedReport> PersistAnonymisedReportAsync(AnonymisedReport report);
    Task<PerReferralReport> PersistPerReferralReportAsync(PerReferralReport report);
    Task PersistAllChangesAsync();
    Task<ReferralRequestFollowUp> PersistReferralFollowUpToken(ReferralRequestFollowUp referralRequestFollowUp);
    Task<ReferralRequestFollowUp> GetReferralFollowUpByToken(string token);
    Task<ReferralRequestFollowUp> UpdateReferralFollowUpByTokenWithWasFollowedUp(string token, bool wasFollowedUp);
    Task<Session> PersistSession(Session session);
    Task RecordEligiblityAndJourneyCompletion(int sessionId, bool? isEligible);
}