using WhlgPublicWebsite.BusinessLogic.ExternalServices.OsPlaces;
using WhlgPublicWebsite.BusinessLogic.Models;

namespace WhlgPublicWebsite.BusinessLogic.Services.RegularJobs;

public interface IReferralFilterService
{
    public IEnumerable<ReferralRequest> FilterForPendingReferralReport(IEnumerable<ReferralRequest> referralRequests);
    public bool WasSubmittedToNonPendingAuthority(ReferralRequest referralRequest);
    public bool WasSubmittedWithContactEmailAddress(ReferralRequest referralRequest);
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

        return referralRequests.Where(rr => WasSubmittedToPendingAuthority(rr, startDate));
    }

    public bool WasSubmittedToNonPendingAuthority(ReferralRequest referralRequest)
    {
        return !referralRequest.WasSubmittedToPendingLocalAuthority;
    }

    public bool WasSubmittedWithContactEmailAddress(ReferralRequest referralRequest)
    {
        return !string.IsNullOrWhiteSpace(referralRequest.ContactEmailAddress);
    }

    private static bool WasSubmittedToPendingAuthority(ReferralRequest referral, DateTime startDate)
    {
        var custodianCode = LaMapping.GetCurrentCustodianCode(referral.CustodianCode);
        var localAuthority = LocalAuthorityData.LocalAuthorityDetailsByCustodianCode[custodianCode];
        var localAuthorityIsPending = localAuthority.Status == LocalAuthorityData.LocalAuthorityStatus.Pending;

        // Also include referrals to local authorities that have gone from Pending to another status in the last month.
        var wasSubmittedToPendingLocalAuthorityInLastMonth =
            referral.WasSubmittedToPendingLocalAuthority && referral.RequestDate >= startDate;

        return localAuthorityIsPending || wasSubmittedToPendingLocalAuthorityInLastMonth;
    }
}