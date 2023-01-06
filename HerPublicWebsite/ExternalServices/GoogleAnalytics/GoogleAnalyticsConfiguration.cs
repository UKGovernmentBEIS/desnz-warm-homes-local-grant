namespace HerPublicWebsite.ExternalServices.GoogleAnalytics;

public class GoogleAnalyticsConfiguration
{
    public const string ConfigSection = "GoogleAnalytics";
    
    public string BaseUrl { get; set; }
    public string ApiSecret { get; set; }
    public string MeasurementId { get; set; }
    public string CookieName { get; set; }
}