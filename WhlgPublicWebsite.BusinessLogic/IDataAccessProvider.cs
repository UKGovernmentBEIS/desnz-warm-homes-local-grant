using WhlgPublicWebsite.BusinessLogic.Models;

namespace WhlgPublicWebsite.BusinessLogic;

public interface IDataAccessProvider
{
    Task<ReferralRequest> PersistNewReferralRequestAsync(ReferralRequest referralRequest);
    Task<ReferralRequest> UpdateReferralRequestByIdWithFollowUpSentAsync(int id);
    Task PersistNotificationConsentAsync(string referralId, NotificationDetails notificationDetails);
    Task<IList<ReferralRequest>> GetWhlgUnsubmittedReferralRequestsAsync();
    Task<IList<ReferralRequest>> GetAllWhlgReferralRequests();
    Task<IList<ReferralRequest>> GetAllWhlgReferralRequestsForSlaComplianceReporting();
    Task<IList<ReferralRequest>> GetWhlgReferralRequestsBetweenDates(DateTime startDate, DateTime endDate);

    Task<IList<ReferralRequest>> GetWhlgReferralRequestsWithNoFollowUpBetweenDates(DateTime startDate,
        DateTime endDate);

    Task<IList<ReferralRequest>> GetWhlgReferralRequestsByCustodianAndRequestDateAsync(string custodianCode,
        int month, int year);

    Task PersistAllChangesAsync();
    Task<ReferralRequestFollowUp> PersistReferralFollowUpToken(ReferralRequestFollowUp referralRequestFollowUp);
    Task<ReferralRequestFollowUp> GetReferralFollowUpByToken(string token);
    Task<ReferralRequestFollowUp> UpdateReferralFollowUpByTokenWithWasFollowedUp(string token, bool wasFollowedUp);
    Task<Session> PersistSession(Session session);
    Task RecordEligiblityAndJourneyCompletion(int sessionId, bool? isEligible);
}