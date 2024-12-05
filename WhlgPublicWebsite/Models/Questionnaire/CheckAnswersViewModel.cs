using HerPublicWebsite.BusinessLogic.Models.Enums;
using HerPublicWebsite.BusinessLogic.Models;

namespace HerPublicWebsite.Models.Questionnaire;

public class CheckAnswersViewModel : QuestionFlowViewModel
{
    public BusinessLogic.Models.Questionnaire Questionnaire { get; set; }
}