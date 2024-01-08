using System.Net.Http.Json;
using HerPublicWebsite.BusinessLogic.ExternalServices.Common;
using HerPublicWebsite.BusinessLogic.ExternalServices.S3FileWriter;
using HerPublicWebsite.BusinessLogic.Models;

namespace HerPublicWebsite.BusinessLogic.Services.RegularJobs;

public interface IRegularJobsService
{
    public Task RunNightlyTasksAsync();
    public Task WriteUnsubmittedReferralRequestToCsv();
    public Task<IList<ReferralRequest>> GetReferralsPassedTenWorkingDayThresholdWithNoFollowUp();
    public Task<DateTime> AddWorkingDaysToDateTime(DateTime dateTime, int workingDaysToAdd);
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
        await WriteUnsubmittedReferralRequestToCsv();
        await GetReferralsPassedTenWorkingDayThresholdWithNoFollowUp();
    }

    public async Task WriteUnsubmittedReferralRequestToCsv() 
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

    public async Task<IList<ReferralRequest>> GetReferralsPassedTenWorkingDayThresholdWithNoFollowUp()
    {
        var startDate = await AddWorkingDaysToDateTime(DateTime.Now, -30);
        var newReferrals = await dataProvider.GetReferralRequestsWithNoFollowUpAfterDate(startDate);
        return newReferrals;
    }

    public async Task<DateTime> AddWorkingDaysToDateTime(DateTime dateTime, int workingDaysToAdd)
    {
        int direction = workingDaysToAdd < 0 ? -1 : 1;
        List<DateTime> holidays = await getHolidays();
        DateTime newDate = dateTime;
        Console.WriteLine("Running");
        while (workingDaysToAdd != 0)
            {
            newDate = newDate.AddDays(direction);
            if (holidays.Contains(newDate)) {
                Console.WriteLine("Holiday on");
                Console.WriteLine(newDate);
            }
            if (newDate.DayOfWeek != DayOfWeek.Saturday && 
                newDate.DayOfWeek != DayOfWeek.Sunday && 
                !holidays.Contains(newDate))
            {
                workingDaysToAdd -= direction;
            }
        }
        return newDate;
    }

    private async Task<List<DateTime>> getHolidays() {
            var httpClient = new HttpClient();
            var parameters = new RequestParameters
            {
                BaseAddress = "https://www.gov.uk/bank-holidays.json",
            };
            var json = await HttpRequestHelper.SendGetRequestAsync<Dictionary<string, Holidays>>(parameters);
            var englandAndWalesHolidays = json["england-and-wales"];
            var foo = englandAndWalesHolidays.events.Select(x => x.date).ToList();
            Console.WriteLine(foo[0]);
            return foo;
        }
}

public class Holidays
{
    public string division { get; set; }
    public List<Event> events { get; set; }
}

public class Event
{
    public DateTime date { get; set; }
    public string notes { get; set; }
    public string title { get; set; }
}