using WhlgPublicWebsite.BusinessLogic.Models.Enums;

namespace WhlgPublicWebsite.Models.Questionnaire
{
    public abstract class QuestionFlowViewModel
    {
        public string BackLink { get; set; }
        public QuestionFlowStep? EntryPoint { get; set; }
    }
}
