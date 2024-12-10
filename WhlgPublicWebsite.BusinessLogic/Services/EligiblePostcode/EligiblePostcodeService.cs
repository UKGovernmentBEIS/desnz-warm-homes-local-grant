using WhlgPublicWebsite.BusinessLogic.Extensions;
using Microsoft.Extensions.Logging;

namespace WhlgPublicWebsite.BusinessLogic.Services.EligiblePostcode;

public interface IEligiblePostcodeService
{
    public bool IsEligiblePostcode(string postcode);
}

public class EligiblePostcodeService : IEligiblePostcodeService
{
    private readonly EligiblePostcodeListCache eligiblePostcodeListCache;
    private readonly ILogger<EligiblePostcodeService> logger;

    public EligiblePostcodeService(EligiblePostcodeListCache eligiblePostcodeListCache,
        ILogger<EligiblePostcodeService> logger)
    {
        this.eligiblePostcodeListCache = eligiblePostcodeListCache;
        this.logger = logger;
    }

    // Check whether a postcode is in the list of eligible postcodes found on this page
    // https://www.gov.uk/government/publications/home-upgrade-grant-phase-2 in the "HUG: Phase 2 - eligible postcodes"
    // spreadsheet.
    public bool IsEligiblePostcode(string postcode)
    {
        var normalisedPostcode = postcode.NormaliseToUkPostcodeFormat();

        if (normalisedPostcode == null)
        {
            // We shouldn't be passing non-postcodes to this method so record an error that can be investigated.
            logger.LogError("IsEligiblePostcode was called with a string that is not a valid postcode: {}", postcode);
            return false;
        }

        return eligiblePostcodeListCache.GetEligiblePostcodes().Contains(normalisedPostcode);
    }
}