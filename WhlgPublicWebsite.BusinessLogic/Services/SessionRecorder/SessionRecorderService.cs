using WhlgPublicWebsite.BusinessLogic.Models;

namespace WhlgPublicWebsite.BusinessLogic.Services.SessionRecorder;

public interface ISessionRecorderService
{
    public Task<Session> RecordNewSessionStarted();

    public Task RecordEligibilityAndJourneyCompletion(Questionnaire questionnaire, bool? isEligible);
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

    public async Task RecordEligibilityAndJourneyCompletion(Questionnaire questionnaire, bool? isEligible)
    {
        var sessionId = questionnaire.SessionId;

        if (sessionId == null)
        {
            throw new Exception("Session ID is null at journey completion");
        }

        await dataAccessProvider.RecordEligiblityAndJourneyCompletion((int)sessionId, isEligible);
    }
}