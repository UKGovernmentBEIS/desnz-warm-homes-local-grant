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
                QuestionFlowStep.GasBoiler => GasBoilerBackDestination(),
                QuestionFlowStep.DirectToEco => DirectToEcoBackDestination(),
                QuestionFlowStep.Country => CountryBackDestination(),
                QuestionFlowStep.ServiceUnsuitable => ServiceUnsuitableBackDestination(questionnaire),
                QuestionFlowStep.OwnershipStatus => OwnershipStatusBackDestination(),
                QuestionFlowStep.Address => AddressBackDestination(),
                QuestionFlowStep.SelectAddress => SelectAddressBackDestination(),
                QuestionFlowStep.ReviewEpc => ReviewEpcBackDestination(),
                QuestionFlowStep.ManualAddress => ManualAddressBackDestination(),
                QuestionFlowStep.HouseholdIncome => HouseholdIncomeBackDestination(questionnaire),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public QuestionFlowStep NextStep(QuestionFlowStep page, Questionnaire questionnaire, QuestionFlowStep? entryPoint = null)
        {
            return page switch
            {
                QuestionFlowStep.GasBoiler => GasBoilerForwardDestination(questionnaire),
                QuestionFlowStep.Country => CountryForwardDestination(questionnaire),
                QuestionFlowStep.OwnershipStatus => OwnershipStatusForwardDestination(questionnaire),
                QuestionFlowStep.Address => AddressForwardDestination(questionnaire),
                QuestionFlowStep.SelectAddress => SelectAddressForwardDestination(questionnaire),
                QuestionFlowStep.ReviewEpc => ReviewEpcForwardDestination(questionnaire),
                QuestionFlowStep.ManualAddress => ManualAddressForwardDestination(questionnaire),
                _ => throw new ArgumentOutOfRangeException(nameof(page), page, null)
            };
        }
        
        private QuestionFlowStep GasBoilerBackDestination()
        {
            return QuestionFlowStep.Start;
        }

        private QuestionFlowStep DirectToEcoBackDestination()
        {
            return QuestionFlowStep.GasBoiler;
        }

        private QuestionFlowStep CountryBackDestination()
        {
            return QuestionFlowStep.GasBoiler;
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

        private QuestionFlowStep OwnershipStatusBackDestination()
        {
            return QuestionFlowStep.Country;
        }

        private QuestionFlowStep AddressBackDestination()
        {
            return QuestionFlowStep.OwnershipStatus;
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

        private QuestionFlowStep HouseholdIncomeBackDestination(Questionnaire questionnaire)
        {
            return questionnaire.Uprn switch
            {
                null => QuestionFlowStep.ManualAddress,
                _ => QuestionFlowStep.Address
            };
        }
        
        private QuestionFlowStep GasBoilerForwardDestination(Questionnaire questionnaire)
        {
            return questionnaire.HasGasBoiler is HasGasBoiler.Yes
                ? QuestionFlowStep.DirectToEco
                : QuestionFlowStep.Country;
        }

        private QuestionFlowStep CountryForwardDestination(Questionnaire questionnaire)
        {
            return questionnaire.Country is not Country.England
                ? QuestionFlowStep.ServiceUnsuitable
                : QuestionFlowStep.OwnershipStatus;
        }

        private QuestionFlowStep OwnershipStatusForwardDestination(Questionnaire questionnaire)
        {
            return questionnaire.OwnershipStatus is not OwnershipStatus.OwnerOccupancy
                ? QuestionFlowStep.ServiceUnsuitable
                : QuestionFlowStep.Address;
        }

        private QuestionFlowStep AddressForwardDestination(Questionnaire questionnaire)
        {
            return QuestionFlowStep.SelectAddress;
        }

        private QuestionFlowStep SelectAddressForwardDestination(Questionnaire questionnaire)
        {
            return questionnaire.FoundEpcBandIsTooHigh ? QuestionFlowStep.ReviewEpc : QuestionFlowStep.HouseholdIncome;
        }

        private QuestionFlowStep ReviewEpcForwardDestination(Questionnaire questionnaire)
        {
            return questionnaire.EpcDetailsAreCorrect!.Value ? QuestionFlowStep.ServiceUnsuitable : QuestionFlowStep.HouseholdIncome;
        }

        private QuestionFlowStep ManualAddressForwardDestination(Questionnaire questionnaire)
        {
            return QuestionFlowStep.HouseholdIncome;
        }
    }
}
