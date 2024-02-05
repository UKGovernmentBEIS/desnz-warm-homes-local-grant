namespace HerPublicWebsite.BusinessLogic.ExternalServices.EmailSending;

public class GovUkNotifyConfiguration
{
    public const string ConfigSection = "GovUkNotify";
        
    public string ApiKey { get; set; }
    public string ComplianceEmailRecipients { get; set; }
    public ReferenceCodeConfiguration ReferenceCodeTemplate { get; set; }
    public ReferralFollowUpConfiguration ReferralFollowUpTemplate { get; set; }
    public ComplianceReportConfiguration ComplianceReportTemplate { get; set; }
}

public class ReferenceCodeConfiguration
{
    public string Id { get; set; }
    public string RecipientNamePlaceholder { get; set; }
    public string ReferenceCodePlaceholder { get; set; }
    public string LocalAuthorityNamePlaceholder { get; set; }
    public string LocalAuthorityWebsiteUrlPlaceholder { get; set; }
}

public class ReferralFollowUpConfiguration
{
    public string Id { get; set; }
    public string RecipientNamePlaceholder { get; set; }
    public string ReferenceCodePlaceholder { get; set; }
    public string LocalAuthorityNamePlaceholder { get; set; }
    public string ReferralDatePlaceholder { get; set; }
    public string FollowUpLinkPlaceholder {get; set; }
}

public class ComplianceReportConfiguration
{
    public string Id { get; set; }
    public string RecipientNamePlaceholder { get; set; }
    public string File1Link { get; set; }
    public string File2Link { get; set; }
    public string File3Link { get; set; }
}
