namespace HerPublicWebsite.BusinessLogic.Services.SessionRecorder;

public interface IDateTimeProvider
{
    public DateTime Now();
}

public class DateTimeProvider : IDateTimeProvider
{
    public DateTime Now()
    {
        return DateTime.UtcNow;
    }
}