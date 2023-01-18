using System.Text.Json;
using System.Text.Json.Serialization;
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
    
    public Questionnaire UpdateCountry(Country country)
    {
        var questionnaire = GetQuestionnaire();
        questionnaire = questionnaireUpdater.UpdateCountry(questionnaire, country);
        SaveQuestionnaireToSession(questionnaire);
        return questionnaire;
    }

    private void SaveQuestionnaireToSession(Questionnaire questionnaire)
    {
        var questionnaireString = JsonSerializer.Serialize(questionnaire, JsonSerializerOptions);
        httpContextAccessor.HttpContext!.Session.SetString(SessionKeyQuestionnaire, questionnaireString);
    }
}