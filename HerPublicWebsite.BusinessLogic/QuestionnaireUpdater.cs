using HerPublicWebsite.BusinessLogic.Models;
using HerPublicWebsite.BusinessLogic.Models.Enums;

namespace HerPublicWebsite.BusinessLogic;

public class QuestionnaireUpdater
{
    public Questionnaire UpdateCountry(Questionnaire questionnaire, Country country)
    {
        questionnaire.Country = country;
        return questionnaire;
    }
}
