using HerPublicWebsite.BusinessLogic.Models.Enums;

namespace HerPublicWebsite.Models.EnergyEfficiency
{
    public abstract class QuestionFlowViewModel
    {
        public string BackLink { get; set; }
        public QuestionFlowStep? EntryPoint { get; set; }
        public string SkipLink { get; set; }
    }
}