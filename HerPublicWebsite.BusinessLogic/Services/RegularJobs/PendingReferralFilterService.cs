using HerPublicWebsite.BusinessLogic.Models;

namespace HerPublicWebsite.BusinessLogic.Services.RegularJobs;

public interface IPendingReferralFilterService
{
    public List<ReferralRequest> FilterForPendingReferralReport(IEnumerable<ReferralRequest> referralRequests);
}

public class PendingReferralFilterService : IPendingReferralFilterService
{
    private readonly StartOfMonthService startOfMonthService;

    public PendingReferralFilterService(StartOfMonthService startOfMonthService)
    {
        this.startOfMonthService = startOfMonthService;
    }
    
    public List<ReferralRequest> FilterForPendingReferralReport(IEnumerable<ReferralRequest> referralRequests)
    {
        var startDate = startOfMonthService.GetStartOfPreviousMonth();
        
        return referralRequests.Where(rr =>
            LocalAuthorityData.LocalAuthorityDetailsByCustodianCode[rr.CustodianCode].Status ==
                LocalAuthorityData.Hug2Status.Pending |
            rr.WasSubmittedToPendingLocalAuthority && rr.RequestDate >= startDate)
            .ToList();
    }
}