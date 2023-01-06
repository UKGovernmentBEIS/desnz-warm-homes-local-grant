using HerPublicWebsite.BusinessLogic.Models.Enums;

namespace HerPublicWebsite.Models.EnergyEfficiency
{
    public class ServiceUnsuitableViewModel : QuestionFlowViewModel
    {
        public string Reference { get; set; }
        
        public Country? Country { get; set; }
    }
}