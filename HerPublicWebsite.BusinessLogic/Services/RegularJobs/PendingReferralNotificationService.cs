using HerPublicWebsite.BusinessLogic.ExternalServices.EmailSending;

namespace HerPublicWebsite.BusinessLogic.Services.RegularJobs;

public interface IPendingReferralNotificationService
{
    public void SendPendingReferralNotifications();
}

public class PendingReferralNotificationService : IPendingReferralNotificationService
{
    private IEmailSender emailSender;
    
    public PendingReferralNotificationService(
        IEmailSender emailSender
    )
    {
        this.emailSender = emailSender;
    }
    
    public void SendPendingReferralNotifications()
    {
        this.emailSender.SendPendingReferralReportEmail();
    }
}