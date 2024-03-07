namespace HerPublicWebsite.BusinessLogic.Services.RegularJobs;

public class StartOfMonthService
{
    private readonly DateTime today;
    
    public StartOfMonthService()
    {
        today = DateTime.Today;
    }

    public StartOfMonthService(DateTime today)
    {
        this.today = today;
    }

    public DateTime GetStartOfPreviousMonth()
    {
        return today.AddMonths(-1).AddDays(1-today.Day); //Remove a month, and set day to 1st
    }
}