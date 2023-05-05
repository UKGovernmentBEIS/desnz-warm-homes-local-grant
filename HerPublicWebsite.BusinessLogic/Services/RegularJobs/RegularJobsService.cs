using HerPublicWebsite.BusinessLogic.ExternalServices.S3FileWriter;

namespace HerPublicWebsite.BusinessLogic.Services.RegularJobs;

public interface IRegularJobsService
{
    public Task RunNightlyTasksAsync();
}

public class RegularJobsService : IRegularJobsService
{
    private readonly IDataAccessProvider dataProvider;
    private readonly IS3FileWriter s3FileWriter;

    public RegularJobsService(
        IDataAccessProvider dataProvider,
        IS3FileWriter s3FileWriter)
    {
        this.dataProvider = dataProvider;
        this.s3FileWriter = s3FileWriter;
    }

    public async Task RunNightlyTasksAsync()
    {
        var newReferrals = await dataProvider.GetUnsubmittedReferralRequestsAsync();

        foreach (var referralsByCustodianMonthAndYear in newReferrals.GroupBy(nr => new { nr.CustodianCode, nr.RequestDate.Month, nr.RequestDate.Year }))
        {
            var grouping = referralsByCustodianMonthAndYear.Key;
            var referralsForFile = await dataProvider.GetReferralRequestsByCustodianAndRequestDateAsync(grouping.CustodianCode, grouping.Month, grouping.Year);
            
            // TODO Generate the CSV file
            var fileData = new MemoryStream();

            await s3FileWriter.WriteFileAsync(grouping.CustodianCode, grouping.Month, grouping.Year, fileData);
            
            foreach (var referralRequest in referralsForFile)
            {
                referralRequest.ReferralCreated = true;
            }

            await dataProvider.PersistAllChangesAsync();
        }
    }
}