using WhlgPublicWebsite.BusinessLogic.ExternalServices.EmailSending;
using WhlgPublicWebsite.BusinessLogic.Services.CsvFileCreator;

namespace WhlgPublicWebsite.BusinessLogic.Services.RegularJobs;

public class PendingReferralNotificationService
{
    private readonly IDataAccessProvider dataProvider;
    private readonly ICsvFileCreator csvFileCreator;
    private readonly IEmailSender emailSender;
    private readonly IReferralFilterService referralFilterService;
    private readonly IDateHelper dateHelper;

    public PendingReferralNotificationService(
        IDataAccessProvider dataProvider,
        ICsvFileCreator csvFileCreator,
        IEmailSender emailSender,
        IReferralFilterService referralFilterService,
        IDateHelper dateHelper)
    {
        this.dataProvider = dataProvider;
        this.csvFileCreator = csvFileCreator;
        this.emailSender = emailSender;
        this.referralFilterService = referralFilterService;
        this.dateHelper = dateHelper;
    }

    public async Task SendPendingReferralNotifications()
    {
        var pendingReferralRequestsFileData = await BuildPendingReferralRequestsFileData();
        emailSender.SendPendingReferralReportEmail(pendingReferralRequestsFileData);
    }

    private async Task<MemoryStream> BuildPendingReferralRequestsFileData()
    {
        var startDate = dateHelper.GetStartOfPreviousMonth();
        var referralRequests = await dataProvider.GetAllWhlgReferralRequests();
        var pendingReferralRequests = referralRequests
            .Where(rr => referralFilterService.WasSubmittedToPendingAuthority(rr, startDate));
        return csvFileCreator.CreatePendingReferralRequestFileDataForS3(pendingReferralRequests);
    }
}