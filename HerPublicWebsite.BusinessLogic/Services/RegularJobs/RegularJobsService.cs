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
    private readonly CsvFileCreator.CsvFileCreator csvFileCreator;

    public RegularJobsService(
        IDataAccessProvider dataProvider,
        IS3FileWriter s3FileWriter,
        CsvFileCreator.CsvFileCreator csvFileCreator)
    {
        this.dataProvider = dataProvider;
        this.s3FileWriter = s3FileWriter;
        this.csvFileCreator = csvFileCreator;
    }

    public async Task RunNightlyTasksAsync()
    {
        var newReferrals = await dataProvider.GetUnsubmittedReferralRequestsAsync();

        foreach (var referralsByCustodianMonthAndYear in newReferrals.GroupBy(nr => new { nr.CustodianCode, nr.RequestDate.Month, nr.RequestDate.Year }))
        {
            var grouping = referralsByCustodianMonthAndYear.Key;
            var referralsForFile = await dataProvider.GetReferralRequestsByCustodianAndRequestDateAsync(grouping.CustodianCode, grouping.Month, grouping.Year);

            using (var fileData = csvFileCreator.CreateFileData(referralsForFile))
            {
                await s3FileWriter.WriteFileAsync(grouping.CustodianCode, grouping.Month, grouping.Year, fileData);
            }

            foreach (var referralRequest in referralsForFile)
            {
                referralRequest.ReferralCreated = true;
            }

            await dataProvider.PersistAllChangesAsync();
        }
    }
}
