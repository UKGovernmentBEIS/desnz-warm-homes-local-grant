using HerPublicWebsite.BusinessLogic.ExternalServices.EmailSending;
using HerPublicWebsite.BusinessLogic.Services.CsvFileCreator;

namespace HerPublicWebsite.BusinessLogic.Services.RegularJobs;

public class PendingReferralNotificationService
{
    private readonly IDataAccessProvider dataProvider;
    private readonly ICsvFileCreator csvFileCreator;
    private readonly IEmailSender emailSender;
    private readonly IPendingReferralFilterService pendingReferralFilterService;
    
    public PendingReferralNotificationService(
        IDataAccessProvider dataProvider,
        ICsvFileCreator csvFileCreator,
        IEmailSender emailSender,
        IPendingReferralFilterService pendingReferralFilterService)
    {
        this.dataProvider = dataProvider;
        this.csvFileCreator = csvFileCreator;
        this.emailSender = emailSender;
        this.pendingReferralFilterService = pendingReferralFilterService;
    }
    
    public async Task SendPendingReferralNotifications()
    {
        var pendingReferralRequestsFileData = await BuildPendingReferralRequestsFileData();
        emailSender.SendPendingReferralReportEmail(pendingReferralRequestsFileData);
    }

    private async Task<MemoryStream> BuildPendingReferralRequestsFileData()
    {
        var referralRequests = await dataProvider.GetAllReferralRequests();
        var pendingReferralRequests = pendingReferralFilterService.FilterForPendingReferralReport(referralRequests);
        return csvFileCreator.CreatePendingReferralRequestFileData(pendingReferralRequests);
    }
}