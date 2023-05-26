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
                QuestionFlowStep.ServiceUnsuitable => ServiceUnsuitableBackDestination(questionnaire),
                QuestionFlowStep.OwnershipStatus => OwnershipStatusBackDestination(entryPoint),
                QuestionFlowStep.Address => AddressBackDestination(entryPoint),
                QuestionFlowStep.SelectAddress => SelectAddressBackDestination(),
                QuestionFlowStep.ReviewEpc => ReviewEpcBackDestination(),
                QuestionFlowStep.ManualAddress => ManualAddressBackDestination(entryPoint),
                QuestionFlowStep.SelectLocalAuthority => SelectLocalAuthorityBackDestination(),
                QuestionFlowStep.ConfirmLocalAuthority => ConfirmLocalAuthorityBackDestination(),
                QuestionFlowStep.HouseholdIncome => HouseholdIncomeBackDestination(questionnaire, entryPoint),
                QuestionFlowStep.CheckAnswers => CheckAnswersBackDestination(questionnaire),
                QuestionFlowStep.Eligible => EligibleBackDestination(),
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
                QuestionFlowStep.HouseholdIncome => HouseholdIncomeForwardDestination(questionnaire),
                QuestionFlowStep.CheckAnswers => CheckAnswersForwardDestination(questionnaire),
                QuestionFlowStep.Eligible => EligibleForwardDestination(questionnaire),
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

        private QuestionFlowStep ServiceUnsuitableBackDestination(Questionnaire questionnaire)
        {
            return questionnaire switch
            {
                { Country: not Country.England }
                    => QuestionFlowStep.Country,
                { OwnershipStatus: not OwnershipStatus.OwnerOccupancy }
                    => QuestionFlowStep.OwnershipStatus,
                { EpcDetailsAreCorrect: true }
                    => QuestionFlowStep.ReviewEpc,
                // By using the browser back button a user can get to the service unsuitable page when their questionnaire
                // says that they are suitable. In that case we don't want to show them an error page, so set the back
                // link to just go to the start of the questionnaire.
                _ => QuestionFlowStep.GasBoiler
            };
        }

        private QuestionFlowStep OwnershipStatusBackDestination(QuestionFlowStep? entryPoint)
        {
            return entryPoint switch
            {
                QuestionFlowStep.OwnershipStatus => QuestionFlowStep.CheckAnswers,
                _ => QuestionFlowStep.Country
            };
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
            if (entryPoint == QuestionFlowStep.ManualAddress)
            {
                return QuestionFlowStep.CheckAnswers;
            }
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

        private QuestionFlowStep HouseholdIncomeBackDestination(Questionnaire questionnaire, QuestionFlowStep? entryPoint)
        {
            if (questionnaire.FoundEpcBandIsTooHigh)
            {
                return QuestionFlowStep.ReviewEpc;
            }
            return (entryPoint, questionnaire.Uprn) switch
            {
                (QuestionFlowStep.HouseholdIncome, _) => QuestionFlowStep.CheckAnswers,
                (_, null) => QuestionFlowStep.ConfirmLocalAuthority,
                _ => QuestionFlowStep.Address
            };
        }

        private QuestionFlowStep CheckAnswersBackDestination(Questionnaire questionnaire)
        {
            return QuestionFlowStep.HouseholdIncome;
        }

        private QuestionFlowStep EligibleBackDestination()
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
                (_, not Country.England) => QuestionFlowStep.ServiceUnsuitable,
                (QuestionFlowStep.Country, _) => QuestionFlowStep.CheckAnswers,
                _ => QuestionFlowStep.OwnershipStatus
            };
        }

        private QuestionFlowStep OwnershipStatusForwardDestination(Questionnaire questionnaire, QuestionFlowStep? entryPoint)
        {
            return (entryPoint, questionnaire.OwnershipStatus) switch
            {
                (_, not OwnershipStatus.OwnerOccupancy) => QuestionFlowStep.ServiceUnsuitable,
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
            return (entryPoint, questionnaire.FoundEpcBandIsTooHigh) switch
            {
                (_, true) => QuestionFlowStep.ReviewEpc,
                (QuestionFlowStep.Address, _) => QuestionFlowStep.CheckAnswers,
                _ => QuestionFlowStep.HouseholdIncome
            };
        }

        private QuestionFlowStep ReviewEpcForwardDestination(Questionnaire questionnaire, QuestionFlowStep? entryPoint)
        {
            return (entryPoint, questionnaire.EpcDetailsAreCorrect!.Value) switch
            {
                (_, true) => QuestionFlowStep.ServiceUnsuitable,
                (QuestionFlowStep.Address, _) => QuestionFlowStep.CheckAnswers,
                _ => QuestionFlowStep.HouseholdIncome
            };
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
            return (entryPoint, questionnaire.LocalAuthorityConfirmed) switch
            {
                (_, not true) => QuestionFlowStep.SelectLocalAuthority,
                (QuestionFlowStep.Address, _) => QuestionFlowStep.CheckAnswers,
                _ => QuestionFlowStep.HouseholdIncome
            };
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
    }
}
