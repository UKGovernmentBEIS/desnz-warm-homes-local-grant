namespace WhlgPublicWebsite.Models.Questionnaire;

public class IneligibleViewModel : QuestionFlowViewModel
{
    public bool EpcIsTooHigh { get; set; }
    public string LocalAuthorityName { get; set; }
    public string LocalAuthorityWebsite { get; set; }
    public bool Submitted { get; set; }
}
