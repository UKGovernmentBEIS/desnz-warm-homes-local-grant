using System.Reflection;
using System.Text;
using Newtonsoft.Json;

namespace WhlgPublicWebsite.BusinessLogic.Services.EligiblePostcode;

public class EligiblePostcodeListCache
{
    private readonly List<string> eligiblePostcodes;

    public EligiblePostcodeListCache()
    {
        var jsonContents = ReadPostcodesFromJsonFile("EligiblePostcodeData");
        eligiblePostcodes = JsonConvert.DeserializeObject<List<string>>(jsonContents);
    }

    public List<string> GetEligiblePostcodes()
    {
        return eligiblePostcodes;
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