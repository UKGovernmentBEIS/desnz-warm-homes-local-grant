using Microsoft.AspNetCore.Mvc;
using HerPublicWebsite.Models.Cookies;
using HerPublicWebsite.Services.Cookies;

namespace HerPublicWebsite.Controllers;

public class CookieController: Controller
{
    private readonly CookieService cookieService;

    public CookieController(CookieService cookieService)
    {
        this.cookieService = cookieService;
    }

    [HttpGet("cookies")]
    public IActionResult CookieSettings_Get(bool changesHaveBeenSaved = false)
    {
        cookieService.TryGetCookie<CookieSettings>(Request, cookieService.Configuration.CookieSettingsCookieName, out var cookie);
        
        var viewModel = new CookieSettingsViewModel
        {
            GoogleAnalytics = cookie?.GoogleAnalytics is true,
            ChangesHaveBeenSaved = changesHaveBeenSaved
        };
        return View("CookieSettings", viewModel);
    }

    [HttpPost("cookies")]
    public IActionResult CookieSettings_Post(CookieSettingsViewModel viewModel)
    {
        var cookieSettings = new CookieSettings
        {
            Version = cookieService.Configuration.CurrentCookieMessageVersion,
            ConfirmationShown = true,
            GoogleAnalytics = viewModel.GoogleAnalytics,
        };
        cookieService.SetCookie(Response, cookieService.Configuration.CookieSettingsCookieName, cookieSettings);
        return CookieSettings_Get(changesHaveBeenSaved: true);
    }

    [HttpPost("cookie-consent")]
    public IActionResult CookieConsent(CookieConsentViewModel cookieConsent)
    {
        if (cookieConsent.Consent == "hide")
        {
            return Redirect(cookieConsent.ReturnUrl);
        }
        var cookiesAccepted = cookieConsent.Consent == "accept";
        var cookieSettings = new CookieSettings
        {
            Version = cookieService.Configuration.CurrentCookieMessageVersion,
            ConfirmationShown = false,
            GoogleAnalytics = cookiesAccepted
        };
        cookieService.SetCookie(Response, cookieService.Configuration.CookieSettingsCookieName, cookieSettings);
        return Redirect(cookieConsent.ReturnUrl);
    }

    [HttpGet("cookie-details")]
    public IActionResult CookieDetails()
    {
        return View("CookieDetails");
    }

}