using System;
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
        mockHttpHandler.Expect("https://www.gov.uk/bank-holidays.json")
            .Respond("application/json", @"{
  'england-and-wales': {
    'division': 'england-and-wales',
    'events': [
        {
            'title':'New Year’s Day',
            'date':'2023-01-01',
        },
        {
            'title':'Fake Bank Holiday on Weekday',
            'date':'2023-03-20',
        },
        {
            'title':'Fake Holiday on Weekend',
            'date':'2023-03-19',
        }
        ]
    }
}"
        );
        // Act
        var newDateTime = await workingDayHelperService.AddWorkingDaysToDateTime(initialDateTime, -10);

        // Assert
        newDateTime.Should().BeSameDateAs(new DateTime(2023, 03, 08));
    }
}
