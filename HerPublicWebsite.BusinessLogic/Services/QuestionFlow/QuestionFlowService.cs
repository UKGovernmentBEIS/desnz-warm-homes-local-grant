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
                QuestionFlowStep.ManualAddress => ManualAddressBackDestination(),
                QuestionFlowStep.HouseholdIncome => HouseholdIncomeBackDestination(questionnaire, entryPoint),
                QuestionFlowStep.CheckAnswers => CheckAnswersBackDestination(questionnaire),
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
                QuestionFlowStep.ManualAddress => ManualAddressForwardDestination(questionnaire, entryPoint),
                QuestionFlowStep.HouseholdIncome => HouseholdIncomeForwardDestination(questionnaire),
                QuestionFlowStep.CheckAnswers => CheckAnswersForwardDestination(questionnaire),
                _ => throw new ArgumentOutOfRangeException(nameof(page), page, null)
            };
        }

        private QuestionFlowStep GasBoilerBackDestination(QuestionFlowStep? entryPoint)
        {
            return entryPoint switch
            {
                QuestionFlowStep.CheckAnswers => QuestionFlowStep.CheckAnswers,
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
                QuestionFlowStep.CheckAnswers => QuestionFlowStep.CheckAnswers,
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
                QuestionFlowStep.CheckAnswers => QuestionFlowStep.CheckAnswers,
                _ => QuestionFlowStep.Country
            };
        }

        private QuestionFlowStep AddressBackDestination(QuestionFlowStep? entryPoint)
        {

            return entryPoint switch
            {
                QuestionFlowStep.CheckAnswers => QuestionFlowStep.CheckAnswers,
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

        private QuestionFlowStep ManualAddressBackDestination()
        {
            return QuestionFlowStep.Address;
        }

        private QuestionFlowStep HouseholdIncomeBackDestination(Questionnaire questionnaire, QuestionFlowStep? entryPoint)
        {
            return (entryPoint, questionnaire.Uprn) switch
            {
                (QuestionFlowStep.CheckAnswers, _) => QuestionFlowStep.CheckAnswers,
                (_, null) => QuestionFlowStep.ManualAddress,
                _ => QuestionFlowStep.Address
            };
        }

        private QuestionFlowStep CheckAnswersBackDestination(Questionnaire questionnaire)
        {
            return QuestionFlowStep.HouseholdIncome;
        }

        private QuestionFlowStep GasBoilerForwardDestination(Questionnaire questionnaire, QuestionFlowStep? entryPoint)
        {
            return (entryPoint, questionnaire.HasGasBoiler) switch
            {
                (_, HasGasBoiler.Yes) => QuestionFlowStep.DirectToEco,
                (QuestionFlowStep.CheckAnswers, _) => QuestionFlowStep.CheckAnswers,
                _ => QuestionFlowStep.Country
            };
        }

        private QuestionFlowStep CountryForwardDestination(Questionnaire questionnaire, QuestionFlowStep? entryPoint)
        {
            return (entryPoint, questionnaire.Country) switch
            {
                (_, not Country.England) => QuestionFlowStep.ServiceUnsuitable,
                (QuestionFlowStep.CheckAnswers, _) => QuestionFlowStep.CheckAnswers,
                _ => QuestionFlowStep.OwnershipStatus
            };
        }

        private QuestionFlowStep OwnershipStatusForwardDestination(Questionnaire questionnaire, QuestionFlowStep? entryPoint)
        {
            return (entryPoint, questionnaire.OwnershipStatus) switch
            {
                (_, not OwnershipStatus.OwnerOccupancy) => QuestionFlowStep.ServiceUnsuitable,
                (QuestionFlowStep.CheckAnswers, _) => QuestionFlowStep.CheckAnswers,
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
                (QuestionFlowStep.CheckAnswers, _) => QuestionFlowStep.CheckAnswers,
                _ => QuestionFlowStep.HouseholdIncome
            };
        }

        private QuestionFlowStep ReviewEpcForwardDestination(Questionnaire questionnaire, QuestionFlowStep? entryPoint)
        {
            return (entryPoint, questionnaire.EpcDetailsAreCorrect!.Value) switch
            {
                (_, true) => QuestionFlowStep.ServiceUnsuitable,
                (QuestionFlowStep.CheckAnswers, _) => QuestionFlowStep.CheckAnswers,
                _ => QuestionFlowStep.HouseholdIncome
            };
        }

        private QuestionFlowStep ManualAddressForwardDestination(Questionnaire questionnaire, QuestionFlowStep? entryPoint)
        {
            return entryPoint switch
            {
                QuestionFlowStep.CheckAnswers => QuestionFlowStep.CheckAnswers,
                _ => QuestionFlowStep.HouseholdIncome
            };
        }

        private QuestionFlowStep HouseholdIncomeForwardDestination(Questionnaire questionnaire)
        {
            return QuestionFlowStep.CheckAnswers;
        }

        private QuestionFlowStep CheckAnswersForwardDestination(Questionnaire questionnaire)
        {
            return (questionnaire.IncomeBand, questionnaire.HasGasBoiler, questionnaire.FoundEpcBandIsTooHigh, questionnaire.EpcDetailsAreCorrect, questionnaire.Country, questionnaire.OwnershipStatus, questionnaire.IsLsoaProperty) switch
            {
                (IncomeBand.UnderOrEqualTo31000, not HasGasBoiler.Yes, false, _, Country.England, OwnershipStatus.OwnerOccupancy, _) => QuestionFlowStep.Eligible,
                (IncomeBand.UnderOrEqualTo31000, not HasGasBoiler.Yes, _, false, Country.England, OwnershipStatus.OwnerOccupancy, _) => QuestionFlowStep.Eligible,
                (IncomeBand.GreaterThan31000, not HasGasBoiler.Yes, false, _, Country.England, OwnershipStatus.OwnerOccupancy, true) => QuestionFlowStep.Eligible,
                (IncomeBand.GreaterThan31000, not HasGasBoiler.Yes, _, false, Country.England, OwnershipStatus.OwnerOccupancy, true) => QuestionFlowStep.Eligible,
                _ => QuestionFlowStep.Ineligible,
            };
        }
    }
}
