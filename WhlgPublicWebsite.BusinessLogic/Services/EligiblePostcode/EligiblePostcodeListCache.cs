using System.Reflection;
using System.Text;
using Newtonsoft.Json;

namespace HerPublicWebsite.BusinessLogic.Services.EligiblePostcode;

public class EligiblePostcodeListCache
{
    private readonly List<string> eligiblePostcodes;

    public EligiblePostcodeListCache()
    {
        var info = Assembly.GetExecutingAssembly().GetName();
        var name = info.Name;
        using var stream = Assembly
            .GetExecutingAssembly()
            .GetManifestResourceStream($"{name}.Services.EligiblePostcode.EligiblePostcodeData.json")!;

        using var reader = new StreamReader(stream, Encoding.UTF8);
        var jsonContents = reader.ReadToEnd();

        eligiblePostcodes = JsonConvert.DeserializeObject<List<string>>(jsonContents);
    }

    public List<string> GetEligiblePostcodes()
    {
        return eligiblePostcodes;
    }
}