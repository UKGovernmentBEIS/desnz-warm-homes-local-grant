using HerPublicWebsite.BusinessLogic.Models;

namespace HerPublicWebsite.BusinessLogic.ExternalServices.EmailSending;

public interface IEmailSender
{
    public void SendReferenceCodeLiveLAEmail
    (
        string emailAddress,
        string recipientName,
        string referenceCode,
        string custodianCode
    );

    public void SendReferenceCodePendingLAEmail
    (
        string emailAddress,
        string recipientName,
        string referenceCode,
        string custodianCode
    );

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
