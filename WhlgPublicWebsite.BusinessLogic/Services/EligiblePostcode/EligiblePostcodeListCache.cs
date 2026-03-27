using System.Reflection;
using System.Text;
using Newtonsoft.Json;

namespace WhlgPublicWebsite.BusinessLogic.Services.EligiblePostcode;

public class EligiblePostcodeListCache
{
    // TODO DESNZ-2197: Remove the references to the old file (including the file itself) once we have moved entirely to the IMD2025 postcodes.

    private readonly List<string> eligiblePostcodesImd2025;
    private readonly List<string> mergedEligiblePostcodes;
    private readonly IEligiblePostcodeImdFileChecker imdChecker;

    public EligiblePostcodeListCache(IEligiblePostcodeImdFileChecker imdChecker)
    {
        this.imdChecker = imdChecker;

        var jsonContents = ReadPostcodesFromJsonFile("EligiblePostcodeData");
        var jsonContentsImd2025 = ReadPostcodesFromJsonFile("EligiblePostcodeData_IMD2025");

        var eligiblePostcodes = JsonConvert.DeserializeObject<List<string>>(jsonContents);
        eligiblePostcodesImd2025 = JsonConvert.DeserializeObject<List<string>>(jsonContentsImd2025);

        mergedEligiblePostcodes = eligiblePostcodes.Concat(eligiblePostcodesImd2025).Distinct().ToList();
    }

    public List<string> GetEligiblePostcodes()
    {
        return imdChecker.ShouldUseImd2025Only()
            ? eligiblePostcodesImd2025
            : mergedEligiblePostcodes;
    }

    private static string ReadPostcodesFromJsonFile(string fileName)
    {
        var info = Assembly.GetExecutingAssembly().GetName();
        var name = info.Name;
        using var stream = Assembly
            .GetExecutingAssembly()
            .GetManifestResourceStream($"{name}.Services.EligiblePostcode.{fileName}.json")!;

        using var reader = new StreamReader(stream, Encoding.UTF8);
        return reader.ReadToEnd();
    }
}