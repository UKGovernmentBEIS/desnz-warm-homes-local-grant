using HerPublicWebsite.BusinessLogic.Models;

namespace HerPublicWebsite.Models.EnergyEfficiency
{
    public class CheckYourUnchangeableAnswersViewModel : QuestionFlowViewModel
    {
        public string Reference { get; set; }
        public string ForwardLink { get; set; }
        public PropertyData PropertyData { get; set; }
    }
}