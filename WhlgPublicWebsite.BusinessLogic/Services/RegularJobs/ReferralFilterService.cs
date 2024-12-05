using WhlgPublicWebsite.BusinessLogic.ExternalServices.OsPlaces;
using WhlgPublicWebsite.BusinessLogic.Models;

namespace WhlgPublicWebsite.BusinessLogic.Services.RegularJobs;

public interface IReferralFilterService
{
    public IEnumerable<ReferralRequest> FilterForPendingReferralReport(IEnumerable<ReferralRequest> referralRequests);
    public IEnumerable<ReferralRequest> FilterForSentToNonPending(IEnumerable<ReferralRequest> referralRequests);
}

public class ReferralFilterService : IReferralFilterService
{
    private readonly IDateHelper dateHelper;

    public ReferralFilterService(IDateHelper dateHelper)
    {
        this.dateHelper = dateHelper;
    }
    
    public IEnumerable<ReferralRequest> FilterForPendingReferralReport(IEnumerable<ReferralRequest> referralRequests)
    {
        var startDate = dateHelper.GetStartOfPreviousMonth();

        return referralRequests.Where(rr => ShouldIncludeInReport(rr, startDate));
    }
    
    public IEnumerable<ReferralRequest> FilterForSentToNonPending(IEnumerable<ReferralRequest> referralRequests)
    {
        return referralRequests.Where(rr => !rr.WasSubmittedToPendingLocalAuthority);
    }
    
    private static bool ShouldIncludeInReport(ReferralRequest referral, DateTime startDate)
    {
        var custodianCode = LaMapping.GetCurrentCustodianCode(referral.CustodianCode);
        var localAuthority = LocalAuthorityData.LocalAuthorityDetailsByCustodianCode[custodianCode];
        var localAuthorityIsPending = localAuthority.Status == LocalAuthorityData.Hug2Status.Pending;

        // Also include referrals to local authorities that have gone from Pending to another status in the last month.
        var wasSubmittedToPendingLocalAuthorityInLastMonth =
            referral.WasSubmittedToPendingLocalAuthority && referral.RequestDate >= startDate;

        return localAuthorityIsPending || wasSubmittedToPendingLocalAuthorityInLastMonth;
    }
}