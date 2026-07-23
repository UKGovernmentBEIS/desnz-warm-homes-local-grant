namespace WhlgPublicWebsite.Models.Questionnaire;

public class ReferralsPausedViewModel : QuestionFlowViewModel
{
    public string LocalAuthorityName { get; set; }
    public string LocalAuthorityMessagePartialViewPath { get; set; }
    public bool Submitted { get; set; }
}
