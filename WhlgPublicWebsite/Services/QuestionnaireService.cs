using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using WhlgPublicWebsite.BusinessLogic;
using WhlgPublicWebsite.BusinessLogic.Models;
using WhlgPublicWebsite.BusinessLogic.Models.Enums;
using WhlgPublicWebsite.BusinessLogic.Services.SessionRecorder;

namespace WhlgPublicWebsite.Services;

public class QuestionnaireService
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        Converters = { new JsonStringEnumConverter() }
    };

    private static readonly string SessionKeyQuestionnaire = "_Questionnaire";
    private readonly IHttpContextAccessor httpContextAccessor;
    private readonly QuestionnaireUpdater questionnaireUpdater;
    private readonly ISessionRecorderService sessionRecorderService;

    public QuestionnaireService(
        QuestionnaireUpdater questionnaireUpdater,
        IHttpContextAccessor httpContextAccessor,
        ISessionRecorderService sessionRecorderService)
    {
        this.questionnaireUpdater = questionnaireUpdater;
        this.httpContextAccessor = httpContextAccessor;
        this.sessionRecorderService = sessionRecorderService;
    }

    public Questionnaire GetQuestionnaire()
    {
        var questionnaireString = httpContextAccessor.HttpContext!.Session.GetString(SessionKeyQuestionnaire);

        var questionnaire = questionnaireString == null
            ? new Questionnaire()
            : JsonSerializer.Deserialize<Questionnaire>(questionnaireString, JsonSerializerOptions);

        return questionnaire;
    }

    public async Task<Questionnaire> UpdateCountry(Country country, QuestionFlowStep? entryPoint)
    {
        var questionnaire = GetQuestionnaire();
        questionnaire = questionnaireUpdater.UpdateCountry(questionnaire, country, entryPoint);
        await SaveQuestionnaireToSession(questionnaire);
        return questionnaire;
    }

    public async Task<Questionnaire> UpdateOwnershipStatus(OwnershipStatus ownershipStatus,
        QuestionFlowStep? entryPoint)
    {
        var questionnaire = GetQuestionnaire();
        questionnaire = questionnaireUpdater.UpdateOwnershipStatus(questionnaire, ownershipStatus, entryPoint);
        await SaveQuestionnaireToSession(questionnaire);
        return questionnaire;
    }

    public async Task<Questionnaire> UpdateAddressAsync(Address address, QuestionFlowStep? entryPoint)
    {
        var questionnaire = GetQuestionnaire();
        if (questionnaire.AddressLine1 != address.AddressLine1 ||
            questionnaire.AddressLine2 != address.AddressLine2 ||
            questionnaire.AddressCounty != address.County ||
            questionnaire.AddressTown != address.Town ||
            questionnaire.AddressPostcode != address.Postcode)
        {
            questionnaire = questionnaireUpdater.UpdateAcknowledgedPending(questionnaire, false, entryPoint);
        }

        questionnaire = await questionnaireUpdater.UpdateAddressAsync(questionnaire, address, entryPoint);
        await SaveQuestionnaireToSession(questionnaire);
        return questionnaire;
    }

    public async Task<Questionnaire> UpdateEpcIsCorrect(EpcConfirmation? epcIsCorrect, QuestionFlowStep? entryPoint)
    {
        var questionnaire = GetQuestionnaire();
        questionnaire = questionnaireUpdater.UpdateEpcIsCorrect(questionnaire, epcIsCorrect, entryPoint);
        await SaveQuestionnaireToSession(questionnaire);
        return questionnaire;
    }

    public async Task<Questionnaire> UpdateLocalAuthority(string custodianCode, QuestionFlowStep? entryPoint)
    {
        var questionnaire = GetQuestionnaire();
        if (questionnaire.CustodianCode != custodianCode)
        {
            questionnaire = questionnaireUpdater.UpdateAcknowledgedPending(questionnaire, false, entryPoint);
            questionnaire = questionnaireUpdater.UpdateAcknowledgedFutureReferral(questionnaire, false, entryPoint);
        }

        questionnaire = questionnaireUpdater.UpdateLocalAuthority(questionnaire, custodianCode, entryPoint);
        await SaveQuestionnaireToSession(questionnaire);
        return questionnaire;
    }

    public async Task<Questionnaire> UpdateLocalAuthorityIsCorrect(bool laIsCorrect, QuestionFlowStep? entryPoint)
    {
        var questionnaire = GetQuestionnaire();
        questionnaire = questionnaireUpdater.UpdateLocalAuthorityIsCorrect(questionnaire, laIsCorrect, entryPoint);
        await SaveQuestionnaireToSession(questionnaire);
        return questionnaire;
    }

    public async Task<Questionnaire> UpdateAcknowledgedPending(bool acknowledgedPending, QuestionFlowStep? entryPoint)
    {
        var questionnaire = GetQuestionnaire();
        questionnaire = questionnaireUpdater.UpdateAcknowledgedPending(questionnaire, acknowledgedPending, entryPoint);
        await SaveQuestionnaireToSession(questionnaire);
        return questionnaire;
    }

    public async Task<Questionnaire> UpdateAcknowledgedFutureReferral(bool acknowledgedFutureReferral,
        QuestionFlowStep? entryPoint)
    {
        var questionnaire = GetQuestionnaire();
        questionnaire =
            questionnaireUpdater.UpdateAcknowledgedFutureReferral(questionnaire, acknowledgedFutureReferral,
                entryPoint);
        await SaveQuestionnaireToSession(questionnaire);
        return questionnaire;
    }

    public async Task<Questionnaire> UpdateHouseholdIncome(IncomeBand incomeBand, QuestionFlowStep? entryPoint)
    {
        var questionnaire = GetQuestionnaire();
        questionnaire = questionnaireUpdater.UpdateHouseholdIncome(questionnaire, incomeBand, entryPoint);
        await SaveQuestionnaireToSession(questionnaire);
        return questionnaire;
    }

    public async Task<Questionnaire> GenerateReferralAsync(string name, string emailAddress, string telephone)
    {
        var questionnaire = GetQuestionnaire();
        questionnaire = await questionnaireUpdater.GenerateReferralAsync(questionnaire, name, emailAddress, telephone);
        await SaveQuestionnaireToSession(questionnaire);
        return questionnaire;
    }

    public async Task<Questionnaire> RecordNotificationConsentAsync(bool consentGranted)
    {
        var questionnaire = GetQuestionnaire();
        questionnaire = await questionnaireUpdater.RecordNotificationConsentAsync(questionnaire, consentGranted);
        await SaveQuestionnaireToSession(questionnaire);
        return questionnaire;
    }

    public async Task<Questionnaire> RecordNotificationConsentAsync(bool consentGranted, string emailAddress)
    {
        var questionnaire = GetQuestionnaire();
        questionnaire =
            await questionnaireUpdater.RecordNotificationConsentAsync(questionnaire, consentGranted, emailAddress);
        await SaveQuestionnaireToSession(questionnaire);
        return questionnaire;
    }

    public async Task<Questionnaire> RecordConfirmationAndNotificationConsentAsync(
        bool notificationConsentGranted,
        string notificationEmailAddress,
        bool confirmationConsentGranted,
        string confirmationEmailAddress)
    {
        var questionnaire = GetQuestionnaire();
        questionnaire = await questionnaireUpdater.RecordConfirmationAndNotificationConsentAsync(
            questionnaire,
            notificationConsentGranted,
            notificationEmailAddress,
            confirmationConsentGranted,
            confirmationEmailAddress);
        await SaveQuestionnaireToSession(questionnaire);
        return questionnaire;
    }

    public async Task SaveQuestionnaireToSession(Questionnaire questionnaire)
    {
        var hasNotSavedQuestionnaireBefore =
            httpContextAccessor.HttpContext!.Session.GetString(SessionKeyQuestionnaire) == null;
        if (hasNotSavedQuestionnaireBefore)
        {
            var newSessionStarted = await sessionRecorderService.RecordNewSessionStarted();
            questionnaire = questionnaireUpdater.RecordSessionId(questionnaire, newSessionStarted.Id);
        }

        var questionnaireString = JsonSerializer.Serialize(questionnaire, JsonSerializerOptions);
        httpContextAccessor.HttpContext!.Session.SetString(SessionKeyQuestionnaire, questionnaireString);
    }
}