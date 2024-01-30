using HerPublicWebsite.BusinessLogic.Models;
using HerPublicWebsite.BusinessLogic.Services.RegularJobs;

namespace HerPublicWebsite.BusinessLogic.Services.RegularJobs;

public interface IReferralFollowUpService
{
    public Task<IList<ReferralRequest>> GetReferralsPassedTenWorkingDayThresholdWithNoFollowUp();
}

public class ReferralFollowUpService : IReferralFollowUpService
{
    private readonly IDataAccessProvider dataProvider;
    private readonly CsvFileCreator.CsvFileCreator csvFileCreator;
    private readonly IWorkingDayHelperService workingDayHelperService;

    public ReferralFollowUpService(
        IDataAccessProvider dataProvider,
        CsvFileCreator.CsvFileCreator csvFileCreator, 
        IWorkingDayHelperService workingDayHelperService)
    {
        this.dataProvider = dataProvider;
        this.csvFileCreator = csvFileCreator;
        this.workingDayHelperService = workingDayHelperService;
    }

    public async Task<IList<ReferralRequest>> GetReferralsPassedTenWorkingDayThresholdWithNoFollowUp()
    {
        var endDate = await workingDayHelperService.AddWorkingDaysToDateTime(DateTime.Today, -10);
        return await dataProvider.GetReferralRequestsWithNoFollowUpBeforeDate(endDate);
    }
}

