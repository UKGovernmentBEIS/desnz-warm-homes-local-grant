using WhlgPublicWebsite.BusinessLogic.ExternalServices.S3FileWriter;
using WhlgPublicWebsite.BusinessLogic.Services.CsvFileCreator;

namespace WhlgPublicWebsite.BusinessLogic.Services.RegularJobs;

public interface IUnsubmittedReferralRequestsService
{
    public Task WriteUnsubmittedReferralRequestsToCsv();
}

public class UnsubmittedReferralRequestsService : IUnsubmittedReferralRequestsService
{
    private readonly IDataAccessProvider dataProvider;
    private readonly IS3FileWriter s3FileWriter;
    private readonly ICsvFileCreator csvFileCreator;

    public UnsubmittedReferralRequestsService(
        IDataAccessProvider dataProvider,
        IS3FileWriter s3FileWriter,
        ICsvFileCreator csvFileCreator)
    {
        this.dataProvider = dataProvider;
        this.s3FileWriter = s3FileWriter;
        this.csvFileCreator = csvFileCreator;
    }

    public async Task WriteUnsubmittedReferralRequestsToCsv()
    {
        var newReferrals = await dataProvider.GetWhlgUnsubmittedReferralRequestsAsync();

        foreach (var referralsByCustodianMonthAndYear in newReferrals.GroupBy(nr =>
                     new { nr.CustodianCode, nr.RequestDate.Month, nr.RequestDate.Year }))
        {
            var grouping = referralsByCustodianMonthAndYear.Key;
            var referralsForFile =
                await dataProvider.GetWhlgReferralRequestsByCustodianAndRequestDateAsync(grouping.CustodianCode,
                    grouping.Month, grouping.Year);

            using (var fileData = csvFileCreator.CreateReferralRequestFileDataForS3(referralsForFile))
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