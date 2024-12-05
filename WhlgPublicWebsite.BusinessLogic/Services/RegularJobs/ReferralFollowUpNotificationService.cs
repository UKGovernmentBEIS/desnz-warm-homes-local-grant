using Microsoft.Extensions.Options;
using WhlgPublicWebsite.BusinessLogic.ExternalServices.EmailSending;
using WhlgPublicWebsite.BusinessLogic.Services.CsvFileCreator;
using WhlgPublicWebsite.BusinessLogic.Services.ReferralFollowUps;

namespace WhlgPublicWebsite.BusinessLogic.Services.RegularJobs;

public interface IReferralFollowUpNotificationService
{
    public Task SendReferralFollowUpNotifications();
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

    public async Task SendReferralFollowUpNotifications()
    {
        var endDate = await workingDayHelperService.AddWorkingDaysToDateTime(DateTime.Today, -10);
        var startDate = referralRequestNotificationConfig.CutoffEpoch;
        var newReferrals = referralFilterService.FilterForSentToNonPending(
            await dataProvider.GetWhlgReferralRequestsWithNoFollowUpBetweenDates(startDate, endDate));
        var uriBuilder = new UriBuilder(globalConfig.AppBaseUrl);
        uriBuilder.Path = "referral-follow-up";
        foreach (var newReferral in newReferrals)
        {
            var referralRequestFollowUp = await referralFollowUpManager.CreateReferralRequestFollowUp(newReferral);
            uriBuilder.Query = "token=" + referralRequestFollowUp.Token;
            emailSender.SendFollowUpEmail(newReferral, uriBuilder.ToString());
            await dataProvider.UpdateReferralRequestByIdWithFollowUpSentAsync(newReferral.Id);
        }
    }
}