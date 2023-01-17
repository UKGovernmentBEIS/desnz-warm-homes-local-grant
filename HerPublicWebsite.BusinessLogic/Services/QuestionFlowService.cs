using HerPublicWebsite.BusinessLogic.Models;
using HerPublicWebsite.BusinessLogic.Models.Enums;

namespace HerPublicWebsite.BusinessLogic.Services
{
    public interface IQuestionFlowService
    { 
        public QuestionFlowStep PreviousStep(QuestionFlowStep page, Questionnaire questionnaire, QuestionFlowStep? entryPoint = null);
        
        public QuestionFlowStep NextStep(QuestionFlowStep page, Questionnaire questionnaire, QuestionFlowStep? entryPoint = null);
    }

    // TODO: Add tests
    public class QuestionFlowService: IQuestionFlowService
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
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public QuestionFlowStep NextStep(QuestionFlowStep page, Questionnaire questionnaire, QuestionFlowStep? entryPoint = null)
        {
            return page switch
            {
                QuestionFlowStep.Country => CountryForwardDestination(questionnaire),
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

        private QuestionFlowStep CountryForwardDestination(Questionnaire questionnaire)
        {
            return questionnaire.Country is not Country.England 
                ? QuestionFlowStep.ServiceUnsuitable
                : QuestionFlowStep.OwnershipStatus;
        }
    }
}