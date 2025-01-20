using Microsoft.EntityFrameworkCore;
using WhlgPublicWebsite.BusinessLogic;
using WhlgPublicWebsite.BusinessLogic.Models;

namespace WhlgPublicWebsite.Data;

public class DataAccessProvider : IDataAccessProvider
{
    private readonly WhlgDbContext context;
    
    private readonly IEnumerable<string> liveWmcaCustodianCodes;

    public DataAccessProvider(WhlgDbContext context)
    {
        this.context = context;
        liveWmcaCustodianCodes = GetLiveWmcaCustodianCodes();
    }
    
    private IEnumerable<string> GetLiveWmcaCustodianCodes()
    {
        return LocalAuthorityData.LocalAuthorityDetailsByCustodianCode
            .Where(localAuthority => localAuthority.Value.Consortium == "West Midlands Combined Authority" && localAuthority.Value.Status == LocalAuthorityData.LocalAuthorityStatus.Live)
            .Select(localAuthority => localAuthority.Key)
            .ToList();
    }

    public async Task<ReferralRequest> PersistNewReferralRequestAsync(ReferralRequest referralRequest)
    {
        context.ReferralRequests.Add(referralRequest);
        await context.SaveChangesAsync();
        referralRequest.UpdateReferralCode();
        await context.SaveChangesAsync();
        return referralRequest;
    }

    public async Task<ReferralRequest> UpdateReferralRequestByIdWithFollowUpSentAsync(int id)
    {
        var referralRequest = await context.ReferralRequests.SingleAsync(rr => rr.Id == id);
        referralRequest.FollowUpEmailSent = true;
        await context.SaveChangesAsync();
        return referralRequest;
    }

    public async Task PersistNotificationConsentAsync(string referralCode, NotificationDetails details)
    {
        if (details.FutureSchemeNotificationConsent)
        {
            if (referralCode is not null)
            {
                var referralRequest =
                    await context.ReferralRequests
                        .SingleAsync(rr => rr.ReferralCode == referralCode);

                details.ReferralRequest = referralRequest;
            }

            context.NotificationDetails.Add(details);
            await context.SaveChangesAsync();
        }
    }

    public async Task<AnonymisedReport> PersistAnonymisedReportAsync(AnonymisedReport report)
    {
        context.AnonymisedReports.Add(report);
        await context.SaveChangesAsync();

        return report;
    }

    public async Task<PerReferralReport> PersistPerReferralReportAsync(PerReferralReport report)
    {
        context.PerReferralReports.Add(report);
        await context.SaveChangesAsync();

        return report;
    }

    public async Task<IList<ReferralRequest>> GetWhlgUnsubmittedReferralRequestsAsync()
    {
        return await context.ReferralRequests
            .Where(rr => !rr.ReferralWrittenToCsv && !rr.WasSubmittedForFutureGrants)
            .ToListAsync();
    }

    public async Task<IList<ReferralRequest>> GetAllWhlgReferralRequests()
    {
        return await context.ReferralRequests
            .Where(rr => !rr.WasSubmittedForFutureGrants && !liveWmcaCustodianCodes.Contains(rr.CustodianCode))
            .Include(rr => rr.FollowUp)
            .ToListAsync();
    }

    public async Task<IList<ReferralRequest>> GetWhlgReferralRequestsBetweenDates(DateTime startDate,
        DateTime endDate)
    {
        return await context.ReferralRequests
            .Where(rr => rr.RequestDate >= startDate && rr.RequestDate <= endDate && !rr.WasSubmittedForFutureGrants && 
                         !liveWmcaCustodianCodes.Contains(rr.CustodianCode))
            .Include(rr => rr.FollowUp)
            .ToListAsync();
    }

    public async Task<IList<ReferralRequest>> GetWhlgReferralRequestsWithNoFollowUpBetweenDates(
        DateTime startDate,
        DateTime endDate)
    {
        return await context.ReferralRequests
            .Where(rr => rr.RequestDate >= startDate && rr.RequestDate <= endDate && !rr.FollowUpEmailSent &&
                         !rr.WasSubmittedForFutureGrants && !liveWmcaCustodianCodes.Contains(rr.CustodianCode))
            .ToListAsync();
    }

    public async Task<IList<ReferralRequest>> GetWhlgReferralRequestsByCustodianAndRequestDateAsync(
        string custodianCode,
        int month, int year)
    {
        return await context.ReferralRequests
            .Where(rr => rr.CustodianCode == custodianCode && rr.RequestDate.Month == month &&
                         rr.RequestDate.Year == year && !rr.WasSubmittedForFutureGrants)
            .ToListAsync();
    }

    public async Task<ReferralRequestFollowUp> PersistReferralFollowUpToken(
        ReferralRequestFollowUp referralRequestFollowUp)
    {
        context.ReferralRequestFollowUps
            .Add(referralRequestFollowUp);
        await context.SaveChangesAsync();
        return referralRequestFollowUp;
    }

    public async Task<ReferralRequestFollowUp> GetReferralFollowUpByToken(string token)
    {
        return await context.ReferralRequestFollowUps.Include(rrfu => rrfu.ReferralRequest)
            .SingleAsync(rrfu => rrfu.Token == token);
    }

    public async Task<ReferralRequestFollowUp> UpdateReferralFollowUpByTokenWithWasFollowedUp(string token,
        bool wasFollowedUp)
    {
        var referralRequestFollowUp = await context.ReferralRequestFollowUps.SingleAsync(rrfu => rrfu.Token == token);
        referralRequestFollowUp.WasFollowedUp = wasFollowedUp;
        referralRequestFollowUp.DateOfFollowUpResponse = DateTime.Now;
        await context.SaveChangesAsync();
        return referralRequestFollowUp;
    }

    public async Task PersistAllChangesAsync()
    {
        await context.SaveChangesAsync();
    }

    public async Task<Session> PersistSession(Session session)
    {
        context.Sessions
            .Add(session);
        await context.SaveChangesAsync();
        return session;
    }

    public async Task RecordEligiblityAndJourneyCompletion(int sessionId, bool? isEligible)
    {
        var referralRequest = await context.Sessions.SingleAsync(session => session.Id == sessionId);
        referralRequest.IsJourneyComplete = true;
        referralRequest.IsEligible = isEligible;
        await context.SaveChangesAsync();
    }
}