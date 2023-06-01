using System;
using System.Collections.Generic;
using HerPublicWebsite.BusinessLogic.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Notify.Client;
using Notify.Exceptions;
using Notify.Models.Responses;

namespace HerPublicWebsite.BusinessLogic.ExternalServices.EmailSending
{
    public class GovUkNotifyApi : IEmailSender
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
        
        public void SendReferenceCodeEmail
        (
            string emailAddress,
            string recipientName,
            string referenceCode,
            string custodianCode
        ) {
            var template = govUkNotifyConfig.ReferenceCodeTemplate;
            LocalAuthorityData.LocalAuthorityDetails localAuthorityDetails;
            try
            {
                localAuthorityDetails = LocalAuthorityData.LocalAuthorityDetailsByCustodianCode[custodianCode];
            }
            catch (KeyNotFoundException ex)
            {
                logger.LogError
                (
                    ex,
                    "Attempted to send reference code email with invalid custodian code \"{CustodianCode}\"",
                    custodianCode
                );
                throw new ArgumentOutOfRangeException
                (
                    $"Attempted to send reference code email with invalid custodian code \"{custodianCode}\"",
                    ex
                );
            }
            
            var personalisation = new Dictionary<string, dynamic>
            {
                { template.RecipientNamePlaceholder, recipientName },
                { template.ReferenceCodePlaceholder, referenceCode },
                { template.LocalAuthorityNamePlaceholder, localAuthorityDetails.Name + " Council" },
                { template.LocalAuthorityWebsiteUrlPlaceholder, localAuthorityDetails.WebsiteUrl },
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