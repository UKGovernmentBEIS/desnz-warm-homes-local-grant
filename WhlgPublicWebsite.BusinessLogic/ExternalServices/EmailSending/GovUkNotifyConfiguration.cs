namespace WhlgPublicWebsite.BusinessLogic.ExternalServices.EmailSending;

public class GovUkNotifyConfiguration
{
    public const string ConfigSection = "GovUkNotify";

    public string ApiKey { get; set; }
    public string ComplianceEmailRecipients { get; set; }
    public string PendingReferralEmailRecipients { get; set; }
    public LiveReferenceCodeConfiguration ReferenceCodeForLiveLocalAuthorityTemplate { get; set; }
    public LiveReferenceCodeConfiguration ReferenceCodeForLiveNo10DayLocalAuthorityTemplate { get; set; }
    public ReferenceCodeConfiguration ReferenceCodeForTakingFutureReferralsLocalAuthorityTemplate { get; set; }
    public ReferenceCodeConfiguration ReferenceCodeForPendingLocalAuthorityTemplate { get; set; }
    public ReferralFollowUpConfiguration ReferralFollowUpTemplate { get; set; }
    public ComplianceReportConfiguration ComplianceReportTemplate { get; set; }
    public PendingReferralReportConfiguration PendingReferralReportTemplate { get; set; }
}

public class LiveReferenceCodeConfiguration
{
    public string Id { get ; set; }
    public string RecipientNamePlaceholder { get; set; }
    public string ReferenceCodePlaceholder { get; set; }
    public string TitleDeliveryPartnerPlaceholder { get; set; }
    public string TitleDeliveryPartnerOrContractorPlaceholder { get; set; }
    public string YourDeliveryPartnerOrContractorPlaceholder { get; set; }
    public string WebsiteNamePlaceholder { get; set; }
    public string WebsiteUrlPlaceholder { get; set; }
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
    public string FollowUpLinkPlaceholder { get; set; }
}

public class ComplianceReportConfiguration
{
    public string Id { get; set; }
    public string RecipientNamePlaceholder { get; set; }
    public string OverviewFileLink { get; set; }
    public string RecentLocalAuthorityFollowUpFileLink { get; set; }
    public string RecentConsortiumFollowUpFileLink { get; set; }
    public string HistoricLocalAuthorityFollowUpFileLink { get; set; }
    public string HistoricConsortiumFollowUpFileLink { get; set; }
}

public class PendingReferralReportConfiguration
{
    public string Id { get; set; }
    public string LinkPlaceholder { get; set; }
}