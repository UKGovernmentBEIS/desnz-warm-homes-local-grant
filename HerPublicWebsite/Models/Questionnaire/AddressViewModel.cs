using GovUkDesignSystem.Attributes.ValidationAttributes;

namespace HerPublicWebsite.Models.Questionnaire;

public record AddressViewModel: QuestionFlowViewModel
{
    public string BuildingNameOrNumber { get; set; }
    public string Postcode { get; set; }
}
