
namespace HerPublicWebsite.ExternalServices.EmailSending
{
    public interface IEmailSender
    {
        public void SendReferenceNumberEmail(string emailAddress, string reference);
        public void SendRequestedDocumentEmail(string emailAddress, byte[] documentContents);
    }
}