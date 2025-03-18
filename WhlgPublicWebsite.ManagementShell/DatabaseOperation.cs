using WhlgPublicWebsite.BusinessLogic.Models;
using WhlgPublicWebsite.Data;

namespace WhlgPublicWebsite.ManagementShell;

public interface IDatabaseOperation
{
    public void AddReferralRequests(IEnumerable<ReferralRequest> referralRequests);
    public IList<ReferralRequest> GetAllWhlgReferralRequestsSubmittedAfterHug2Shutdown();
}

public class DatabaseOperation(WhlgDbContext dbContext, IOutputProvider outputProvider) : IDatabaseOperation
{
    private static readonly DateTime Hug2ShutdownDate = new(2025, 02, 03);

    public void AddReferralRequests(IEnumerable<ReferralRequest> referralRequests)
    {
        outputProvider.Output("(1/2) Adding fake referrals");
        PerformTransaction(() => { dbContext.ReferralRequests.AddRange(referralRequests); });

        outputProvider.Output("(2/2) Adding WHLG IDs for each referral");
        PerformTransaction(() =>
        {
            foreach (var referralRequest in referralRequests) referralRequest.UpdateReferralCode();
        });
    }

    private void PerformTransaction(Action transaction)
    {
        using var dbContextTransaction = dbContext.Database.BeginTransaction();
        try
        {
            transaction();

            dbContext.SaveChanges();
            outputProvider.Output("Operation successful");
            dbContextTransaction.Commit();
        }
        catch (Exception e)
        {
            outputProvider.Output($"Rollback following error in transaction: {e.InnerException?.Message}");
            dbContextTransaction.Rollback();
        }
    }

    public IList<ReferralRequest> GetAllWhlgReferralRequestsSubmittedAfterHug2Shutdown()
    {
        outputProvider.Output("Retrieving all WHLG referrals submitted after HUG2 Shutdown.");
        var whlgReferrals = dbContext.ReferralRequests
            .Where(rr => rr.RequestDate >= Hug2ShutdownDate)
            .ToList();
        outputProvider.Output($"{whlgReferrals.Count} referrals found.");
        return whlgReferrals;
    }
}