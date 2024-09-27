using HerPublicWebsite.BusinessLogic.Extensions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace HerPublicWebsite.BusinessLogic.Services.EligiblePostcode;

public interface IEligiblePostcodeService
{
    public bool IsEligiblePostcode(string postcode);
}

public class EligiblePostcodeService : IEligiblePostcodeService
{
    // build structure means json file is in different location when running locally
    private const string PostcodeJsonPath = "Services/EligiblePostcode/EligiblePostcodeData.json";

    private const string LocalPostcodeJsonPath =
        "../HerPublicWebsite.BusinessLogic/Services/EligiblePostcode/EligiblePostcodeData.json";

    private readonly List<string> eligiblePostcodes;
    private readonly ILogger<EligiblePostcodeService> logger;

    public EligiblePostcodeService(ILogger<EligiblePostcodeService> logger)
    {
        this.logger = logger;
        string jsonContents;

        if (File.Exists(LocalPostcodeJsonPath))
        {
            using var reader = new StreamReader(LocalPostcodeJsonPath);
            jsonContents = reader.ReadToEnd();
        }
        else
        {
            using var reader = new StreamReader(PostcodeJsonPath);
            jsonContents = reader.ReadToEnd();
        }

        eligiblePostcodes = JsonConvert.DeserializeObject<List<string>>(jsonContents);
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

        return eligiblePostcodes.Contains(normalisedPostcode);
    }
}