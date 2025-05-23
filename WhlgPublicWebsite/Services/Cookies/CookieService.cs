﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using WhlgPublicWebsite.Models.Cookies;

namespace WhlgPublicWebsite.Services.Cookies;

public class CookieService(IOptions<CookieServiceConfiguration> options, ILogger<CookieService> logger)
{
    public readonly CookieServiceConfiguration Configuration = options.Value;

    public bool TryGetCookie<T>(HttpRequest request, string cookieName, out T cookie)
    {
        if (request.Cookies.TryGetValue(cookieName, out var cookieString))
        {
            try
            {
                cookie = JsonConvert.DeserializeObject<T>(cookieString);
                return true;
            }
            catch (JsonException)
            {
                logger.LogWarning("There was an error in deserializing the cookie string '{}' to the type '{}'",
                    cookieString, nameof(T));
                // In case of failure, return false as if there was no cookie
            }
        }

        cookie = default;
        return false;
    }

    public bool CookieSettingsAreUpToDate(HttpRequest request)
    {
        return TryGetCookie<CookieSettings>(request, Configuration.CookieSettingsCookieName, out var cookie) &&
               cookie.Version == Configuration.CurrentCookieMessageVersion;
    }

    public bool HasAcceptedGoogleAnalytics(HttpRequest request)
    {
        return CookieSettingsAreUpToDate(request)
               && TryGetCookie<CookieSettings>(request, Configuration.CookieSettingsCookieName, out var cookie)
               && cookie.GoogleAnalytics;
    }

    public BannerState GetAndUpdateBannerState(HttpRequest request, HttpResponse response)
    {
        if (UrlShouldHideCookieBanner(request))
        {
            return BannerState.Hide;
        }

        // Banner is displayed if the most recent cookie version wasn't reviewed.
        if (!CookieSettingsAreUpToDate(request))
        {
            return BannerState.ShowBanner;
        }

        if (TryGetCookie<CookieSettings>(request, Configuration.CookieSettingsCookieName, out var cookie))
        {
            // We don't need to show anything else after showing the confirmation
            if (cookie.ConfirmationShown)
            {
                return BannerState.Hide;
            }

            // Otherwise, update the cookie to only show the confirmation banner once
            var bannerState = cookie.GoogleAnalytics ? BannerState.ShowAccepted : BannerState.ShowRejected;
            cookie.ConfirmationShown = true;
            SetCookie(response, Configuration.CookieSettingsCookieName, cookie);
            return bannerState;
        }

        return BannerState.Hide;
    }

    public void SetCookie<T>(HttpResponse response, string cookieName, T cookie)
    {
        var cookieString = JsonConvert.SerializeObject(cookie);
        response.Cookies.Append(
            cookieName,
            cookieString,
            new CookieOptions
            {
                Secure = true,
                SameSite = SameSiteMode.Lax,
                MaxAge = TimeSpan.FromDays(Configuration.DefaultDaysUntilExpiry),
                HttpOnly = true
            });
    }

    private bool UrlShouldHideCookieBanner(HttpRequest request)
    {
        List<string> ignoredCookieUrlSections =
        [
            "/cookies", // Cookie settings page doesn't display the banner
            "/password" // Password page shouldn't display as requests to hide the cookie are also password protected
        ];

        return ignoredCookieUrlSections.Any(urlSection => request.GetEncodedUrl().Contains(urlSection));
    }
}