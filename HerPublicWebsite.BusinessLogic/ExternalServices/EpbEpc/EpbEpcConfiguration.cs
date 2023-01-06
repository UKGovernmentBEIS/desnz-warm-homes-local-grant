namespace HerPublicWebsite.BusinessLogic.ExternalServices.EpbEpc
{
    public class EpbEpcConfiguration
    {
        public const string ConfigSection = "EpbEpc";
        
        public string BaseUrl { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}