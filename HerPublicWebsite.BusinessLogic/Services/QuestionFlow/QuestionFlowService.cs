using HerPublicWebsite.BusinessLogic.Models;
using HerPublicWebsite.BusinessLogic.Models.Enums;

namespace HerPublicWebsite.BusinessLogic.Services.QuestionFlow
{
    public interface IQuestionFlowService
    {
        public QuestionFlowStep PreviousStep(QuestionFlowStep page, Questionnaire questionnaire, QuestionFlowStep? entryPoint = null);

        public QuestionFlowStep NextStep(QuestionFlowStep page, Questionnaire questionnaire, QuestionFlowStep? entryPoint = null);
    }

    public class QuestionFlowService : IQuestionFlowService
    {
        public QuestionFlowStep PreviousStep(
            QuestionFlowStep page,
            Questionnaire questionnaire,
            QuestionFlowStep? entryPoint = null)
        {
            return page switch
            {
                QuestionFlowStep.GasBoiler => GasBoilerBackDestination(entryPoint),
                QuestionFlowStep.DirectToEco => DirectToEcoBackDestination(),
                QuestionFlowStep.Country => CountryBackDestination(entryPoint),
                QuestionFlowStep.IneligibleWales => IneligibleWalesBackDestination(),
                QuestionFlowStep.IneligibleScotland => IneligibleScotlandBackDestination(),
                QuestionFlowStep.IneligibleNorthernIreland => IneligibleNorthernIrelandBackDestination(),
                QuestionFlowStep.OwnershipStatus => OwnershipStatusBackDestination(entryPoint),
                QuestionFlowStep.IneligibleTenure => IneligibleTenureBackDestination(),
                QuestionFlowStep.Address => AddressBackDestination(entryPoint),
                QuestionFlowStep.SelectAddress => SelectAddressBackDestination(),
                QuestionFlowStep.ReviewEpc => ReviewEpcBackDestination(),
                QuestionFlowStep.NotParticipating => NotParticipatingBackDestination(questionnaire),
                QuestionFlowStep.NotTakingPart => NotTakingPartBackDestination(questionnaire),
                QuestionFlowStep.NoLongerParticipating => NoLongerParticipatingBackDestination(questionnaire),
                QuestionFlowStep.Pending => PendingBackDestination(questionnaire),
                QuestionFlowStep.ManualAddress => ManualAddressBackDestination(entryPoint),
                QuestionFlowStep.SelectLocalAuthority => SelectLocalAuthorityBackDestination(),
                QuestionFlowStep.ConfirmLocalAuthority => ConfirmLocalAuthorityBackDestination(),
                QuestionFlowStep.HouseholdIncome => HouseholdIncomeBackDestination(questionnaire, entryPoint),
                QuestionFlowStep.CheckAnswers => CheckAnswersBackDestination(questionnaire),
                QuestionFlowStep.Eligible => EligibleBackDestination(),
                QuestionFlowStep.Confirmation => ConfirmationBackDestination(),
                QuestionFlowStep.NoConsent => NoConsentBackDestination(),
                QuestionFlowStep.Ineligible => IneligibleBackDestination(),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public QuestionFlowStep NextStep(QuestionFlowStep page, Questionnaire questionnaire, QuestionFlowStep? entryPoint = null)
        {
            return page switch
            {
                QuestionFlowStep.GasBoiler => GasBoilerForwardDestination(questionnaire, entryPoint),
                QuestionFlowStep.Country => CountryForwardDestination(questionnaire, entryPoint),
                QuestionFlowStep.OwnershipStatus => OwnershipStatusForwardDestination(questionnaire, entryPoint),
                QuestionFlowStep.Address => AddressForwardDestination(questionnaire),
                QuestionFlowStep.SelectAddress => SelectAddressForwardDestination(questionnaire, entryPoint),
                QuestionFlowStep.ReviewEpc => ReviewEpcForwardDestination(questionnaire, entryPoint),
                QuestionFlowStep.ManualAddress => ManualAddressForwardDestination(questionnaire),
                QuestionFlowStep.SelectLocalAuthority => SelectLocalAuthorityForwardDestination(questionnaire),
                QuestionFlowStep.ConfirmLocalAuthority => ConfirmLocalAuthorityForwardDestination(questionnaire, entryPoint),
                QuestionFlowStep.NotParticipating => NotParticipatingForwardDestination(questionnaire),
                QuestionFlowStep.NotTakingPart => NotTakingPartForwardDestination(questionnaire),
                QuestionFlowStep.NoLongerParticipating => NoLongerParticipatingForwardDestination(questionnaire),
                QuestionFlowStep.Pending => PendingForwardDestination(questionnaire, entryPoint),
                QuestionFlowStep.HouseholdIncome => HouseholdIncomeForwardDestination(questionnaire),
                QuestionFlowStep.CheckAnswers => CheckAnswersForwardDestination(questionnaire),
                QuestionFlowStep.Eligible => EligibleForwardDestination(questionnaire),
                QuestionFlowStep.Confirmation => ConfirmationForwardDestination(questionnaire),
                QuestionFlowStep.Ineligible => IneligibleForwardDestination(questionnaire),
                _ => throw new ArgumentOutOfRangeException(nameof(page), page, null)
            };
        }

        private QuestionFlowStep GasBoilerBackDestination(QuestionFlowStep? entryPoint)
        {
            return entryPoint switch
            {
                QuestionFlowStep.GasBoiler => QuestionFlowStep.CheckAnswers,
                _ => QuestionFlowStep.Start
            };
        }

        private QuestionFlowStep DirectToEcoBackDestination()
        {
            return QuestionFlowStep.GasBoiler;
        }

        private QuestionFlowStep CountryBackDestination(QuestionFlowStep? entryPoint)
        {
            return entryPoint switch
            {
                QuestionFlowStep.Country => QuestionFlowStep.CheckAnswers,
                _ => QuestionFlowStep.GasBoiler
            };
        }

        private QuestionFlowStep IneligibleWalesBackDestination()
        {
            return QuestionFlowStep.Country;
        }

        private QuestionFlowStep IneligibleScotlandBackDestination()
        {
            return QuestionFlowStep.Country;
        }

        private QuestionFlowStep IneligibleNorthernIrelandBackDestination()
        {
            return QuestionFlowStep.Country;
        }

        private QuestionFlowStep OwnershipStatusBackDestination(QuestionFlowStep? entryPoint)
        {
            return entryPoint switch
            {
                QuestionFlowStep.OwnershipStatus => QuestionFlowStep.CheckAnswers,
                _ => QuestionFlowStep.Country
            };
        }

        private QuestionFlowStep IneligibleTenureBackDestination()
        {
            return QuestionFlowStep.OwnershipStatus;
        }

        private QuestionFlowStep AddressBackDestination(QuestionFlowStep? entryPoint)
        {

            return entryPoint switch
            {
                QuestionFlowStep.Address => QuestionFlowStep.CheckAnswers,
                _ => QuestionFlowStep.OwnershipStatus
            };
        }

        private QuestionFlowStep SelectAddressBackDestination()
        {
            return QuestionFlowStep.Address;
        }

        private QuestionFlowStep ReviewEpcBackDestination()
        {
            return QuestionFlowStep.Address;
        }

        private QuestionFlowStep ManualAddressBackDestination(QuestionFlowStep? entryPoint)
        {
            return QuestionFlowStep.Address;
        }

        private QuestionFlowStep SelectLocalAuthorityBackDestination()
        {
            return QuestionFlowStep.ManualAddress;
        }

        private QuestionFlowStep ConfirmLocalAuthorityBackDestination()
        {
            return QuestionFlowStep.SelectLocalAuthority;
        }

        private QuestionFlowStep NotTakingPartBackDestination(Questionnaire questionnaire)
        {
            return questionnaire.Uprn switch
            {
                null => QuestionFlowStep.ConfirmLocalAuthority,
                _ => QuestionFlowStep.Address
            };
        }
        
        private QuestionFlowStep NotParticipatingBackDestination(Questionnaire questionnaire)
        {
            return questionnaire.Uprn switch
            {
                null => QuestionFlowStep.ConfirmLocalAuthority,
                _ => QuestionFlowStep.Address
            };
        }
        
        private QuestionFlowStep NoLongerParticipatingBackDestination(Questionnaire questionnaire)
        {
            return questionnaire.Uprn switch
            {
                null => QuestionFlowStep.ConfirmLocalAuthority,
                _ => QuestionFlowStep.Address
            };
        }
        
        private QuestionFlowStep PendingBackDestination(Questionnaire questionnaire)
        {
            if (questionnaire.FoundEpcBandIsTooHigh)
            {
                return QuestionFlowStep.ReviewEpc;
            }
            else if (questionnaire.Uprn is null)
            {
                return QuestionFlowStep.ConfirmLocalAuthority;
            }

            return QuestionFlowStep.Address;
        }

        private QuestionFlowStep HouseholdIncomeBackDestination(Questionnaire questionnaire, QuestionFlowStep? entryPoint)
        {
            if (entryPoint is QuestionFlowStep.HouseholdIncome)
            {
                return QuestionFlowStep.CheckAnswers;
            }
            else if (questionnaire.LocalAuthorityHug2Status == LocalAuthorityData.Hug2Status.Pending)
            {
                return QuestionFlowStep.Pending;
            }
            else if (questionnaire.FoundEpcBandIsTooHigh)
            {
                return QuestionFlowStep.ReviewEpc;
            }
            else if (questionnaire.Uprn is null)
            {
                return QuestionFlowStep.ConfirmLocalAuthority;
            }

            return QuestionFlowStep.Address;
        }

        private QuestionFlowStep CheckAnswersBackDestination(Questionnaire questionnaire)
        {
            return QuestionFlowStep.HouseholdIncome;
        }

        private QuestionFlowStep EligibleBackDestination()
        {
            return QuestionFlowStep.CheckAnswers;

        }

        private QuestionFlowStep NoConsentBackDestination()
        {
            return QuestionFlowStep.Eligible;
        }

        private QuestionFlowStep ConfirmationBackDestination()
        {
            return QuestionFlowStep.Eligible;
        }

        private QuestionFlowStep IneligibleBackDestination()
        {
            return QuestionFlowStep.CheckAnswers;

        }

        private QuestionFlowStep GasBoilerForwardDestination(Questionnaire questionnaire, QuestionFlowStep? entryPoint)
        {
            return (entryPoint, questionnaire.HasGasBoiler) switch
            {
                (_, HasGasBoiler.Yes) => QuestionFlowStep.DirectToEco,
                (QuestionFlowStep.GasBoiler, _) => QuestionFlowStep.CheckAnswers,
                _ => QuestionFlowStep.Country
            };
        }

        private QuestionFlowStep CountryForwardDestination(Questionnaire questionnaire, QuestionFlowStep? entryPoint)
        {
            return (entryPoint, questionnaire.Country) switch
            {
                (_, Country.Wales) => QuestionFlowStep.IneligibleWales,
                (_, Country.Scotland) => QuestionFlowStep.IneligibleScotland,
                (_, Country.NorthernIreland) => QuestionFlowStep.IneligibleNorthernIreland,
                (QuestionFlowStep.Country, _) => QuestionFlowStep.CheckAnswers,
                _ => QuestionFlowStep.OwnershipStatus
            };
        }

        private QuestionFlowStep OwnershipStatusForwardDestination(Questionnaire questionnaire, QuestionFlowStep? entryPoint)
        {
            return (entryPoint, questionnaire.OwnershipStatus) switch
            {
                (_, not OwnershipStatus.OwnerOccupancy) => QuestionFlowStep.IneligibleTenure,
                (QuestionFlowStep.OwnershipStatus, _) => QuestionFlowStep.CheckAnswers,
                _ => QuestionFlowStep.Address
            };
        }

        private QuestionFlowStep AddressForwardDestination(Questionnaire questionnaire)
        {
            return QuestionFlowStep.SelectAddress;
        }

        private QuestionFlowStep SelectAddressForwardDestination(Questionnaire questionnaire, QuestionFlowStep? entryPoint)
        {
            if (questionnaire.LocalAuthorityHug2Status is LocalAuthorityData.Hug2Status.NotTakingPart)
            {
                return QuestionFlowStep.NotTakingPart;
            }
            else if (questionnaire.LocalAuthorityHug2Status is LocalAuthorityData.Hug2Status.NotParticipating)
            {
                return QuestionFlowStep.NotParticipating;
            }
            else if (questionnaire.LocalAuthorityHug2Status is LocalAuthorityData.Hug2Status.NoLongerParticipating)
            {
                return QuestionFlowStep.NoLongerParticipating;
            }
            else if (questionnaire.FoundEpcBandIsTooHigh)
            {
                return QuestionFlowStep.ReviewEpc;
            }
            else if (questionnaire.LocalAuthorityHug2Status is LocalAuthorityData.Hug2Status.Pending)
            {
                return QuestionFlowStep.Pending;
            }
            // If the LA has changed and the income band the user selected previously is no longer valid then we don't
            // go back to the check your answers page as the user will need to select a new income band.
            else if (entryPoint is QuestionFlowStep.Address && questionnaire.IncomeBandIsValid)
            {
                return QuestionFlowStep.CheckAnswers;
            }

            return QuestionFlowStep.HouseholdIncome;
        }

        private QuestionFlowStep ReviewEpcForwardDestination(Questionnaire questionnaire, QuestionFlowStep? entryPoint)
        {
            if (questionnaire.LocalAuthorityHug2Status is LocalAuthorityData.Hug2Status.Pending)
            {
                return QuestionFlowStep.Pending;
            }
            // If the LA has changed and the income band the user selected previously is no longer valid then we don't
            // go back to the check your answers page as the user will need to select a new income band.
            else if (entryPoint is QuestionFlowStep.Address && questionnaire.IncomeBandIsValid)
            {
                return QuestionFlowStep.CheckAnswers;
            }

            return QuestionFlowStep.HouseholdIncome;
        }

        private QuestionFlowStep ManualAddressForwardDestination(Questionnaire questionnaire)
        {
            return QuestionFlowStep.SelectLocalAuthority;
        }

        private QuestionFlowStep SelectLocalAuthorityForwardDestination(Questionnaire questionnaire)
        {
            return QuestionFlowStep.ConfirmLocalAuthority;
        }

        private QuestionFlowStep ConfirmLocalAuthorityForwardDestination(Questionnaire questionnaire, QuestionFlowStep? entryPoint)
        {
            if (questionnaire.LocalAuthorityConfirmed is not true)
            {
                return QuestionFlowStep.SelectLocalAuthority;
            }
            else if (questionnaire.LocalAuthorityHug2Status is LocalAuthorityData.Hug2Status.NotTakingPart)
            {
                return QuestionFlowStep.NotTakingPart;
            }
            else if (questionnaire.LocalAuthorityHug2Status is LocalAuthorityData.Hug2Status.NoLongerParticipating)
            {
                return QuestionFlowStep.NoLongerParticipating;
            }
            else if (questionnaire.LocalAuthorityHug2Status is LocalAuthorityData.Hug2Status.NotParticipating)
            {
                return QuestionFlowStep.NotParticipating;
            }
            else if (questionnaire.LocalAuthorityHug2Status is LocalAuthorityData.Hug2Status.Pending)
            {
                return QuestionFlowStep.Pending;
            }
            // If the LA has changed and the income band the user selected previously is no longer valid then we don't
            // go back to the check your answers page as the user will need to select a new income band.
            else if (entryPoint is QuestionFlowStep.Address && questionnaire.IncomeBandIsValid)
            {
                return QuestionFlowStep.CheckAnswers;
            }

            return QuestionFlowStep.HouseholdIncome;
        }

        private QuestionFlowStep NotTakingPartForwardDestination(Questionnaire questionnaire)
        {
            return QuestionFlowStep.NotTakingPart;
        }
        
        private QuestionFlowStep NotParticipatingForwardDestination(Questionnaire questionnaire)
        {
            return QuestionFlowStep.NotParticipating;
        }
        
        private QuestionFlowStep NoLongerParticipatingForwardDestination(Questionnaire questionnaire)
        {
            return QuestionFlowStep.NoLongerParticipating;
        }
        
        private QuestionFlowStep PendingForwardDestination(Questionnaire questionnaire, QuestionFlowStep? entryPoint)
        {
            // If the LA has changed and the income band the user selected previously is no longer valid then we don't
            // go back to the check your answers page as the user will need to select a new income band.
            if (entryPoint is QuestionFlowStep.Address && questionnaire.IncomeBandIsValid)
            {
                return QuestionFlowStep.CheckAnswers;
            }
            return QuestionFlowStep.HouseholdIncome;
        }

        private QuestionFlowStep HouseholdIncomeForwardDestination(Questionnaire questionnaire)
        {
            return QuestionFlowStep.CheckAnswers;
        }

        private QuestionFlowStep CheckAnswersForwardDestination(Questionnaire questionnaire)
        {
            return questionnaire.IsEligibleForHug2 ? QuestionFlowStep.Eligible : QuestionFlowStep.Ineligible;
        }

        private QuestionFlowStep EligibleForwardDestination(Questionnaire questionnaire)
        {
            return QuestionFlowStep.Confirmation;
        }

        private QuestionFlowStep ConfirmationForwardDestination(Questionnaire questionnaire)
        {
            return QuestionFlowStep.Confirmation;
        }

        private QuestionFlowStep IneligibleForwardDestination(Questionnaire questionnaire)
        {
            return QuestionFlowStep.Ineligible;
        }
    }
}
