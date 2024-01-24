using System;
using System.Threading.Tasks;
using FluentAssertions;
using HerPublicWebsite.BusinessLogic;
using HerPublicWebsite.BusinessLogic.Services.CsvFileCreator;
using NUnit.Framework;
using HerPublicWebsite.BusinessLogic.Services.ReferralFollowUp;
using Moq;
using RichardSzalay.MockHttp;
using HerPublicWebsite.BusinessLogic.ExternalServices.Common;

namespace Tests.BusinessLogic.Services;

[TestFixture]
public class ReferralFollowUpServiceTests
{
    private IReferralFollowUpService referralFollowUpService;
    private Mock<IDataAccessProvider> mockDataAccessProvider;
    private MockHttpMessageHandler mockHttpHandler;
    
    [SetUp]
    public void Setup()
    {
        mockDataAccessProvider = new Mock<IDataAccessProvider>();
        referralFollowUpService = new ReferralFollowUpService(mockDataAccessProvider.Object, new CsvFileCreator());
        mockHttpHandler = new MockHttpMessageHandler();
        HttpRequestHelper.handler = mockHttpHandler;
    }
    
    [Test]
    public async Task AddWorkingDaysToDateTime_WhenCalledOnADayFollowingABankHoliday_ReturnsThoseReferrals()
    {
        // Arrange
        DateTime initialDateTime = new DateTime(2023, 03, 23);
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
        var newDateTime = await referralFollowUpService.AddWorkingDaysToDateTime(initialDateTime, -10);


        // Assert
        newDateTime.Should().BeSameDateAs(new DateTime(2023, 03, 08));
    }
}
