using System;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using RichardSzalay.MockHttp;
using HerPublicWebsite.BusinessLogic.ExternalServices.Common;
using HerPublicWebsite.BusinessLogic.Services.RegularJobs;

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
    
    [TestCase("2024-01-24", 0, "2024-01-24")] // Zero days change
    [TestCase("2024-01-24", +2, "2024-01-26")] // Adding days
    [TestCase("2024-01-24", -2, "2024-01-22")] // Subtracting days
    [TestCase("2024-01-24", +10, "2024-02-07")] // Over weekends
    [TestCase("2024-05-8", -5, "2024-04-30")] // Over bank holiday
    [TestCase("2024-05-30", -20, "2024-04-30")] // Over multiple bank holidays
    [TestCase("2024-05-12", -3, "2024-05-08")] // Starting during weekend
    [TestCase("2024-05-06", -3, "2024-05-01")] // Starting during bank holiday
    [TestCase("2024-05-15", -3, "2024-05-10")] // Ending  during weekend
    [TestCase("2024-05-09", -3, "2024-05-03")] // Ending  during bank holiday
    [TestCase("2024-05-24 00:05:00", -5, "2024-05-17")] // Starting after midnight

    public async Task AddWorkingDaysToDateTime_WhenCalledOnADayFollowingABankHoliday_CorrectlyCalculatesANewDate(DateTime initialDateTime, int workingDaysToAdd, DateTime expectedResult)
    {
        // Arrange
        var bankHolidayResponse = await File.ReadAllTextAsync("MockHttpResponses/bank-holidays.json");
        mockHttpHandler.Expect("https://www.gov.uk/bank-holidays.json")
            .Respond("application/json", bankHolidayResponse
        );
        // Act
        var newDateTime = await workingDayHelperService.AddWorkingDaysToDateTime(initialDateTime, workingDaysToAdd);

        // Assert
        newDateTime.Should().BeSameDateAs(expectedResult);
    }
}
