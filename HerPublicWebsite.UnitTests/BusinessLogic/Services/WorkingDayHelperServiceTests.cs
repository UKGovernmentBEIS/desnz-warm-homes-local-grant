using System;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using RichardSzalay.MockHttp;
using HerPublicWebsite.BusinessLogic.ExternalServices.Common;
using HerPublicWebsite.BusinessLogic.Services.WorkingDayHelper;

namespace Tests.BusinessLogic.Services;

[TestFixture]
public class WorkingDayHelperServiceTests
{
    private IWorkingDayHelperService workingDayHelperService;
    private MockHttpMessageHandler mockHttpHandler;
    
    [SetUp]
    public void Setup()
    {
        workingDayHelperService = new WorkingDayHelperService();
        mockHttpHandler = new MockHttpMessageHandler();
        HttpRequestHelper.handler = mockHttpHandler;
    }
    
    [Test]
    public async Task AddWorkingDaysToDateTime_WhenCalledOnADayFollowingABankHoliday_CorrectlyCalculatesANewDate()
    {
        // Arrange
        var initialDateTime = new DateTime(2023, 03, 23);
        var bankHolidayResponse = await File.ReadAllTextAsync("MockHttpResponses/bank-holidays.json");
        mockHttpHandler.Expect("https://www.gov.uk/bank-holidays.json")
            .Respond("application/json", bankHolidayResponse
        );
        // Act
        var newDateTime = await workingDayHelperService.AddWorkingDaysToDateTime(initialDateTime, -10);

        // Assert
        newDateTime.Should().BeSameDateAs(new DateTime(2023, 03, 08));
    }
}
