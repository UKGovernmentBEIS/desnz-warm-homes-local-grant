namespace SeaPublicWebsite.BusinessLogic.ExternalServices.Bre
{
    public class BreConfiguration
    {
        public const string ConfigSection = "Bre";
        
        public string BaseUrl { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}