namespace WhlgPublicWebsite.BusinessLogic.ExternalServices.S3FileWriter;

public class S3Configuration
{
    public const string ConfigSection = "S3";

    public string BucketName { get; set; }

    public string Region { get; set; }
    
    public string LocalDevOnly_AccessKey { get; set; }
    public string LocalDevOnly_SecretKey { get; set; }
    public string LocalDevOnly_ServiceUrl { get; set; }
}
