using HerPublicWebsite.BusinessLogic.Models;
using HerPublicWebsite.Data;

namespace HerPublicWebsite.ManagementShell;

public interface IDatabaseOperation
{
    public void AddReferralRequests(IEnumerable<ReferralRequest> referralRequests);
}

public class DatabaseOperation : IDatabaseOperation
{
    private readonly HerDbContext dbContext;
    private readonly IOutputProvider outputProvider;

    public DatabaseOperation(HerDbContext dbContext, IOutputProvider outputProvider)
    {
        this.dbContext = dbContext;
        this.outputProvider = outputProvider;
    }

    public void AddReferralRequests(IEnumerable<ReferralRequest> referralRequests)
    {
        PerformTransaction(() =>
        {
            dbContext.ReferralRequests.AddRange(referralRequests);
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
}