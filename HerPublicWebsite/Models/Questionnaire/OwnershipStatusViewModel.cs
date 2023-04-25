using HerPublicWebsite.BusinessLogic.Models.Enums;

namespace HerPublicWebsite.Models.Questionnaire;

public record OwnershipStatusViewModel : QuestionFlowViewModel
{
    public OwnershipStatus? OwnershipStatus { get; set; }
}