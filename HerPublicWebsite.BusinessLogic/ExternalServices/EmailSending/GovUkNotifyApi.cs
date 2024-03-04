using HerPublicWebsite.BusinessLogic.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Notify.Client;
using Notify.Exceptions;
using Notify.Interfaces;

namespace HerPublicWebsite.BusinessLogic.ExternalServices.EmailSending
{
    public class GovUkNotifyApi : IEmailSender
    {
        private readonly INotificationClient client;
        private readonly GovUkNotifyConfiguration govUkNotifyConfig;
        private readonly ILogger<GovUkNotifyApi> logger;
        
        public GovUkNotifyApi(IOptions<GovUkNotifyConfiguration> config, ILogger<GovUkNotifyApi> logger, INotificationClient client)
        {
            govUkNotifyConfig = config.Value;
            this.client = client;
            this.logger = logger;
        }

        private void SendEmail(GovUkNotifyEmailModel emailModel)
        {
            try
            {
                client.SendEmail(
                    emailModel.EmailAddress,
                    emailModel.TemplateId,
                    emailModel.Personalisation,
                    emailModel.Reference,
                    emailModel.EmailReplyToId);
            }
            catch (NotifyClientException e)
            {
                if (e.Message.Contains("Not a valid email address"))
                {
                    logger.LogWarning("GOV.UK Notify could not send to an invalid email address");
                }
                else if (e.Message.Contains("send to this recipient using a team-only API key"))
                {
                    // In development we use a 'team-only' API key which can only send to team emails
                    logger.LogWarning("GOV.UK Notify cannot send to this recipient using a team-only API key");
                }
                else
                {
                    logger.LogError(e, "GOV.UK Notify returned an error");
                }
            }
        }
        
        public void SendReferenceCodeEmailForLiveLocalAuthority
        (
            string emailAddress,
            string recipientName, 
            ReferralRequest referralRequest)
        {
            SendReferenceCodeEmail(emailAddress, recipientName, referralRequest, govUkNotifyConfig.ReferenceCodeForLiveLocalAuthorityTemplate);
        }
        
        public void SendReferenceCodeEmailForPendingLocalAuthority
        (
            string emailAddress,
            string recipientName,
            ReferralRequest referralRequest)
        {
            SendReferenceCodeEmail(emailAddress, recipientName, referralRequest, govUkNotifyConfig.ReferenceCodeForPendingLocalAuthorityTemplate);
        }

        public void SendFollowUpEmail
        (
            ReferralRequest referralRequest,
            string followUpLink
        ) {
            var template = govUkNotifyConfig.ReferralFollowUpTemplate;
            LocalAuthorityData.LocalAuthorityDetails localAuthorityDetails;
            try
            {
                localAuthorityDetails = LocalAuthorityData.LocalAuthorityDetailsByCustodianCode[referralRequest.CustodianCode];

                var personalisation = new Dictionary<string, dynamic>
                {
                    { template.RecipientNamePlaceholder, referralRequest.FullName },
                    { template.ReferenceCodePlaceholder, referralRequest.ReferralCode },
                    { template.LocalAuthorityNamePlaceholder, localAuthorityDetails.Name },
                    { template.ReferralDatePlaceholder, referralRequest.RequestDate.ToShortDateString() },
                    { template.FollowUpLinkPlaceholder, followUpLink },
                };
                var emailModel = new GovUkNotifyEmailModel
                {
                    EmailAddress = referralRequest.ContactEmailAddress,
                    TemplateId = template.Id,
                    Personalisation = personalisation
                };
                SendEmail(emailModel);
            }
            catch (KeyNotFoundException ex)
            {
                logger.LogError
                (
                    ex,
                    "Failed to send follow up email for referral request \"{referralRequest.Id}\" with invalid custodian code",
                    referralRequest.Id
                );
            }
            
            

        }
        
        public void SendComplianceEmail(
                MemoryStream recentReferralRequestOverviewFileData,
                MemoryStream recentReferralRequestFollowUpFileData,
                MemoryStream historicReferralRequestFollowUpFileData
                )
                {
                    var recipientList = govUkNotifyConfig.ComplianceEmailRecipients;
                    if (String.IsNullOrEmpty(recipientList))
                    {
                        return;
                    }
                    
                    var template = govUkNotifyConfig.ComplianceReportTemplate;
                    Dictionary<String, dynamic> personalisation = new Dictionary<String, dynamic>
                    {
                        { "File1Link", NotificationClient.PrepareUpload(recentReferralRequestOverviewFileData.ToArray(), true)},
                        { "File2Link", NotificationClient.PrepareUpload(recentReferralRequestFollowUpFileData.ToArray(), true)},
                        { "File3Link", NotificationClient.PrepareUpload(historicReferralRequestFollowUpFileData.ToArray(), true)},
                    };
                    foreach (var emailAddress in recipientList.Split(","))
                    {
                        var emailModel = new GovUkNotifyEmailModel
                        {
                            EmailAddress = emailAddress.Trim(),
                            TemplateId = template.Id,
                            Personalisation = personalisation
                        };
                        SendEmail(emailModel);
                    }
                    
                }

        public void SendPendingReferralReportEmail()
        {
            var recipientList = govUkNotifyConfig.PendingReferralEmailRecipients;
            if (string.IsNullOrEmpty(recipientList))
            {
                return;
            }
            var template = govUkNotifyConfig.PendingReferralReportTemplate;
            var personalisation = new Dictionary<string, dynamic>
            {
                { "Link", "https://www.gov.uk/apply-home-upgrade-grant" },
            };
            foreach (var emailAddress in recipientList.Split(","))
            {
                var emailModel = new GovUkNotifyEmailModel
                {
                    EmailAddress = emailAddress.Trim(),
                    TemplateId = template.Id,
                    Personalisation = personalisation
                };
                SendEmail(emailModel);
            }
        }

        private void SendReferenceCodeEmail
        (
            string emailAddress,
            string recipientName,
            ReferralRequest referralRequest,
            ReferenceCodeConfiguration template)
        {
            LocalAuthorityData.LocalAuthorityDetails localAuthorityDetails;
            try
            {
                localAuthorityDetails = LocalAuthorityData.LocalAuthorityDetailsByCustodianCode[referralRequest.CustodianCode];
            }
            catch (KeyNotFoundException ex)
            {
                logger.LogError
                (
                    ex,
                    "Attempted to send reference code email with invalid custodian code \"{CustodianCode}\"",
                    referralRequest.CustodianCode
                );
                throw new ArgumentOutOfRangeException
                (
                    $"Attempted to send reference code email with invalid custodian code \"{referralRequest.CustodianCode}\"",
                    ex
                );
            }
            var personalisation = new Dictionary<string, dynamic>
            {
                { template.RecipientNamePlaceholder, recipientName },
                { template.ReferenceCodePlaceholder, referralRequest.ReferralCode },
                { template.LocalAuthorityNamePlaceholder, localAuthorityDetails.Name },
                { template.LocalAuthorityWebsiteUrlPlaceholder, localAuthorityDetails.WebsiteUrl },
            };
            var emailModel = new GovUkNotifyEmailModel
            {
                EmailAddress = emailAddress,
                TemplateId = template.Id,
                Personalisation = personalisation
            };
            SendEmail(emailModel);
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