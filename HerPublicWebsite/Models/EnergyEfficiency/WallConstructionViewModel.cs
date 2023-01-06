using GovUkDesignSystem.Attributes.ValidationAttributes;
using HerPublicWebsite.BusinessLogic.Models;
using HerPublicWebsite.BusinessLogic.Models.Enums;

namespace HerPublicWebsite.Models.EnergyEfficiency
{
    public class WallConstructionViewModel : QuestionFlowViewModel
    {
        [GovUkValidateRequired(ErrorMessageIfMissing = "Select wall type")]
        public WallConstruction? WallConstruction { get; set; }
        
        public bool? HintSolidWalls { get; set; }

        public string Reference { get; set; }
        public Epc Epc { get; set; }
    }
}