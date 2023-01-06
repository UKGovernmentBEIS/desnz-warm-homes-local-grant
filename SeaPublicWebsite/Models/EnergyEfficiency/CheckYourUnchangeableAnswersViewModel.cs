using SeaPublicWebsite.BusinessLogic.Models;

namespace SeaPublicWebsite.Models.EnergyEfficiency
{
    public class CheckYourUnchangeableAnswersViewModel : QuestionFlowViewModel
    {
        public string Reference { get; set; }
        public string ForwardLink { get; set; }
        public PropertyData PropertyData { get; set; }
    }
}