namespace WhlgPublicWebsite.BusinessLogic.Models;

public class NotificationDetails
{
    public string FutureSchemeNotificationEmail { get; set; }
    public bool FutureSchemeNotificationConsent { get; set; }
    
    public int? ReferralRequestId { get; set; }
    
    public ReferralRequest ReferralRequest { get; set; }

    public NotificationDetails()
    {
    }

    public NotificationDetails(Questionnaire questionnaire)
    {
        FutureSchemeNotificationEmail = questionnaire.NotificationEmailAddress;
        FutureSchemeNotificationConsent = questionnaire.NotificationConsent!.Value;
    }
}
