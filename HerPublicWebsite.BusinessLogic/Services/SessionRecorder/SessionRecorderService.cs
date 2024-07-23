using HerPublicWebsite.BusinessLogic.Models;

namespace HerPublicWebsite.BusinessLogic.Services.SessionRecorder;

public interface ISessionRecorderService
{
    public Task<Session> RecordNewSessionStarted();

    public Task SetIsJourneyCompleteToTrue(Questionnaire questionnaire);
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

    public async Task<Session> RecordNewSessionStarted()
    {
        var session = new Session
        {
            Timestamp = dateTimeProvider.Now()
        };
        return await dataAccessProvider.PersistSession(session);
    }

    public async Task SetIsJourneyCompleteToTrue(Questionnaire questionnaire)
    {
        var sessionId = questionnaire.SessionId;
        if (sessionId != null)
        {
            await dataAccessProvider.SetIsJourneyCompleteToTrue((int)sessionId);
        }
    }
}