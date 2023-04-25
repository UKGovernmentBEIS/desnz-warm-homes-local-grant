using HerPublicWebsite.BusinessLogic.Models.Enums;

namespace HerPublicWebsite.Models.Questionnaire
{
    public abstract record class QuestionFlowViewModel
    {
        public string BackLink { get; set; }
        public QuestionFlowStep? EntryPoint { get; set; }
    }
}