using System.Globalization;
using HerPublicWebsite.BusinessLogic.Models;
using Newtonsoft.Json;

namespace HerPublicWebsite.BusinessLogic.ExternalServices.OsPlaces;

public class OsPlacesLpiDto
{
    private static readonly TextInfo GbTextInfo = new CultureInfo("en-GB").TextInfo;
    
    // Adapted from the guidance at:
    // https://www.ordnancesurvey.co.uk/documents/product-support/getting-started/addressbase-addressbase-plus-addressbase-plus-islands-getting-started-guide.pdf
    public Address Parse()
    {
        var line1Parts = new List<string>();
        var line2Parts = new List<string>();
        
        var saoParts = new List<string>();
        var paoParts = new List<string>();
        
        // Organisation name
        if (!string.IsNullOrEmpty(Organisation))
        {
            line1Parts.Add(ToTitleCase(Organisation));
        }
        
        // SAO
        if (!string.IsNullOrEmpty(SaoText))
        {
            saoParts.Add(ToTitleCase(SaoText));
        }
        var saoStart = "";
        if (!string.IsNullOrEmpty(SaoStartNumber)) saoStart += SaoStartNumber;
        if (!string.IsNullOrEmpty(SaoStartSuffix)) saoStart += SaoStartSuffix;
        var saoEnd = "";
        if (!string.IsNullOrEmpty(SaoEndNumber)) saoEnd += SaoEndNumber;
        if (!string.IsNullOrEmpty(SaoEndSuffix)) saoEnd += SaoEndSuffix;
        var saoNumbers = "";
        if (!string.IsNullOrEmpty(saoStart)) saoNumbers += saoStart;
        if (!string.IsNullOrEmpty(saoEnd)) saoNumbers += ("–" + saoEnd);  // Use an en dash (–) for ranges
        if (!string.IsNullOrEmpty(saoNumbers))
        {
            saoParts.Add(saoNumbers);
        }
        if (saoParts.Any())
        {
            line1Parts.AddRange(saoParts);
        }
        
        // PAO text goes on the line before the rest of the PAO, if present
        if (!string.IsNullOrEmpty(PaoText))
        {
            line1Parts.Add(ToTitleCase(PaoText));
        }
        
        // Rest of PAO
        var paoStart = "";
        if (!string.IsNullOrEmpty(PaoStartNumber)) paoStart += PaoStartNumber;
        if (!string.IsNullOrEmpty(PaoStartSuffix)) paoStart += PaoStartSuffix;
        var paoEnd = "";
        if (!string.IsNullOrEmpty(PaoEndNumber)) paoEnd += PaoEndNumber;
        if (!string.IsNullOrEmpty(PaoEndSuffix)) paoEnd += PaoEndSuffix;
        var paoNumbers = "";
        if (!string.IsNullOrEmpty(paoStart)) paoNumbers += paoStart;
        if (!string.IsNullOrEmpty(paoEnd)) paoNumbers += ("–" + paoEnd);  // Use an en dash (–) for ranges
        if (!string.IsNullOrEmpty(paoNumbers))
        {
            paoParts.Add(paoNumbers);
        }
        
        if (line1Parts.Any())
        {
            line2Parts.AddRange(paoParts);
            line2Parts.Add(ToTitleCase(StreetDescription));
        }
        else
        {
            line1Parts.AddRange(paoParts);
            line1Parts.Add(ToTitleCase(StreetDescription));
        }

        var line1 = string.Join(", ", line1Parts.Where(p => !string.IsNullOrEmpty(p)));
        var line2 = string.Join(", ", line2Parts.Where(p => !string.IsNullOrEmpty(p)));
        
        return new Address()
        {
            AddressLine1 = line1,
            AddressLine2 = line2,
            Town = ToTitleCase(TownName),
            Postcode = PostcodeLocator,
            LocalCustodianCode = LocalCustodianCode,
            Uprn = Uprn
        };
    }
    
    public bool IsCurrentResidential()
    {
        return PostalAddressCode != "N" // is a postal address
               && LpiLogicalStatusCode == "1" // only want current addresses
               && (
                   ClassificationCode.StartsWith("R") // Residential addresses
                   || ClassificationCode.StartsWith("CE") // Educational addresses
                   || ClassificationCode.StartsWith("X") // Dual-use (residential and commercial) addresses
                   || ClassificationCode.StartsWith("M") // Military addresses
               );
    }

    private string ToTitleCase(string text)
    {
        if (text is null)
        {
            return null;
        }
        
        // ToTitleCase doesn't change words in all capitals as it assumes they are acronyms, so we have to first change
        // the text to lower case.
        return GbTextInfo.ToTitleCase(text.ToLower());
    }

    // Properties from the "LPI Output" section of https://osdatahub.os.uk/docs/places/technicalSpecification
    [JsonProperty("UPRN")]
    public string Uprn { get; set; }
    
    [JsonProperty("ADDRESS")]
    public string Address { get; set; }
    
    [JsonProperty("LANGUAGE")]
    public string Language { get; set; }
    
    [JsonProperty("USRN")]
    public string Usrn { get; set; }
    
    [JsonProperty("LPI_KEY")]
    public string LpiKey { get; set; }
    
    [JsonProperty("LEVEL")]
    public string Level { get; set; }
    
    [JsonProperty("ORGANISATION")]
    public string Organisation { get; set; }
    
    [JsonProperty("SAO_START_NUMBER")]
    public string SaoStartNumber { get; set; }
    
    [JsonProperty("SAO_START_SUFFIX")]
    public string SaoStartSuffix { get; set; }
    
    [JsonProperty("SAO_END_NUMBER")]
    public string SaoEndNumber { get; set; }
    
    [JsonProperty("SAO_END_SUFFIX")]
    public string SaoEndSuffix { get; set; }
    
    [JsonProperty("SAO_TEXT")]
    public string SaoText { get; set; }
    
    [JsonProperty("PAO_START_NUMBER")]
    public string PaoStartNumber { get; set; }
    
    [JsonProperty("PAO_START_SUFFIX")]
    public string PaoStartSuffix { get; set; }
    
    [JsonProperty("PAO_END_NUMBER")]
    public string PaoEndNumber { get; set; }
    
    [JsonProperty("PAO_END_SUFFIX")]
    public string PaoEndSuffix { get; set; }
    
    [JsonProperty("PAO_TEXT")]
    public string PaoText { get; set; }
    
    [JsonProperty("STREET_DESCRIPTION")]
    public string StreetDescription { get; set; }
    
    [JsonProperty("LOCALITY_NAME")]
    public string LocalityName { get; set; }
    
    [JsonProperty("TOWN_NAME")]
    public string TownName { get; set; }
    
    [JsonProperty("ADMINISTRATIVE_AREA")]
    public string AdministrativeArea { get; set; }
    
    [JsonProperty("AREA_NAME")]
    public string AreaName { get; set; }
    
    [JsonProperty("POSTCODE_LOCATOR")]
    public string PostcodeLocator { get; set; }
    
    [JsonProperty("RPC")]
    public string Rpc { get; set; }
    
    [JsonProperty("X_COORDINATE")]
    public double XCoordinate { get; set; }
    
    [JsonProperty("Y_COORDINATE")]
    public double YCoordinate { get; set; }
    
    [JsonProperty("LNG")]
    public double? Lng { get; set; }
    
    [JsonProperty("LAT")]
    public double? Lat { get; set; }
    
    [JsonProperty("STATUS")]
    public string Status { get; set; }
    
    [JsonProperty("MATCH")]
    public string Match { get; set; }
    
    [JsonProperty("MATCH_DESCRIPTION")]
    public string MatchDescription { get; set; }
    
    [JsonProperty("LOCAL_CUSTODIAN_CODE")]
    public string LocalCustodianCode { get; set; }
    
    [JsonProperty("LOCAL_CUSTODIAN_CODE_DESCRIPTION")]
    public string LocalCustodianCodeDescription { get; set; }
    
    [JsonProperty("CLASSIFICATION_CODE")]
    public string ClassificationCode { get; set; }
    
    [JsonProperty("CLASSIFICATION_CODE_DESCRIPTION")]
    public string ClassificationCodeDescription { get; set; }
    
    [JsonProperty("POSTAL_ADDRESS_CODE")]
    public string PostalAddressCode { get; set; }
    
    [JsonProperty("POSTAL_ADDRESS_CODE_DESCRIPTION")]
    public string PostalAddressCodeDescription { get; set; }
    
    [JsonProperty("STREET_STATE_CODE")]
    public string StreetStateCode { get; set; }
    
    [JsonProperty("STREET_STATE_CODE_DESCRIPTION")]
    public string StreetStateCodeDescription { get; set; }
    
    [JsonProperty("STREET_CLASSIFICATION_CODE")]
    public string StreetClassificationCode { get; set; }
    
    [JsonProperty("STREET_CLASSIFICATION_CODE_DESCRIPTION")]
    public string StreetClassificationCodeDescription { get; set; }
    
    [JsonProperty("LOGICAL_STATUS_CODE")]
    public string LogicalStatusCode { get; set; }
    
    [JsonProperty("BLPU_STATE_CODE")]
    public string BlpuStateCode { get; set; }
    
    [JsonProperty("BLPU_STATE_CODE_DESCRIPTION")]
    public string BlpuStateCodeDescription { get; set; }
    
    [JsonProperty("TOPOGRAPHY_LAYER_TOID")]
    public string TopographyLayerToid { get; set; }
    
    [JsonProperty("PARENT_UPRN")]
    public string ParentUprn { get; set; }
    
    [JsonProperty("LAST_UPDATE_DATE")]
    public string LastUpdateDate { get; set; }
    
    [JsonProperty("ENTRY_DATE")]
    public string EntryDate { get; set; }
    
    [JsonProperty("LEGAL_NAME")]
    public string LegalName { get; set; }
    
    [JsonProperty("BLPU_STATE_DATE")]
    public string BlpuStateDate { get; set; }
    
    [JsonProperty("LPI_LOGICAL_STATUS_CODE")]
    public string LpiLogicalStatusCode { get; set; }
    
    [JsonProperty("LPI_LOGICAL_STATUS_CODE_DESCRIPTION")]
    public string LpiLogicalStatusCodeDescription { get; set; }
}
