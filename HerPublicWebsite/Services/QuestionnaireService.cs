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
    
    public Questionnaire UpdateGasBoiler(HasGasBoiler hasGasBoiler)
    {
        var questionnaire = GetQuestionnaire();
        questionnaire = questionnaireUpdater.UpdateGasBoiler(questionnaire, hasGasBoiler);
        SaveQuestionnaireToSession(questionnaire);
        return questionnaire;
    }

    public Questionnaire UpdateCountry(Country country)
    {
        var questionnaire = GetQuestionnaire();
        questionnaire = questionnaireUpdater.UpdateCountry(questionnaire, country);
        SaveQuestionnaireToSession(questionnaire);
        return questionnaire;
    }
  
    public Questionnaire UpdateOwnershipStatus(OwnershipStatus ownershipStatus)
    {
        var questionnaire = GetQuestionnaire();
        questionnaire = questionnaireUpdater.UpdateOwnershipStatus(questionnaire, ownershipStatus);
        SaveQuestionnaireToSession(questionnaire);
        return questionnaire;
    }
    
    public async Task<Questionnaire> UpdateAddressAsync(Address address)

    {
        var questionnaire = GetQuestionnaire();
        questionnaire = await questionnaireUpdater.UpdateAddressAsync(questionnaire, address);
        SaveQuestionnaireToSession(questionnaire);
        return questionnaire;
    }

    public Questionnaire UpdateEpcIsCorrect(bool epcIsCorrect)
    {
        var questionnaire = GetQuestionnaire();
        questionnaire = questionnaireUpdater.UpdateEpcIsCorrect(questionnaire, epcIsCorrect);
        SaveQuestionnaireToSession(questionnaire);
        return questionnaire;
    }

    public Questionnaire UpdateLocalAuthority(string custodianCode)
    {
        var questionnaire = GetQuestionnaire();
        questionnaire = questionnaireUpdater.UpdateLocalAuthority(questionnaire, custodianCode);
        SaveQuestionnaireToSession(questionnaire);
        return questionnaire;
    }
    
    public Questionnaire UpdateLocalAuthorityIsCorrect(bool laIsCorrect)
    {
        var questionnaire = GetQuestionnaire();
        questionnaire = questionnaireUpdater.UpdateLocalAuthorityIsCorrect(questionnaire, laIsCorrect);
        SaveQuestionnaireToSession(questionnaire);
        return questionnaire;
    }
    
    public Questionnaire UpdateHouseholdIncome(IncomeBand incomeBand)
    {
        var questionnaire = GetQuestionnaire();
        questionnaire = questionnaireUpdater.UpdateHouseholdIncome(questionnaire, incomeBand);
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
    
    private void SaveQuestionnaireToSession(Questionnaire questionnaire)
    {
        var questionnaireString = JsonSerializer.Serialize(questionnaire, JsonSerializerOptions);
        httpContextAccessor.HttpContext!.Session.SetString(SessionKeyQuestionnaire, questionnaireString);
    }
}
