using HerPublicWebsite.BusinessLogic.Models;

namespace HerPublicWebsite.BusinessLogic.Services.SessionRecorder;

public interface ISessionRecorderService
{
    public Task RecordNewSessionStarted();
}

public class SessionRecorderService : ISessionRecorderService
{
    private readonly IDataAccessProvider dataAccessProvider;
    private readonly IDateTimeProvider dateTimeProvider;

    public SessionRecorderService(IDataAccessProvider dataAccessProvider, IDateTimeProvider dateTimeProvider)
    {
        this.dataAccessProvider = dataAccessProvider;
        this.dateTimeProvider = dateTimeProvider;
    }

    public async Task RecordNewSessionStarted()
    {
        var session = new Session
        {
            Timestamp = dateTimeProvider.Now()
        };
        await dataAccessProvider.PersistSession(session);
    }
}