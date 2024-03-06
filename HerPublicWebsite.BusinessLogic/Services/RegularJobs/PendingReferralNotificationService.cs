using HerPublicWebsite.BusinessLogic.ExternalServices.EmailSending;
using HerPublicWebsite.BusinessLogic.Models;

namespace HerPublicWebsite.BusinessLogic.Services.RegularJobs;

public class PendingReferralNotificationService
{
    private readonly IDataAccessProvider dataProvider;
    private readonly CsvFileCreator.CsvFileCreator csvFileCreator;
    private readonly IEmailSender emailSender;
    
    public PendingReferralNotificationService(
        IDataAccessProvider dataProvider,
        CsvFileCreator.CsvFileCreator csvFileCreator,
        IEmailSender emailSender)
    {
        this.dataProvider = dataProvider;
        this.csvFileCreator = csvFileCreator;
        this.emailSender = emailSender;
    }
    
    public async Task SendPendingReferralNotifications()
    {
        var pendingReferralRequestsFileData = await BuildPendingReferralRequestsFileData();
        this.emailSender.SendPendingReferralReportEmail(pendingReferralRequestsFileData);
    }

    private async Task<MemoryStream> BuildPendingReferralRequestsFileData()
    {
        // it is known that emails are sent on the 1st of the new month
        var startDate = DateTime.Now.AddMonths(-1); // 1st of previous month
        var endDate = DateTime.Now.AddDays(-1); // 31st of previous month
        var referralRequests = await dataProvider.GetReferralRequestsBetweenDates(startDate, endDate);
        
        var pendingReferralRequests = referralRequests.Where(rr =>
            rr.WasSubmittedToPendingLocalAuthority |
            LocalAuthorityData.LocalAuthorityDetailsByCustodianCode[rr.CustodianCode].Status ==
            LocalAuthorityData.Hug2Status.Pending)
            .ToList();

        return csvFileCreator.CreatePendingReferralRequestFileData(pendingReferralRequests);
    }
}