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
                QuestionFlowStep.Country => CountryBackDestination(),
                QuestionFlowStep.ServiceUnsuitable => ServiceUnsuitableBackDestination(questionnaire),
                QuestionFlowStep.OwnershipStatus => OwnershipStatusBackDestination(),
                QuestionFlowStep.Address => AddressBackDestination(),
                QuestionFlowStep.SelectAddress => SelectAddressBackDestination(),
                QuestionFlowStep.ManualAddress => ManualAddressBackDestination(),
                QuestionFlowStep.GasBoiler => GasBoilerBackDestination(questionnaire),
                QuestionFlowStep.HouseholdIncome => HouseholdIncomeBackDestination(),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public QuestionFlowStep NextStep(QuestionFlowStep page, Questionnaire questionnaire, QuestionFlowStep? entryPoint = null)
        {
            return page switch
            {
                QuestionFlowStep.Country => CountryForwardDestination(questionnaire),
                QuestionFlowStep.OwnershipStatus => OwnershipStatusForwardDestination(questionnaire),
                QuestionFlowStep.Address => AddressForwardDestination(questionnaire),
                QuestionFlowStep.SelectAddress => SelectAddressForwardDestination(questionnaire),
                QuestionFlowStep.ManualAddress => ManualAddressForwardDestination(questionnaire),
                QuestionFlowStep.GasBoiler => GasBoilerForwardDestination(questionnaire),
                QuestionFlowStep.HouseholdIncome => HouseholdIncomeForwardDestination(questionnaire),
                _ => throw new ArgumentOutOfRangeException(nameof(page), page, null)
            };
        }

        private QuestionFlowStep CountryBackDestination()
        {
            return QuestionFlowStep.Start;
        }

        private QuestionFlowStep ServiceUnsuitableBackDestination(Questionnaire questionnaire)
        {
            return questionnaire switch
            {
                { Country: not Country.England }
                    => QuestionFlowStep.Country,
                { OwnershipStatus: not OwnershipStatus.OwnerOccupancy }
                    => QuestionFlowStep.OwnershipStatus,
                _ => throw new ArgumentOutOfRangeException()
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

        private QuestionFlowStep ManualAddressBackDestination()
        {
            return QuestionFlowStep.Address;
        }

        private QuestionFlowStep GasBoilerBackDestination(Questionnaire questionnaire)
        {
            return questionnaire.Uprn switch
            {
                null => QuestionFlowStep.ManualAddress,
                _ => QuestionFlowStep.Address
            };
        }

        private QuestionFlowStep HouseholdIncomeBackDestination()
        {
            return QuestionFlowStep.GasBoiler;
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
            return QuestionFlowStep.GasBoiler;
        }

        private QuestionFlowStep ManualAddressForwardDestination(Questionnaire questionnaire)
        {
            return QuestionFlowStep.GasBoiler;
        }

        private QuestionFlowStep GasBoilerForwardDestination(Questionnaire questionnaire)
        {
            return QuestionFlowStep.HouseholdIncome;
        }

        private QuestionFlowStep HouseholdIncomeForwardDestination(Questionnaire questionnaire)
        {
            return QuestionFlowStep.CheckAnswers;
        }
    }
}
