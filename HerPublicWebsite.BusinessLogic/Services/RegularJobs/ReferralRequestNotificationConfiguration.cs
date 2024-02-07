namespace HerPublicWebsite.BusinessLogic.Services.RegularJobs;

public class ReferralRequestNotificationConfiguration
{
    public const string ConfigSection = "ReferralRequestNotifications";
    
    public DateTime CutoffEpoch { get; set; }
}
