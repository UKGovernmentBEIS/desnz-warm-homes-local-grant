namespace WhlgPublicWebsite.Middleware
{
    public class PasswordAuthMiddlewareConfiguration
    {
        public const string ConfigSection = "PasswordAuth";
        
        public string Username { get; set; }
        public string Password { get; set; }
    }
}