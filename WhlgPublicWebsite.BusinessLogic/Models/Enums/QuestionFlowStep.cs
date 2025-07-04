﻿namespace WhlgPublicWebsite.BusinessLogic.Models.Enums;

// WARNING: Do not re-order this enum without considering the impact on questionnaires stored
// in session data, and data that has been recorded in the database, as it may make that data
// invalid
public enum QuestionFlowStep
{
    Start,
    Country,
    IneligibleWales,
    IneligibleScotland,
    IneligibleNorthernIreland,
    OwnershipStatus,
    IneligibleTenure,
    Address,
    SelectAddress,
    ReviewEpc,
    ManualAddress,
    SelectLocalAuthority,
    ConfirmLocalAuthority,
    NoFunding,
    NotParticipating,
    NoLongerParticipating,
    TakingFutureReferrals,
    Pending,
    HouseholdIncome,
    CheckAnswers,
    Eligible,
    Confirmation,
    Ineligible,
    NoConsent
}