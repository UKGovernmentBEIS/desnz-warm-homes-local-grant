using Microsoft.EntityFrameworkCore;
using HerPublicWebsite.BusinessLogic.Models;

namespace HerPublicWebsite.Data;

public class DataAccessProvider : IDataAccessProvider
{
    private readonly HerDbContext context;

    public DataAccessProvider(HerDbContext context)
    {
        this.context = context;
    }

    public async Task<Questionnaire> AddQuestionnaireAsync(Questionnaire questionnaire)
    {
        context.Questionnaires.Add(questionnaire);
        await context.SaveChangesAsync();
        return questionnaire;
    }

    public async Task UpdateQuestionnaireAsync(Questionnaire questionnaire)
    {
        context.Questionnaires.Update(questionnaire);
        await context.SaveChangesAsync();
    }

    public async Task<Questionnaire> GetQuestionnaireAsync(int id)
    {
        return await context.Questionnaires
            .Include(q => q.ContactDetails)
            .SingleOrDefaultAsync(q => q.QuestionnaireId == id);
    }
}