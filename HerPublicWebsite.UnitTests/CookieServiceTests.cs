using System;
using NUnit.Framework;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using HerPublicWebsite.Models.Cookies;
using HerPublicWebsite.Services.Cookies;

namespace Tests;

[TestFixture]
public class CookieServiceTests
{
    private CookieService CookieService;
    private string Key;
    private static int LatestVersion = 3;

    [DatapointSource] 
    private static CookieServiceTestCase[] CookieServiceTestCases =
    {
        new("Accepted latest cookies", new()
        {
            Version = LatestVersion,
            ConfirmationShown = true,
            GoogleAnalytics = true
        }),
        new("Outdated version", new()
        {
            Version = LatestVersion - 1,
            ConfirmationShown = true,
            GoogleAnalytics = true
        }),
        new("Rejected analytics and confirmation shown", new()
        {
            Version = LatestVersion,
            ConfirmationShown = true,
            GoogleAnalytics = false
        }),
        new("Rejected analytics and confirmation noy shown", new()
        {
            Version = LatestVersion,
            ConfirmationShown = false,
            GoogleAnalytics = false
        }),
        new("Missing cookie", new()),
    };

    public CookieServiceTests()
    {
        var config = new CookieServiceConfiguration
        {
            CookieSettingsCookieName = "cookie_settings",
            CurrentCookieMessageVersion = LatestVersion,
            DefaultDaysUntilExpiry = 365
        };
        var options = Options.Create(config);
        CookieService = new CookieService(options, new NullLogger<CookieService>());
        Key = CookieService.Configuration.CookieSettingsCookieName;
    }
    
    [TestCaseSource(nameof(CookieServiceTestCases))]
    public void CanSetResponseCookie(CookieServiceTestCase testCase)
    {
        // Arrange
        var value = testCase.CookieSettings;
        var context = new DefaultHttpContext();
        var response = context.Response;
        
        // Act
        CookieService.SetCookie(response, Key, value);
        
        // Assert
        AssertResponseContainsCookie(response, Key, value);
    }
    
    [TestCaseSource(nameof(CookieServiceTestCases))]
    public void CanGetRequestCookieSettings(CookieServiceTestCase testCase)
    {
        // Arrange
        var value = testCase.CookieSettings;
        var context = new DefaultHttpContext();
        var request = context.Request;
        request.Headers.Cookie = $"{Key}={ConvertObjectToHttpHeaderSrting(value)}";
        
        // Act
        var success = CookieService.TryGetCookie<CookieSettings>(request, Key, out var cookie);
        
        // Assert
        success.Should().Be(true);
        cookie.Should().BeEquivalentTo(value);
    }
    
    [Test]
    public void ShouldReturnFalseIfCantGetRequestCookie()
    {
        // Arrange
        var context = new DefaultHttpContext();
        var request = context.Request;
        
        // Act
        var success = CookieService.TryGetCookie<CookieSettings>(request, Key, out _);
        
        // Assert
        success.Should().Be(false);
    }
    
    [TestCaseSource(nameof(CookieServiceTestCases))]
    public void CanCheckIfCookieSettingsVersionMatches(CookieServiceTestCase testCase)
    {
        // Arrange
        var value = testCase.CookieSettings;
        var context = new DefaultHttpContext();
        var request = context.Request;
        request.Headers.Cookie = $"{Key}={ConvertObjectToHttpHeaderSrting(value)}";
        
        // Act
        var success = CookieService.CookieSettingsAreUpToDate(request);
        
        // Assert
        success.Should().Be(value.Version == LatestVersion);
    }
    
    [TestCaseSource(nameof(CookieServiceTestCases))]
    public void CanCheckIfGoogleAnalyticsAreAccepted(CookieServiceTestCase testCase)
    {
        // Arrange
        var value = testCase.CookieSettings;
        var analytics = value.Version == LatestVersion && value.GoogleAnalytics;
        var context = new DefaultHttpContext();
        var request = context.Request;
        request.Headers.Cookie = $"{Key}={ConvertObjectToHttpHeaderSrting(value)}";
        
        // Act
        var success = CookieService.HasAcceptedGoogleAnalytics(request);
        
        // Assert
        success.Should().Be(analytics);
    }
    
    [Test]
    public void HidesBannerIfOnCookiePage()
    {
        // Arrange
        var context = new DefaultHttpContext();
        var request = context.Request;
        var response = context.Response;
        request.Path = "/cookies";

        // Act
        var bannerState = CookieService.GetAndUpdateBannerState(request, response);
        
        // Assert
        bannerState.Should().Be(BannerState.Hide);
    }
    
    [TestCaseSource(nameof(CookieServiceTestCases))]
    public void ShowsBannerIfSettingsAreOutdatedOrMissing(CookieServiceTestCase testCase)
    {
        // Arrange
        var value = testCase.CookieSettings;
        var context = new DefaultHttpContext();
        var request = context.Request;
        var response = context.Response;
        request.Headers.Cookie = $"{Key}={ConvertObjectToHttpHeaderSrting(value)}";
        
        // Precondition
        Assume.That(!CookieService.CookieSettingsAreUpToDate(request));

        // Act
        var bannerState = CookieService.GetAndUpdateBannerState(request, response);
        
        // Assert
        bannerState.Should().Be(BannerState.ShowBanner);
    }
    
    [TestCaseSource(nameof(CookieServiceTestCases))]
    public void HidesBannerIfCookiesWereSetAndConfirmationWasShown(CookieServiceTestCase testCase)
    {
        // Arrange
        var value = testCase.CookieSettings;
        var context = new DefaultHttpContext();
        var request = context.Request;
        var response = context.Response;
        request.Headers.Cookie = $"{Key}={ConvertObjectToHttpHeaderSrting(value)}";
        
        // Precondition
        Assume.That(CookieService.CookieSettingsAreUpToDate(request));
        Assume.That(value.ConfirmationShown);

        // Act
        var bannerState = CookieService.GetAndUpdateBannerState(request, response);
        
        // Assert
        bannerState.Should().Be(BannerState.Hide);
    }
    
    [TestCaseSource(nameof(CookieServiceTestCases))]
    public void ShowsConfirmationBannerAndUpdatesRequestCookieIfItWasNotShownAlready(CookieServiceTestCase testCase)
    {
        // Arrange
        var value = testCase.CookieSettings;
        var context = new DefaultHttpContext();
        var request = context.Request;
        var response = context.Response;
        request.Headers.Cookie = $"{Key}={ConvertObjectToHttpHeaderSrting(value)}";
        
        // Precondition
        Assume.That(CookieService.CookieSettingsAreUpToDate(request));
        Assume.That(!value.ConfirmationShown);

        // Act
        var bannerState = CookieService.GetAndUpdateBannerState(request, response);
        
        // Assert
        var expectedBannerState = value.GoogleAnalytics ? BannerState.ShowAccepted : BannerState.ShowRejected;
        bannerState.Should().Be(expectedBannerState);
        value.ConfirmationShown = true;
        AssertResponseContainsCookie(response, Key, value);
    }

    private void AssertResponseContainsCookie(HttpResponse response, string key, object value)
    {
        response.Headers.SetCookie.ToString().Should().Contain($"{key}={ConvertObjectToHttpHeaderSrting(value)}");
    }

    private string ConvertObjectToHttpHeaderSrting(object o)
    {
        return Uri.EscapeDataString(JsonConvert.SerializeObject(o));
    }
    
    public class CookieServiceTestCase
    {
        public string Description;
        public CookieSettings CookieSettings;

        public CookieServiceTestCase(string description, CookieSettings cookieSettings)
        {
            Description = description;
            CookieSettings = cookieSettings;
        }

        public override string ToString()
        {
            return Description;
        }
    }
}