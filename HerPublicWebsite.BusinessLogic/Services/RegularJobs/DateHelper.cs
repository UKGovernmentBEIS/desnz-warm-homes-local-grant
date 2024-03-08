namespace HerPublicWebsite.BusinessLogic.Services.RegularJobs;

public class DateHelper
{
    private readonly Func<DateTime> getToday;
    
    public DateHelper(): this(() => DateTime.Today)
    {
    }

    public DateHelper(Func<DateTime> getToday)
    {
        this.getToday = getToday;
    }

    public DateTime GetStartOfPreviousMonth()
    {
        var today = getToday();
        var oneMonthAgo = today.AddMonths(-1);
        return oneMonthAgo.AddDays(1 - oneMonthAgo.Day); // Remove a month, and set day to 1st
    }
}