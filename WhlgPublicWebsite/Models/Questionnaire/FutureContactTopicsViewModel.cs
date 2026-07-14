namespace WhlgPublicWebsite.Models.Questionnaire;

public class FutureContactTopicsViewModel : QuestionFlowViewModel
{
    public bool ConsentToGrants { get; set; }
    public bool ConsentToAdvice { get; set; }
    public bool ConsentToUpdates { get; set; }

    public string SkipUrl { get; set; }

    public bool AtLeastOneTopicRequired { get; set; }
}
