using HerPublicWebsite.BusinessLogic.Models;

namespace HerPublicWebsite.BusinessLogic.Services.SessionRecorder;

public interface ISessionRecorderService
{
    public Task RecordNewSessionStarted();
}

public class SessionRecorderService : ISessionRecorderService
{
    private readonly IDataAccessProvider dataAccessProvider;

    public SessionRecorderService(IDataAccessProvider dataAccessProvider)
    {
        this.dataAccessProvider = dataAccessProvider;
    }

    public async Task RecordNewSessionStarted()
    {
        var session = new Session
        {
            Timestamp = DateTime.Now
        };
        await dataAccessProvider.PersistSession(session);
    }
}