using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using HerPublicWebsite.BusinessLogic;
using HerPublicWebsite.BusinessLogic.Models;
using HerPublicWebsite.BusinessLogic.Models.Enums;
using Microsoft.AspNetCore.Http;

namespace HerPublicWebsite.Services;

public class QuestionnaireService
{
    private readonly QuestionnaireUpdater questionnaireUpdater;
    private readonly IHttpContextAccessor httpContextAccessor;

    private static readonly JsonSerializerOptions JsonSerializerOptions = new JsonSerializerOptions()
    {
        Converters = { new JsonStringEnumConverter() }
    };

    private static string SessionKeyQuestionnaire = "_Questionnaire";

    public QuestionnaireService(
        QuestionnaireUpdater questionnaireUpdater,
        IHttpContextAccessor httpContextAccessor)
    {
        this.questionnaireUpdater = questionnaireUpdater;
        this.httpContextAccessor = httpContextAccessor;
    }

    public Questionnaire GetQuestionnaire()
    {
        var questionnaireString = httpContextAccessor.HttpContext!.Session.GetString(SessionKeyQuestionnaire);

        var questionnaire = questionnaireString == null
            ? new Questionnaire()
            : JsonSerializer.Deserialize<Questionnaire>(questionnaireString, JsonSerializerOptions);

        return questionnaire;
    }

    public Questionnaire UpdateGasBoiler(HasGasBoiler hasGasBoiler, QuestionFlowStep? entryPoint)
    {
        var questionnaire = GetQuestionnaire();
        questionnaire = questionnaireUpdater.UpdateGasBoiler(questionnaire, hasGasBoiler, entryPoint);
        SaveQuestionnaireToSession(questionnaire);
        return questionnaire;
    }

    public Questionnaire UpdateCountry(Country country, QuestionFlowStep? entryPoint)
    {
        var questionnaire = GetQuestionnaire();
        questionnaire = questionnaireUpdater.UpdateCountry(questionnaire, country, entryPoint);
        SaveQuestionnaireToSession(questionnaire);
        return questionnaire;
    }

    public Questionnaire UpdateOwnershipStatus(OwnershipStatus ownershipStatus, QuestionFlowStep? entryPoint)
    {
        var questionnaire = GetQuestionnaire();
        questionnaire = questionnaireUpdater.UpdateOwnershipStatus(questionnaire, ownershipStatus, entryPoint);
        SaveQuestionnaireToSession(questionnaire);
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
        SaveQuestionnaireToSession(questionnaire);
        return questionnaire;
    }

    public Questionnaire UpdateEpcIsCorrect(EpcConfirmation? epcIsCorrect, QuestionFlowStep? entryPoint)
    {
        var questionnaire = GetQuestionnaire();
        questionnaire = questionnaireUpdater.UpdateEpcIsCorrect(questionnaire, epcIsCorrect, entryPoint);
        SaveQuestionnaireToSession(questionnaire);
        return questionnaire;
    }

    public Questionnaire UpdateLocalAuthority(string custodianCode, QuestionFlowStep? entryPoint)
    {
        var questionnaire = GetQuestionnaire();
        if (questionnaire.CustodianCode != custodianCode)
        {
            questionnaire = questionnaireUpdater.UpdateAcknowledgedPending(questionnaire, false, entryPoint);
        }
        questionnaire = questionnaireUpdater.UpdateLocalAuthority(questionnaire, custodianCode, entryPoint);
        SaveQuestionnaireToSession(questionnaire);
        return questionnaire;
    }

    public Questionnaire UpdateLocalAuthorityIsCorrect(bool laIsCorrect, QuestionFlowStep? entryPoint)
    {
        var questionnaire = GetQuestionnaire();
        questionnaire = questionnaireUpdater.UpdateLocalAuthorityIsCorrect(questionnaire, laIsCorrect, entryPoint);
        SaveQuestionnaireToSession(questionnaire);
        return questionnaire;
    }
    
    public Questionnaire UpdateAcknowledgedPending(bool acknowledgedPending, QuestionFlowStep? entryPoint)
    {
        var questionnaire = GetQuestionnaire();
        questionnaire = questionnaireUpdater.UpdateAcknowledgedPending(questionnaire, acknowledgedPending, entryPoint);
        SaveQuestionnaireToSession(questionnaire);
        return questionnaire;
    }

    public Questionnaire UpdateHouseholdIncome(IncomeBand incomeBand, QuestionFlowStep? entryPoint)
    {
        var questionnaire = GetQuestionnaire();
        questionnaire = questionnaireUpdater.UpdateHouseholdIncome(questionnaire, incomeBand, entryPoint);
        SaveQuestionnaireToSession(questionnaire);
        return questionnaire;
    }

    public async Task<Questionnaire> GenerateReferralAsync(string name, string emailAddress, string telephone)
    {
        var questionnaire = GetQuestionnaire();
        questionnaire = await questionnaireUpdater.GenerateReferralAsync(questionnaire, name, emailAddress, telephone);
        SaveQuestionnaireToSession(questionnaire);
        return questionnaire;
    }

    public async Task<Questionnaire> ConfirmQuestionnaireAnswers()
    {
        var questionnaire = GetQuestionnaire();
        questionnaire = await questionnaireUpdater.GenerateAnonymisedReportAsync(questionnaire);
        SaveQuestionnaireToSession(questionnaire);
        return questionnaire;
    }

    public async Task<Questionnaire> RecordNotificationConsentAsync(bool consentGranted)
    {
        var questionnaire = GetQuestionnaire();
        questionnaire = await questionnaireUpdater.RecordNotificationConsentAsync(questionnaire, consentGranted);
        SaveQuestionnaireToSession(questionnaire);
        return questionnaire;
    }

    public async Task<Questionnaire> RecordNotificationConsentAsync(bool consentGranted, string emailAddress)
    {
        var questionnaire = GetQuestionnaire();
        questionnaire = await questionnaireUpdater.RecordNotificationConsentAsync(questionnaire, consentGranted, emailAddress);
        SaveQuestionnaireToSession(questionnaire);
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
        SaveQuestionnaireToSession(questionnaire);
        return questionnaire;
    }

    public void SaveQuestionnaireToSession(Questionnaire questionnaire)
    {
        var questionnaireString = JsonSerializer.Serialize(questionnaire, JsonSerializerOptions);
        httpContextAccessor.HttpContext!.Session.SetString(SessionKeyQuestionnaire, questionnaireString);
    }
}
