using Microsoft.Extensions.Options;
using WhlgPublicWebsite.BusinessLogic.ExternalServices.EmailSending;
using WhlgPublicWebsite.BusinessLogic.Services.CsvFileCreator;
using WhlgPublicWebsite.BusinessLogic.Services.ReferralFollowUps;

namespace WhlgPublicWebsite.BusinessLogic.Services.RegularJobs;

public interface IReferralFollowUpNotificationService
{
    public Task SendReferralFollowUpEmails();
}

public class ReferralFollowUpNotificationService : IReferralFollowUpNotificationService
{
    private readonly GlobalConfiguration globalConfig;
    private readonly ReferralRequestNotificationConfiguration referralRequestNotificationConfig;
    private readonly IDataAccessProvider dataProvider;
    private readonly ICsvFileCreator csvFileCreator;
    private readonly IWorkingDayHelperService workingDayHelperService;
    private readonly IReferralFollowUpService referralFollowUpManager;
    private readonly IEmailSender emailSender;
    private readonly IReferralFilterService referralFilterService;

    public ReferralFollowUpNotificationService(
        IOptions<GlobalConfiguration> globalConfig,
        IOptions<ReferralRequestNotificationConfiguration> referralRequestNotificationConfig,
        IEmailSender emailSender,
        IDataAccessProvider dataProvider,
        ICsvFileCreator csvFileCreator,
        IWorkingDayHelperService workingDayHelperService,
        IReferralFollowUpService referralFollowUpManager,
        IReferralFilterService referralFilterService)
    {
        this.globalConfig = globalConfig.Value;
        this.referralRequestNotificationConfig = referralRequestNotificationConfig.Value;
        this.emailSender = emailSender;
        this.dataProvider = dataProvider;
        this.csvFileCreator = csvFileCreator;
        this.workingDayHelperService = workingDayHelperService;
        this.referralFollowUpManager = referralFollowUpManager;
        this.referralFilterService = referralFilterService;
    }

    public async Task SendReferralFollowUpEmails()
    {
        var endDate = await workingDayHelperService.AddWorkingDaysToDateTime(DateTime.Today, -10);
        var startDate = referralRequestNotificationConfig.CutoffEpoch;
        var newReferrals = await dataProvider.GetWhlgReferralRequestsWithNoFollowUpBetweenDates(startDate, endDate);
        var filteredReferrals = newReferrals
            .Where(referralFilterService.WasSubmittedWithContactEmailAddress)
            .Where(referralFilterService.WasSubmittedToNonPendingAuthority);
        var uriBuilder = new UriBuilder(globalConfig.AppBaseUrl);
        uriBuilder.Path = "referral-follow-up";
        foreach (var referral in filteredReferrals)
        {
            var referralRequestFollowUp = await referralFollowUpManager.CreateReferralRequestFollowUp(referral);
            uriBuilder.Query = "token=" + referralRequestFollowUp.Token;
            emailSender.SendFollowUpEmail(referral, uriBuilder.ToString());
            await dataProvider.UpdateReferralRequestByIdWithFollowUpSentAsync(referral.Id);
        }
    }
}