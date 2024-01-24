using HerPublicWebsite.BusinessLogic.ExternalServices.Common;
using HerPublicWebsite.BusinessLogic.ExternalServices.S3FileWriter;
using HerPublicWebsite.BusinessLogic.Models;

namespace HerPublicWebsite.BusinessLogic.Services.ReferralFollowUp;

public interface IReferralFollowUpService
{
    public Task<IList<ReferralRequest>> GetReferralsPassedTenWorkingDayThresholdWithNoFollowUp();
    public Task<DateTime> AddWorkingDaysToDateTime(DateTime initialDateTime, int workingDaysToAdd);
}

public class ReferralFollowUpService : IReferralFollowUpService
{
    private readonly IDataAccessProvider dataProvider;
    private readonly CsvFileCreator.CsvFileCreator csvFileCreator;

    public ReferralFollowUpService(
        IDataAccessProvider dataProvider,
        CsvFileCreator.CsvFileCreator csvFileCreator)
    {
        this.dataProvider = dataProvider;
        this.csvFileCreator = csvFileCreator;
    }

    public async Task<IList<ReferralRequest>> GetReferralsPassedTenWorkingDayThresholdWithNoFollowUp()
    {
        var endDate = await AddWorkingDaysToDateTime(DateTime.Today, -10);
        return await dataProvider.GetReferralRequestsWithNoFollowUpBeforeDate(endDate);
    }

    public async Task<DateTime> AddWorkingDaysToDateTime(DateTime initialDateTime, int workingDaysToAdd)
    {
        var direction = workingDaysToAdd < 0 ? -1 : 1;
        var holidays = await getHolidays();
        var newDateTime = initialDateTime;
        while (workingDaysToAdd != 0)
            {
            newDateTime = newDateTime.AddDays(direction);
            if (newDateTime.DayOfWeek != DayOfWeek.Saturday && 
                newDateTime.DayOfWeek != DayOfWeek.Sunday && 
                !holidays.Contains(newDateTime))
            {
                workingDaysToAdd -= direction;
            }
        }
        return newDateTime;
    }

    private async Task<List<DateTime>> getHolidays() {
            var parameters = new RequestParameters
            {
                BaseAddress = "https://www.gov.uk/bank-holidays.json",
            };
            var holidayDataJson = await HttpRequestHelper.SendGetRequestAsync<Dictionary<string, Holidays>>(parameters);
            var englandAndWalesHolidays = holidayDataJson["england-and-wales"];
            return englandAndWalesHolidays.events.Select(x => x.date).ToList();
        }

    private class Holidays {
            public List<Event> events { get; set;}
        }
    private class Event {
            public DateTime date { get; set; }

        }
}

