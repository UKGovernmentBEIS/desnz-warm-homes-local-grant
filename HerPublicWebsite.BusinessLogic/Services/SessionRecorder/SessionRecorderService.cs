using HerPublicWebsite.BusinessLogic.Models;

namespace HerPublicWebsite.BusinessLogic.Services.SessionRecorder;

public interface ISessionRecorderService
{
    public Task<Session> RecordNewSessionStarted();

    public Task SetJourneyComplete(Questionnaire questionnaire);
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

    public async Task SetJourneyComplete(Questionnaire questionnaire)
    {
        var sessionId = questionnaire.SessionId;

        if (sessionId == null)
        {
            throw new Exception("Session ID is null at journey completion");
        }

        await dataAccessProvider.SetJourneyComplete((int)sessionId);
    }
}