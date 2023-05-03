namespace HerPublicWebsite.BusinessLogic.Services.RegularJobs;

public interface IRegularJobsService
{
    public Task RunNightlyTasksAsync();
}

public class RegularJobsService : IRegularJobsService
{
    private readonly IDataAccessProvider dataProvider;

    public RegularJobsService(IDataAccessProvider dataProvider)
    {
        this.dataProvider = dataProvider;
    }

    public async Task RunNightlyTasksAsync()
    {
        var newReferrals = await dataProvider.GetUnsubmittedReferralRequestsAsync();
        
        // TODO Generate and write the CSV file
        
        foreach (var referralRequest in newReferrals)
        {
            referralRequest.ReferralCreated = true;
        }

        await dataProvider.PersistAllChangesAsync();
    }
}