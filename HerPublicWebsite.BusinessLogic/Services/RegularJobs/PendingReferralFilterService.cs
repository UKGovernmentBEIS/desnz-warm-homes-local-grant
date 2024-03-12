using HerPublicWebsite.BusinessLogic.ExternalServices.OsPlaces;
using HerPublicWebsite.BusinessLogic.Models;

namespace HerPublicWebsite.BusinessLogic.Services.RegularJobs;

public interface IPendingReferralFilterService
{
    public IEnumerable<ReferralRequest> FilterForPendingReferralReport(IEnumerable<ReferralRequest> referralRequests);
}

public class PendingReferralFilterService : IPendingReferralFilterService
{
    private readonly IDateHelper dateHelper;

    public PendingReferralFilterService(IDateHelper dateHelper)
    {
        this.dateHelper = dateHelper;
    }
    
    public IEnumerable<ReferralRequest> FilterForPendingReferralReport(IEnumerable<ReferralRequest> referralRequests)
    {
        var startDate = dateHelper.GetStartOfPreviousMonth();

        return referralRequests.Where(rr => ShouldIncludeInReport(rr, startDate));
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