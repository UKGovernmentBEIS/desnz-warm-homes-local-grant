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

    public async Task<ReferralRequest> AddReferralRequestAsync(ReferralRequest referralRequest)
    {
        context.ReferralRequests.Add(referralRequest);
        await context.SaveChangesAsync();
        return referralRequest;
    }

    public async Task<IEnumerable<ReferralRequest>> GetUnsubmittedReferralRequestsAsync()
    {
        return await context.ReferralRequests
            .Include(rr => rr.ContactDetails)
            .Where(rr => !rr.ReferralCreated)
            .ToListAsync();
    }
}