using HerPublicWebsite.BusinessLogic.ExternalServices.EmailSending;

namespace HerPublicWebsite.BusinessLogic.Services.RegularJobs;

public class PendingReferralNotificationService
{
    private readonly IEmailSender emailSender;
    
    public PendingReferralNotificationService(IEmailSender emailSender)
    {
        this.emailSender = emailSender;
    }
    
    public void SendPendingReferralNotifications()
    {
        this.emailSender.SendPendingReferralReportEmail();
    }
}