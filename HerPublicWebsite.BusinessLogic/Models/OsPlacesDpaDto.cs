using Newtonsoft.Json;

public record class OsPlacesDpaDto
{

    [JsonProperty("UPRN")]
    public string Uprn { get; set; }

    [JsonProperty("ADDRESS")]
    public string Address { get; set; }

    [JsonProperty("ORGANISATION_NAME")]
    public string OrganisationName { get; set; }

    [JsonProperty("BUILDING_NUMBER")]
    public string BuildingNumber { get; set; }

    [JsonProperty("THOROUGHFARE_NAME")]
    public string ThoroughFareName { get; set; }

    [JsonProperty("DEPENDENT_LOCALITY")]
    public string DependentLocality { get; set; }

    [JsonProperty("POST_TOWN")]
    public string PostTown { get; set; }

    [JsonProperty("POSTCODE")]
    public string Postcode { get; set; }

    [JsonProperty("RPC")]
    public string Rpc { get; set; }

    [JsonProperty("X_COORDINATE")]
    public int XCoordinate { get; set; }

    [JsonProperty("Y_COORDINATE")]
    public int YCoordinate { get; set; }

    [JsonProperty("STATUS")]
    public string Status { get; set; }

    [JsonProperty("LOGICAL_STATUS_CODE")]
    public string LogicalStatusCode { get; set; }

    [JsonProperty("CLASSIFICATION_CODE")]
    public string ClassificationCode { get; set; }

    [JsonProperty("CLASSIFICATION_CODE_DESCRIPTION")]
    public string ClassificationCodeDescription { get; set; }

    [JsonProperty("LOCAL_CUSTODIAN_CODE")]
    public int LocalCustodianCode { get; set; }

    [JsonProperty("LOCAL_CUSTODIAN_CODE_DESCRIPTION")]
    public string LocalCustodianCodeDescription { get; set; }

    [JsonProperty("POSTAL_ADDRESS_CODE")]
    public string PostalAddressCode { get; set; }

    [JsonProperty("POSTAL_ADDRESS_CODE_DESCRIPTION")]
    public string PostalAddressCodeDescription { get; set; }

    [JsonProperty("BLPU_STATE_CODE")]
    public string BlpuStateCode { get; set; }

    [JsonProperty("BLPU_STATE_CODE_DESCRIPTION")]
    public string BlpuStateCodeDescription { get; set; }

    [JsonProperty("TOPOGRAPHY_LAYER_TOID")]
    public string TopographyLayerToid { get; set; }

    [JsonProperty("LAST_UPDATE_DATE")]
    public string LastUpdateDate { get; set; }

    [JsonProperty("ENTRY_DATE")]
    public string EntryDate { get; set; }

    [JsonProperty("BLPU_STATE_DATE")]
    public string BlpuStateDate { get; set; }

    [JsonProperty("LANGUAGE")]
    public string Language { get; set; }

    [JsonProperty("MATCH")]
    public double Match { get; set; }

    [JsonProperty("MATCH_DESCRIPTION")]
    public string MatchDescription { get; set; }
}
