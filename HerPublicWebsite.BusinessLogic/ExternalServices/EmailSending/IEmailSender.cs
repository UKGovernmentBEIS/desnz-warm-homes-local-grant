namespace HerPublicWebsite.BusinessLogic.ExternalServices.EmailSending;

public interface IEmailSender
{
    public void SendReferenceCodeEmail
    (
        string emailAddress,
        string recipientName,
        string referenceCode,
        string custodianCode
    );
}
