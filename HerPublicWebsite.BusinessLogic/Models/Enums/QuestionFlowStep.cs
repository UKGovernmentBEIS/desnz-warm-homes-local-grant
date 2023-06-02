namespace HerPublicWebsite.BusinessLogic.Models.Enums;

public enum QuestionFlowStep
{
    Start,
    GasBoiler,
    DirectToEco,
    Country,
    ServiceUnsuitable,
    OwnershipStatus,
    OwnershipUnsuitable,
    Address,
    SelectAddress,
    ReviewEpc,
    ManualAddress,
    SelectLocalAuthority,
    ConfirmLocalAuthority,
    NotTakingPart,
    HouseholdIncome,
    CheckAnswers,
    Eligible,
    Confirmation,
    Ineligible,
    NoConsent
}
