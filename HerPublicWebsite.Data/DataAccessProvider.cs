using HerPublicWebsite.BusinessLogic;
using Microsoft.EntityFrameworkCore;
using HerPublicWebsite.BusinessLogic.Models;

namespace HerPublicWebsite.Data;

public class DataAccessProvider : IDataAccessProvider
{
    private readonly HerDbContext context;

    public DataAccessProvider(HerDbContext context)
    {
        this.context = context;
    }

    public async Task<ReferralRequest> PersistNewReferralRequestAsync(ReferralRequest referralRequest)
    {
        context.ReferralRequests.Add(referralRequest);
        await context.SaveChangesAsync();
        referralRequest.UpdateReferralCode();
        await context.SaveChangesAsync();
        return referralRequest;
    }

    public async Task PersistNotificationConsentAsync(string referralCode, NotificationDetails details)
    {
        if (details.FutureSchemeNotificationConsent)
        {
            var referralRequest =
                context.ReferralRequests.Single(rr => rr.ReferralCode == referralCode);

            details.ReferralRequest = referralRequest;

            context.NotificationDetails.Add(details);
            await context.SaveChangesAsync();
        }
    }

    public async Task<IList<ReferralRequest>> GetUnsubmittedReferralRequestsAsync()
    {
        return await context.ReferralRequests
            .Where(rr => !rr.ReferralWrittenToCsv)
            .ToListAsync();
    }
    
    public async Task<IList<ReferralRequest>> GetReferralRequestsByCustodianAndRequestDateAsync(string custodianCode, int month, int year)
    {
        return await context.ReferralRequests
            .Where(rr => rr.CustodianCode == custodianCode && rr.RequestDate.Month == month && rr.RequestDate.Year == year)
            .ToListAsync();
    }

    public async Task PersistAllChangesAsync()
    {
        await context.SaveChangesAsync();
    }
}