using System.Globalization;
using HerPublicWebsite.BusinessLogic.Models;
using Newtonsoft.Json;

namespace HerPublicWebsite.BusinessLogic.ExternalServices.OsPlaces;

public class OsPlacesPostcodeResponseDto
{
    [JsonProperty("results")]
    public List<OsPlacesPostcodeResultDto> Results { get; set; }
    
    [JsonProperty("header")]
    public OsPlacesHeaderDto Header { get; set; }
}

public class OsPlacesHeaderDto
{
    [JsonProperty("totalresults")]
    public int TotalResults { get; set; }
}

public class OsPlacesPostcodeResultDto
{
    [JsonProperty("DPA")]
    public OsPlacesDpaDto Dpa { get; set; }
}

public class OsPlacesDpaDto
{
    private static readonly TextInfo GbTextInfo = new CultureInfo("en-GB").TextInfo;
    public Address Parse()
    {
        return new Address()
        {
            AddressLine1 = $"{BuildingNumber ?? ToTitleCase(BuildingName)} {ToTitleCase(DependentThoroughFareName) ?? ToTitleCase(ThoroughFareName)}",
            AddressLine2 = DependentThoroughFareName is not null ? ToTitleCase(ThoroughFareName) : null,
            Town = ToTitleCase(PostTown),
            Postcode = Postcode,
            LocalCustodianCode = LocalCustodianCode,
            Uprn = Uprn
        };
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

    // Properties from the "DPA Output" section of https://osdatahub.os.uk/docs/places/technicalSpecification
    [JsonProperty("UPRN")]
    public string Uprn { get; set; }
    
    [JsonProperty("UDPRN")]
    public string Udprn { get; set; }
    
    [JsonProperty("ADDRESS")]
    public string Address { get; set; }
    
    [JsonProperty("PO_BOX_NUMBER")]
    public string PoBoxNumber { get; set; }

    [JsonProperty("ORGANISATION_NAME")]
    public string OrganisationName { get; set; }
    
    [JsonProperty("DEPARTMENT_NAME")]
    public string DepartmentName { get; set; }

    [JsonProperty("SUB_BUILDING_NAME")]
    public string SubBuildingNumber { get; set; }
    
    [JsonProperty("BUILDING_NAME")]
    public string BuildingName { get; set; }
    
    [JsonProperty("BUILDING_NUMBER")]
    public string BuildingNumber { get; set; }

    [JsonProperty("DEPENDENT_THOROUGHFARE_NAME")]
    public string DependentThoroughFareName { get; set; }
    
    [JsonProperty("THOROUGHFARE_NAME")]
    public string ThoroughFareName { get; set; }

    [JsonProperty("DOUBLE_DEPENDENT_LOCALITY")]
    public string DoubleDependentLocality { get; set; }
    
    [JsonProperty("DEPENDENT_LOCALITY")]
    public string DependentLocality { get; set; }

    [JsonProperty("POST_TOWN")]
    public string PostTown { get; set; }

    [JsonProperty("POSTCODE")]
    public string Postcode { get; set; }

    [JsonProperty("RPC")]
    public string Rpc { get; set; }

    [JsonProperty("X_COORDINATE")]
    public double? XCoordinate { get; set; }

    [JsonProperty("Y_COORDINATE")]
    public double? YCoordinate { get; set; }
    
    [JsonProperty("LNG")]
    public double? Longitude { get; set; }
    
    [JsonProperty("LAT")]
    public double? Latitude { get; set; }

    [JsonProperty("STATUS")]
    public string Status { get; set; }
    
    [JsonProperty("MATCH")]
    public double? MatchScore { get; set; }

    [JsonProperty("MATCH_DESCRIPTION")]
    public string MatchDescription { get; set; }
    
    [JsonProperty("LANGUAGE")]
    public string Language { get; set; }
    
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
}
