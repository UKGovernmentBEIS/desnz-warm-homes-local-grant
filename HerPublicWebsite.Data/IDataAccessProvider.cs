using HerPublicWebsite.BusinessLogic.Models;

namespace HerPublicWebsite.Data;

public interface IDataAccessProvider
{
    Task<Questionnaire> AddQuestionnaireAsync(Questionnaire questionnaire);
    Task UpdateQuestionnaireAsync(Questionnaire questionnaire);
    Task<Questionnaire> GetQuestionnaireAsync(int id);
}