using System;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using NUnit.Framework;
using WhlgPublicWebsite.Models.Cookies;
using WhlgPublicWebsite.Services.Cookies;

namespace Tests.Website.Services.Cookies;

[TestFixture]
public class CookieServiceTests
{
    private readonly CookieService cookieService;
    private readonly string testKey;
    private const int LatestVersion = 3;

    [DatapointSource] private static CookieServiceTestCase[] CookieServiceTestCases =
    [
        new("Accepted latest cookies", new CookieSettings
        {
            Version = LatestVersion,
            ConfirmationShown = true,
            GoogleAnalytics = true
        }),
        new("Outdated version", new CookieSettings
        {
            Version = LatestVersion - 1,
            ConfirmationShown = true,
            GoogleAnalytics = true
        }),
        new("Rejected analytics and confirmation shown", new CookieSettings
        {
            Version = LatestVersion,
            ConfirmationShown = true,
            GoogleAnalytics = false
        }),
        new("Rejected analytics and confirmation not shown", new CookieSettings
        {
            Version = LatestVersion,
            ConfirmationShown = false,
            GoogleAnalytics = false
        }),
        new("Missing cookie", new CookieSettings())
    ];

    public CookieServiceTests()
    {
        var config = new CookieServiceConfiguration
        {
            CookieSettingsCookieName = "cookie_settings",
            CurrentCookieMessageVersion = LatestVersion,
            DefaultDaysUntilExpiry = 365
        };
        var options = Options.Create(config);
        cookieService = new CookieService(options, new NullLogger<CookieService>());
        testKey = cookieService.Configuration.CookieSettingsCookieName;
    }

    [TestCaseSource(nameof(CookieServiceTestCases))]
    public void CanSetResponseCookie(CookieServiceTestCase testCase)
    {
        // Arrange
        var value = testCase.CookieSettings;
        var context = new DefaultHttpContext();
        var response = context.Response;

        // Act
        cookieService.SetCookie(response, testKey, value);

        // Assert
        AssertResponseContainsCookie(response, testKey, value);
    }

    [TestCaseSource(nameof(CookieServiceTestCases))]
    public void CanGetRequestCookieSettings(CookieServiceTestCase testCase)
    {
        // Arrange
        var value = testCase.CookieSettings;
        var context = new DefaultHttpContext();
        var request = context.Request;
        request.Headers.Cookie = $"{testKey}={ConvertObjectToHttpHeaderSrting(value)}";

        // Act
        var success = cookieService.TryGetCookie<CookieSettings>(request, testKey, out var cookie);

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
        var success = cookieService.TryGetCookie<CookieSettings>(request, testKey, out _);

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
        request.Headers.Cookie = $"{testKey}={ConvertObjectToHttpHeaderSrting(value)}";

        // Act
        var success = cookieService.CookieSettingsAreUpToDate(request);

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
        request.Headers.Cookie = $"{testKey}={ConvertObjectToHttpHeaderSrting(value)}";

        // Act
        var success = cookieService.HasAcceptedGoogleAnalytics(request);

        // Assert
        success.Should().Be(analytics);
    }

    [TestCase("/password")]
    [TestCase("/password?returnPath=%2fquestionnaire%2fcountry")]
    public void HidesBannerIfOnPasswordPage(string passwordUrl)
    {
        // Arrange
        var context = new DefaultHttpContext();
        var request = context.Request;
        var response = context.Response;
        request.Path = passwordUrl;

        // Act
        var bannerState = cookieService.GetAndUpdateBannerState(request, response);

        // Assert
        bannerState.Should().Be(BannerState.Hide);
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
        var bannerState = cookieService.GetAndUpdateBannerState(request, response);

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
        request.Headers.Cookie = $"{testKey}={ConvertObjectToHttpHeaderSrting(value)}";

        // Precondition: the only cases we are testing are 'Missing cookie' and 'Outdated version'
        Assume.That(!cookieService.CookieSettingsAreUpToDate(request));

        // Act
        var bannerState = cookieService.GetAndUpdateBannerState(request, response);

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
        request.Headers.Cookie = $"{testKey}={ConvertObjectToHttpHeaderSrting(value)}";

        // Precondition: the only cases we are testing are 'Accepted latest cookies' and 'Rejected analytics and
        // confirmation shown'
        Assume.That(cookieService.CookieSettingsAreUpToDate(request));
        Assume.That(value.ConfirmationShown);

        // Act
        var bannerState = cookieService.GetAndUpdateBannerState(request, response);

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
        request.Headers.Cookie = $"{testKey}={ConvertObjectToHttpHeaderSrting(value)}";

        // Precondition: the only case we are testing is 'Rejected analytics and confirmation not shown'
        Assume.That(cookieService.CookieSettingsAreUpToDate(request));
        Assume.That(!value.ConfirmationShown);

        // Act
        var bannerState = cookieService.GetAndUpdateBannerState(request, response);

        // Assert
        var expectedBannerState = value.GoogleAnalytics ? BannerState.ShowAccepted : BannerState.ShowRejected;
        bannerState.Should().Be(expectedBannerState);
        value.ConfirmationShown = true;
        AssertResponseContainsCookie(response, testKey, value);
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
        public CookieSettings CookieSettings;
        public string Description;

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