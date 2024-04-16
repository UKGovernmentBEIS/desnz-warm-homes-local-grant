using HerPublicWebsite.BusinessLogic.ExternalServices.EmailSending;
using HerPublicWebsite.BusinessLogic.Services.CsvFileCreator;
using HerPublicWebsite.BusinessLogic.Services.RegularJobs;

namespace HerPublicWebsite.BusinessLogic.Services.PolicyTeamUpdate;

public interface IPolicyTeamUpdate
{
    public Task SendPolicyTeamUpdate();
}

public class PolicyTeamUpdateService : IPolicyTeamUpdate
{
    private readonly IDataAccessProvider dataProvider;
    private readonly ICsvFileCreator csvFileCreator;
    private readonly IWorkingDayHelperService workingDayHelperService;
    private readonly IEmailSender emailSender;
    
    public PolicyTeamUpdateService(
        IDataAccessProvider dataProvider,
        ICsvFileCreator csvFileCreator, 
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
        var (recentLocalAuthorityReferralRequestFollowUpFileData,
            recentConsortiumReferralRequestFollowUpFileData) =
                await BuildRecentReferralRequestFollowUpFileData();
        var (historicLocalAuthorityReferralRequestFollowUpFileData, 
            historicConsortiumReferralRequestFollowUpFileData) = 
                await BuildHistoricReferralRequestFollowUpFileData();
        emailSender.SendComplianceEmail(
            recentReferralRequestOverviewFileData,
            recentLocalAuthorityReferralRequestFollowUpFileData,
            recentConsortiumReferralRequestFollowUpFileData,
            historicLocalAuthorityReferralRequestFollowUpFileData,
            historicConsortiumReferralRequestFollowUpFileData
        );

    }

    private async Task<MemoryStream> CreateReferralRequestOverviewFileData (){
        DateTime endDate = await workingDayHelperService.AddWorkingDaysToDateTime(DateTime.Now, -10); 
        DateTime startDate = await workingDayHelperService.AddWorkingDaysToDateTime(DateTime.Now.AddDays(-7), -10); 
        var newReferrals = await dataProvider.GetReferralRequestsBetweenDates(startDate, endDate);
        return csvFileCreator.CreateReferralRequestOverviewFileData(newReferrals);
    }

    private async Task<(DateTime, DateTime)> RecentReferralRequestTimePeriod()
    {
        DateTime endDate = await workingDayHelperService.AddWorkingDaysToDateTime(DateTime.Now, -10); 
        DateTime startDate = await workingDayHelperService.AddWorkingDaysToDateTime(DateTime.Now.AddDays(-7), -10);
        return (endDate, startDate);
    }

    private async Task<(MemoryStream, MemoryStream)> BuildRecentReferralRequestFollowUpFileData (){
        (DateTime endDate, DateTime startDate) = await RecentReferralRequestTimePeriod();
        var newReferrals = await dataProvider.GetReferralRequestsBetweenDates(startDate, endDate);
        return (csvFileCreator.CreateLocalAuthorityReferralRequestFollowUpFileData(newReferrals),
            csvFileCreator.CreateConsortiumReferralRequestFollowUpFileData(newReferrals));
    }

    private async Task<(MemoryStream,MemoryStream)> BuildHistoricReferralRequestFollowUpFileData (){
        var newReferrals = await dataProvider.GetAllReferralRequests();
        return (csvFileCreator.CreateLocalAuthorityReferralRequestFollowUpFileData(newReferrals), 
            csvFileCreator.CreateConsortiumReferralRequestFollowUpFileData(newReferrals));
    }
}
