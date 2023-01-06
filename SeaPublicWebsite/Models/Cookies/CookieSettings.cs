using GovUkDesignSystem.GovUkDesignSystemComponents;

namespace SeaPublicWebsite.Models.Cookies;

public class CookieSettings
{
    public int Version { get; set; }
    public bool ConfirmationShown { get; set; }
    public bool GoogleAnalytics { get; set; }
}