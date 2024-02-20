using HerPublicWebsite.BusinessLogic.Models;

namespace HerPublicWebsite.BusinessLogic.ExternalServices.EmailSending;

public interface IEmailSender
{
    public void SendReferenceCodeEmailForLiveLocalAuthority(
        string emailAddress,
        string recipientName,
        ReferralRequest referralRequest);

    public void SendReferenceCodeEmailForPendingLocalAuthority(
        string emailAddress,
        string recipientName,
        ReferralRequest referralRequest);

    public void SendFollowUpEmail
    (
        ReferralRequest referralRequest,
        string followUpLink
    );
    
    public void SendComplianceEmail
    (
        MemoryStream recentReferralRequestOverviewFileData,
        MemoryStream recentReferralRequestFollowUpFileData,
        MemoryStream historicReferralRequestFollowUpFileData
    );
}
