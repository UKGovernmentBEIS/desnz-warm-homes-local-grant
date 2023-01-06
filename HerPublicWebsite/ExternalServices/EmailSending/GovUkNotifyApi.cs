using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Notify.Client;
using Notify.Exceptions;
using Notify.Models.Responses;

namespace HerPublicWebsite.ExternalServices.EmailSending
{
    public class GovUkNotifyApi: IEmailSender
    {
        private readonly NotificationClient client;
        private readonly GovUkNotifyConfiguration govUkNotifyConfig;
        private readonly ILogger<GovUkNotifyApi> logger;
        
        public GovUkNotifyApi(IOptions<GovUkNotifyConfiguration> config, ILogger<GovUkNotifyApi> logger)
        {
            govUkNotifyConfig = config.Value;
            client = new NotificationClient(govUkNotifyConfig.ApiKey);
            this.logger = logger;
        }

        private EmailNotificationResponse SendEmail(GovUkNotifyEmailModel emailModel)
        {
            try
            {
                var response = client.SendEmail(
                    emailModel.EmailAddress,
                    emailModel.TemplateId,
                    emailModel.Personalisation,
                    emailModel.Reference,
                    emailModel.EmailReplyToId);
                return response;
            }
            catch (NotifyClientException e)
            {
                if (e.Message.Contains("Not a valid email address"))
                {
                    throw new EmailSenderException(EmailSenderExceptionType.InvalidEmailAddress);
                }

                logger.LogError("GOV.UK Notify returned an error: " + e.Message);
                throw new EmailSenderException(EmailSenderExceptionType.Other);
            }
        }

        public void SendReferenceNumberEmail(string emailAddress, string reference)
        {
            var template = govUkNotifyConfig.ApplicationReferenceNumberTemplate;
            var personalisation = new Dictionary<string, dynamic>
            {
                { template.ReferencePlaceholder, reference },
                { template.MagicLinkPlaceholder, govUkNotifyConfig.BaseUrl + "returning-user/" + reference },
                { template.ReturningUserLinkPlaceholder, govUkNotifyConfig.BaseUrl + "new-or-returning-user" },
                { template.FeedbackLinkPlaceholder, Constants.FEEDBACK_URL_DEFAULT }
            };
            var emailModel = new GovUkNotifyEmailModel
            {
                EmailAddress = emailAddress,
                TemplateId = template.Id,
                Personalisation = personalisation
            };
            var response = SendEmail(emailModel);
        }

        public void SendRequestedDocumentEmail(string emailAddress, byte[] documentContents)
        {
            var template = govUkNotifyConfig.RequestDocumentTemplate;
            var personalisation = new Dictionary<string, dynamic>
            {
                { template.DocumentContentsPlaceholder, NotificationClient.PrepareUpload(documentContents) }
            };
            var emailModel = new GovUkNotifyEmailModel
            {
                EmailAddress = emailAddress,
                TemplateId = template.Id,
                Personalisation = personalisation
            };
            var response = SendEmail(emailModel);
        }
    }

    internal class GovUkNotifyEmailModel
    {
        public string EmailAddress { get; set; }
        public string TemplateId { get; set; }
        public Dictionary<string, dynamic> Personalisation { get; set; }
        public string Reference { get; set; }
        public string EmailReplyToId { get; set; }
    }
}