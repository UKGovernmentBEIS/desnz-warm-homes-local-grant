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
        return referralRequest;
    }

    public async Task<IList<ReferralRequest>> GetUnsubmittedReferralRequestsAsync()
    {
        return await context.ReferralRequests
            .Where(rr => !rr.ReferralCreated)
            .ToListAsync();
    }
    
    public async Task<IList<ReferralRequest>> GetReferralRequestsByCustodianAndRequestDateAsync(int custodianCode, int month, int year)
    {
        return await context.ReferralRequests
            .Include(rr => rr.ContactDetails)
            .Where(rr => rr.CustodianCode == custodianCode && rr.RequestDate.Month == month && rr.RequestDate.Year == year)
            .ToListAsync();
    }

    public async Task PersistAllChangesAsync()
    {
        await context.SaveChangesAsync();
    }
}