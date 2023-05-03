using HerPublicWebsite.BusinessLogic.Models;

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
        Console.WriteLine("RunNightlyTasks called");
        var newReferrals = await dataProvider.GetUnsubmittedReferralRequestsAsync();
    }
}