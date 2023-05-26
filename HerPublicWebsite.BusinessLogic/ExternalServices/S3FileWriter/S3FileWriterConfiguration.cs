using Amazon;

namespace HerPublicWebsite.BusinessLogic.ExternalServices.S3FileWriter;

public class S3FileWriterConfiguration
{
    public const string ConfigSection = "S3";

    public string BucketName { get; set; }

    public string Region { get; set; }
}
