using WhlgPublicWebsite.BusinessLogic.ExternalServices.OsPlaces;
using WhlgPublicWebsite.BusinessLogic.Models;

namespace WhlgPublicWebsite.BusinessLogic.Services.RegularJobs;

public interface IReferralFilterService
{
    public bool WasSubmittedToPendingAuthority(ReferralRequest referralRequest, DateTime startDate);
    public bool WasSubmittedToNonPendingAuthority(ReferralRequest referralRequest);
    public bool WasSubmittedWithContactEmailAddress(ReferralRequest referralRequest);
}

public class ReferralFilterService : IReferralFilterService
{
    public bool WasSubmittedToPendingAuthority(ReferralRequest referral, DateTime startDate)
    {
        var custodianCode = LaMapping.GetCurrentCustodianCode(referral.CustodianCode);
        var localAuthority = LocalAuthorityData.LocalAuthorityDetailsByCustodianCode[custodianCode];
        var localAuthorityIsPending = localAuthority.Status == LocalAuthorityData.LocalAuthorityStatus.Pending;

        // Also include referrals to local authorities that have gone from Pending to another status in the last month.
        var wasSubmittedToPendingLocalAuthorityInLastMonth =
            referral.WasSubmittedToPendingLocalAuthority && referral.RequestDate >= startDate;

        return localAuthorityIsPending || wasSubmittedToPendingLocalAuthorityInLastMonth;
    }

    public bool WasSubmittedToNonPendingAuthority(ReferralRequest referralRequest)
    {
        return !referralRequest.WasSubmittedToPendingLocalAuthority;
    }

    public bool WasSubmittedWithContactEmailAddress(ReferralRequest referralRequest)
    {
        return !string.IsNullOrWhiteSpace(referralRequest.ContactEmailAddress);
    }
}