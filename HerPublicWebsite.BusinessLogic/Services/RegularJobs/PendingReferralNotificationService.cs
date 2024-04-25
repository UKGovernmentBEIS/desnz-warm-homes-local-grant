using HerPublicWebsite.BusinessLogic.ExternalServices.EmailSending;
using HerPublicWebsite.BusinessLogic.Services.CsvFileCreator;

namespace HerPublicWebsite.BusinessLogic.Services.RegularJobs;

public class PendingReferralNotificationService
{
    private readonly IDataAccessProvider dataProvider;
    private readonly ICsvFileCreator csvFileCreator;
    private readonly IEmailSender emailSender;
    private readonly IReferralFilterService referralFilterService;
    
    public PendingReferralNotificationService(
        IDataAccessProvider dataProvider,
        ICsvFileCreator csvFileCreator,
        IEmailSender emailSender,
        IReferralFilterService referralFilterService)
    {
        this.dataProvider = dataProvider;
        this.csvFileCreator = csvFileCreator;
        this.emailSender = emailSender;
        this.referralFilterService = referralFilterService;
    }
    
    public async Task SendPendingReferralNotifications()
    {
        var pendingReferralRequestsFileData = await BuildPendingReferralRequestsFileData();
        emailSender.SendPendingReferralReportEmail(pendingReferralRequestsFileData);
    }

    private async Task<MemoryStream> BuildPendingReferralRequestsFileData()
    {
        var referralRequests = await dataProvider.GetAllReferralRequests();
        var pendingReferralRequests = referralFilterService.FilterForPendingReferralReport(referralRequests);
        return csvFileCreator.CreatePendingReferralRequestFileData(pendingReferralRequests);
    }
}