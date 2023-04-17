namespace HerPublicWebsite.BusinessLogic.ExternalServices.EpbEpc
{
    public class EpbEpcConfiguration
    {
        public const string ConfigSection = "EpbEpc";

        public string BaseUrl { get; set; }
        public string Token { get; set; }
    }
}
