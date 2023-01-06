using Newtonsoft.Json;

namespace HerPublicWebsite.BusinessLogic.ExternalServices.EpbEpc;

public class EpbAssessmentsDto
{
    [JsonProperty(PropertyName = "data")]
    public EpbAssessmentsDataDto Data { get; set; }
}

public class EpbAssessmentsDataDto
{
    [JsonProperty(PropertyName = "assessments")]
    public List<EpbAssessmentInformation> Assessments { get; set; }
}

public class EpbAssessmentInformation
{
    [JsonProperty(PropertyName = "epcRrn")]
    public string EpcId { get; set; }
    
    [JsonProperty(PropertyName = "address")]
    public EpbAddressDto Address { get; set; }
}