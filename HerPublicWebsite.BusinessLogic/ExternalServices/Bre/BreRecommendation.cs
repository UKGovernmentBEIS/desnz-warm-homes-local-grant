using HerPublicWebsite.BusinessLogic.Models.Enums;

namespace HerPublicWebsite.BusinessLogic.ExternalServices.Bre
{
    public class BreRecommendation
    {
        public RecommendationKey Key { get; set; }
        public string Title { get; set; }
        public int MinInstallCost { get; set; }
        public int MaxInstallCost { get; set; }
        public int Saving { get; set; }
        public int LifetimeSaving { get; set; }
        public int Lifetime { get; set; }
        public string Summary { get; set; }
    }
}
