using WhlgPublicWebsite.BusinessLogic.ExternalServices.EmailSending;
using WhlgPublicWebsite.BusinessLogic.Services.CsvFileCreator;
using WhlgPublicWebsite.BusinessLogic.Services.RegularJobs;

namespace WhlgPublicWebsite.BusinessLogic.Services.PolicyTeamUpdate;

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
    private readonly IReferralFilterService referralFilterService;

    public PolicyTeamUpdateService(
        IDataAccessProvider dataProvider,
        ICsvFileCreator csvFileCreator,
        IWorkingDayHelperService workingDayHelperService,
        IEmailSender emailSender,
        IReferralFilterService referralFilterService
    )
    {
        this.dataProvider = dataProvider;
        this.csvFileCreator = csvFileCreator;
        this.workingDayHelperService = workingDayHelperService;
        this.emailSender = emailSender;
        this.referralFilterService = referralFilterService;
    }

    public async Task SendPolicyTeamUpdate()
    {
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

    private async Task<MemoryStream> CreateReferralRequestOverviewFileData()
    {
        var endDate = await workingDayHelperService.AddWorkingDaysToDateTime(DateTime.Now, -10);
        var startDate = await workingDayHelperService.AddWorkingDaysToDateTime(DateTime.Now.AddDays(-7), -10);
        var referrals = await dataProvider.GetWhlgReferralRequestsBetweenDates(startDate, endDate);
        var filteredReferrals = referrals
            .Where(referralFilterService.WasSubmittedToNonPendingAuthority);
        return csvFileCreator.CreateReferralRequestOverviewFileDataForS3(filteredReferrals);
    }

    private async Task<(DateTime, DateTime)> RecentReferralRequestTimePeriod()
    {
        var endDate = await workingDayHelperService.AddWorkingDaysToDateTime(DateTime.Now, -10);
        var startDate = await workingDayHelperService.AddWorkingDaysToDateTime(DateTime.Now.AddDays(-7), -10);
        return (endDate, startDate);
    }

    private async Task<(MemoryStream, MemoryStream)> BuildRecentReferralRequestFollowUpFileData()
    {
        var (endDate, startDate) = await RecentReferralRequestTimePeriod();
        var referrals = await dataProvider.GetWhlgReferralRequestsBetweenDates(startDate, endDate);
        var filteredReferrals = referrals
            .Where(referralFilterService.WasSubmittedToNonPendingAuthority)
            .ToList();
        return (csvFileCreator.CreateLocalAuthorityReferralRequestFollowUpFileDataForS3(filteredReferrals),
            csvFileCreator.CreateConsortiumReferralRequestFollowUpFileDataForS3(filteredReferrals));
    }

    private async Task<(MemoryStream, MemoryStream)> BuildHistoricReferralRequestFollowUpFileData()
    {
        var referrals = await dataProvider.GetAllWhlgReferralRequestsForSlaComplianceReporting();
        var filteredReferrals = referrals
            .Where(referralFilterService.WasSubmittedToNonPendingAuthority)
            .ToList();
        return (csvFileCreator.CreateLocalAuthorityReferralRequestFollowUpFileDataForS3(filteredReferrals),
            csvFileCreator.CreateConsortiumReferralRequestFollowUpFileDataForS3(filteredReferrals));
    }
}