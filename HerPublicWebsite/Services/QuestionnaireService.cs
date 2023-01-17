using HerPublicWebsite.BusinessLogic;
using HerPublicWebsite.BusinessLogic.Models;
using HerPublicWebsite.BusinessLogic.Models.Enums;

namespace HerPublicWebsite.Services;

public class QuestionnaireService
{
    private readonly QuestionnaireUpdater questionnaireUpdater;
    
    public QuestionnaireService(QuestionnaireUpdater questionnaireUpdater)
    {
        this.questionnaireUpdater = questionnaireUpdater;
    }
    
    public Questionnaire GetQuestionnaire()
    {
        // TODO: Get the real data from the session
        return new Questionnaire();
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
        // TODO: Actually save the questionnaire
    }
}