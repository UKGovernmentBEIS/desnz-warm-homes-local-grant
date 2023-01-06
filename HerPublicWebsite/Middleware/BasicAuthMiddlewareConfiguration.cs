namespace HerPublicWebsite.Middleware
{
    public class BasicAuthMiddlewareConfiguration
    {
        public const string ConfigSection = "BasicAuth";
        
        public string Username { get; set; }
        public string Password { get; set; }
    }
}