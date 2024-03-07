using System;
using HerPublicWebsite.BusinessLogic.Services.RegularJobs;
using NUnit.Framework;

namespace Tests.BusinessLogic.Services;

public class StartOfMonthServiceTests
{
    [Test]
    public void StartOfMonthService_WhenCalled_ReturnsAPreviousDate()
    {
        // arrange
        var date = DateTime.Today;
        var startOfMonthService = new StartOfMonthService(date);
        
        // act
        var result = startOfMonthService.GetStartOfPreviousMonth();
        
        // assert
        Assert.That(result <= date);
    }
}