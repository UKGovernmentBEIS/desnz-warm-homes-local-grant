using System.Collections.Generic;
using System.Linq;
using GovUkDesignSystem.Attributes.ValidationAttributes;
using GovUkDesignSystem.GovUkDesignSystemComponents;
using SeaPublicWebsite.BusinessLogic.Models;
using SeaPublicWebsite.BusinessLogic.Models.Enums;

namespace SeaPublicWebsite.Models.EnergyEfficiency
{
    public class RecommendationViewModel
    {
        [GovUkValidateRequired(ErrorMessageIfMissing = "Select what to do with this recommendation")]
        public RecommendationAction? RecommendationAction { get; set; }
        public int RecommendationIndex { get; set; }
        public List<PropertyRecommendation> PropertyRecommendations { get; set; }
        public bool FromActionPlan { get; set; }
        public string BackLink { get; set; }
        public TagViewModel DisruptionTagViewModel { get; set;  }
        public TagViewModel DurationTagViewModel { get; set;  }

        public PropertyRecommendation GetCurrentPropertyRecommendation() => PropertyRecommendations[RecommendationIndex];

        public bool HasPreviousIndex()
        {
            return RecommendationIndex > 0;
        }
        
        public bool HasNextIndex()
        {
            return RecommendationIndex < PropertyRecommendations.Count - 1;
        }

        public List<PropertyRecommendation> GetSavedRecommendations()
        {
            return PropertyRecommendations.Where(r => r.RecommendationAction == BusinessLogic.Models.Enums.RecommendationAction.SaveToActionPlan).ToList();
        }
        
        public string GetTotalInstallationCostText()
        {
            var minCost = GetSavedRecommendations().Sum(r => r.MinInstallCost);
            var maxCost = GetSavedRecommendations().Sum(r => r.MaxInstallCost);
            return $"£{minCost:N0} - £{maxCost:N0}";
        }

        public string GetTotalSavingText()
        {
            var saving = GetSavedRecommendations().Sum(r => r.Saving);
            return $"£{saving:N0} a year";
        }
    }
}