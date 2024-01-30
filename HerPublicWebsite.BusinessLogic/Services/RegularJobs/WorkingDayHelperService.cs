using HerPublicWebsite.BusinessLogic.ExternalServices.Common;

namespace HerPublicWebsite.BusinessLogic.Services.RegularJobs;

public interface IWorkingDayHelperService
{
    public Task<DateTime> AddWorkingDaysToDateTime(DateTime initialDateTime, int workingDaysToAdd);
}

public class WorkingDayHelperService : IWorkingDayHelperService
{
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
                !holidays.Contains(newDateTime.Date))
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
