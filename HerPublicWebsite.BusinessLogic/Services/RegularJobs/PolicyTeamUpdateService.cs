using HerPublicWebsite.BusinessLogic.ExternalServices.EmailSending;
using HerPublicWebsite.BusinessLogic.Services.RegularJobs;

namespace HerPublicWebsite.BusinessLogic.Services.PolicyTeamUpdate;

public interface IPolicyTeamUpdate
{
    public Task SendPolicyTeamUpdate();
}

public class PolicyTeamUpdateService : IPolicyTeamUpdate
{
    private readonly IDataAccessProvider dataProvider;
    private readonly CsvFileCreator.CsvFileCreator csvFileCreator;
    private readonly IWorkingDayHelperService workingDayHelperService;
    private readonly IEmailSender emailSender;
    
    public PolicyTeamUpdateService(
        IDataAccessProvider dataProvider,
        CsvFileCreator.CsvFileCreator csvFileCreator, 
        IWorkingDayHelperService workingDayHelperService,
        IEmailSender emailSender
    )
    {
        this.dataProvider = dataProvider;
        this.csvFileCreator = csvFileCreator;
        this.workingDayHelperService = workingDayHelperService;
        this.emailSender = emailSender;
    }

    public async Task SendPolicyTeamUpdate(){
        var recentReferralRequestOverviewFileData = await CreateReferralRequestOverviewFileData();
        var recentReferralRequestFollowUpFileData = await BuildRecentReferralRequestFollowUpFileData();
        var historicReferralRequestFollowUpFileData = await BuildHistoricReferralRequestFollowUpFileData();
        emailSender.SendComplianceEmail(
            recentReferralRequestOverviewFileData,
            recentReferralRequestFollowUpFileData,
            historicReferralRequestFollowUpFileData
        );

    }

    private async Task<MemoryStream> CreateReferralRequestOverviewFileData (){
        DateTime endDate = await workingDayHelperService.AddWorkingDaysToDateTime(DateTime.Now, -10); 
        DateTime startDate = await workingDayHelperService.AddWorkingDaysToDateTime(DateTime.Now.AddDays(-7), -10); 
        var newReferrals = await dataProvider.GetReferralRequestsBetweenDates(startDate, endDate);
        return csvFileCreator.CreateReferralRequestOverviewFileData(newReferrals);
    }

    private async Task<MemoryStream> BuildRecentReferralRequestFollowUpFileData (){
        DateTime endDate = await workingDayHelperService.AddWorkingDaysToDateTime(DateTime.Now, -10); 
        DateTime startDate = await workingDayHelperService.AddWorkingDaysToDateTime(DateTime.Now.AddDays(-7), -10); 
        var newReferrals = await dataProvider.GetReferralRequestsBetweenDates(startDate, endDate);
        return csvFileCreator.CreateReferralRequestFollowUpFileData(newReferrals);
    }

    private async Task<MemoryStream> BuildHistoricReferralRequestFollowUpFileData (){
        var newReferrals = await dataProvider.GetAllReferralRequests();
        return csvFileCreator.CreateReferralRequestFollowUpFileData(newReferrals);
    }
}
