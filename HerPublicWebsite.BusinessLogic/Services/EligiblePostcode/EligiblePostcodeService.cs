using System.Reflection;
using System.Text;
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
    private readonly List<string> eligiblePostcodes;
    private readonly ILogger<EligiblePostcodeService> logger;

    public EligiblePostcodeService(ILogger<EligiblePostcodeService> logger)
    {
        this.logger = logger;

        var info = Assembly.GetExecutingAssembly().GetName();
        var name = info.Name;
        using var stream = Assembly
            .GetExecutingAssembly()
            .GetManifestResourceStream($"{name}.Services.EligiblePostcode.EligiblePostcodeData.json")!;

        using var reader = new StreamReader(stream, Encoding.UTF8);
        var jsonContents = reader.ReadToEnd();

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