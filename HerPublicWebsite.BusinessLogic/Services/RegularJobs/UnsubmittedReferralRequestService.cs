using HerPublicWebsite.BusinessLogic.ExternalServices.S3FileWriter;

namespace HerPublicWebsite.BusinessLogic.Services.UnsubmittedReferralRequests;

public interface IUnsubmittedReferralRequestsService
{
    public Task WriteUnsubmittedReferralRequestsToCsv();
}

public class UnsubmittedReferralRequestsService : IUnsubmittedReferralRequestsService
{
    private readonly IDataAccessProvider dataProvider;
    private readonly IS3FileWriter s3FileWriter;
    private readonly CsvFileCreator.CsvFileCreator csvFileCreator;

    public UnsubmittedReferralRequestsService(
        IDataAccessProvider dataProvider,
        IS3FileWriter s3FileWriter,
        CsvFileCreator.CsvFileCreator csvFileCreator)
    {
        this.dataProvider = dataProvider;
        this.s3FileWriter = s3FileWriter;
        this.csvFileCreator = csvFileCreator;
    }
    
    public async Task WriteUnsubmittedReferralRequestsToCsv() 
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
                referralRequest.ReferralWrittenToCsv = true;
            }

            await dataProvider.PersistAllChangesAsync();
        }
    }
}

