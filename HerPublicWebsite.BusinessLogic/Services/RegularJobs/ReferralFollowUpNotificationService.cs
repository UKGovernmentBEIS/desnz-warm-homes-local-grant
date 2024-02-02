using HerPublicWebsite.BusinessLogic.ExternalServices.EmailSending;
using HerPublicWebsite.BusinessLogic.Services.ReferralFollowUps;
using Microsoft.Extensions.Options;

namespace HerPublicWebsite.BusinessLogic.Services.RegularJobs;

public interface IReferralFollowUpNotificationService
{
    public Task SendReferralFollowUpNotifications();
}

public class ReferralFollowUpNotificationService : IReferralFollowUpNotificationService
{
    private readonly GlobalConfiguration globalConfig;
    private readonly ReferralRequestNotificationConfiguration referralRequestNotificationConfig;
    private readonly IDataAccessProvider dataProvider;
    private readonly CsvFileCreator.CsvFileCreator csvFileCreator;
    private readonly IWorkingDayHelperService workingDayHelperService;
    private readonly IReferralFollowUpService referralFollowUpManager;
    private readonly IEmailSender emailSender;

    public ReferralFollowUpNotificationService(
        IOptions<GlobalConfiguration> globalConfig,
        IOptions<ReferralRequestNotificationConfiguration> referralRequestNotificationConfig,
        IEmailSender emailSender,
        IDataAccessProvider dataProvider,
        CsvFileCreator.CsvFileCreator csvFileCreator, 
        IWorkingDayHelperService workingDayHelperService,
        IReferralFollowUpService referralFollowUpManager
        )
    {
        this.globalConfig = globalConfig.Value;
        this.referralRequestNotificationConfig = referralRequestNotificationConfig.Value;
        this.emailSender = emailSender;
        this.dataProvider = dataProvider;
        this.csvFileCreator = csvFileCreator;
        this.workingDayHelperService = workingDayHelperService;
        this.referralFollowUpManager = referralFollowUpManager;
    }

    public async Task SendReferralFollowUpNotifications()
    {
        var endDate = await workingDayHelperService.AddWorkingDaysToDateTime(DateTime.Today, -10);
        var startDate = referralRequestNotificationConfig.CutoffEpoch;
        var newReferrals = await dataProvider.GetReferralRequestsWithNoFollowUpBetweenDates(startDate, endDate);
        var uriBuilder = new UriBuilder(globalConfig.AppBaseUrl);
        uriBuilder.Path = "referral-follow-up";
        foreach (var newReferral in newReferrals) {
            var referralRequestFollowUp = await referralFollowUpManager.CreateReferralRequestFollowUp(newReferral);
            uriBuilder.Query = "token=" + referralRequestFollowUp.Token;
            emailSender.SendFollowUpEmail(newReferral, uriBuilder.ToString());
            await dataProvider.UpdateReferralRequestByIdWithFollowUpSentAsync(newReferral.Id);
        }
    }
}

