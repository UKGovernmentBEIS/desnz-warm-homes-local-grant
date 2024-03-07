using System;
using HerPublicWebsite.BusinessLogic.Services.RegularJobs;
using NUnit.Framework;

namespace Tests.BusinessLogic.Services;

public class StartOfMonthServiceTests
{
    public void StartOfMonthService_WhenCalledWithoutParameter_ReturnsAPreviousDate()
    {
        // arrange
        var startOfMonthService = new StartOfMonthService();
        
        // act
        var result = startOfMonthService.GetStartOfPreviousMonth();
        
        // assert
        Assert.That(result <= DateTime.Today);
    }
    
    [Test]
    public void StartOfMonthService_WhenCalledWithoutParameter_ReturnsFirstDayOfAMonth()
    {
        // arrange
        var startOfMonthService = new StartOfMonthService();
        
        // act
        var result = startOfMonthService.GetStartOfPreviousMonth();
        
        // assert
        Assert.That(result.Day == 1);
    }
    
    [Test]
    public void StartOfMonthService_WhenCalledWithParameter_ReturnsAPreviousDate()
    {
        // arrange
        var date = DateTime.Today;
        var startOfMonthService = new StartOfMonthService(date);
        
        // act
        var result = startOfMonthService.GetStartOfPreviousMonth();
        
        // assert
        Assert.That(result <= date);
    }
    
    [TestCase("2024-10-12", "2024-9-1")]
    [TestCase("2024-9-1", "2024-8-1")]
    [TestCase("2024-2-29", "2024-1-1")] //Leap day tests
    [TestCase("2024-3-29", "2024-2-1")] //Leap day tests
    public void StartOfMonthService_WhenCalledWithParameter_ReturnsFirstOfPreviousMonth(DateTime testDate, DateTime assertDate)
    {
        // arrange
        var startOfMonthService = new StartOfMonthService(testDate);
        
        // act
        var result = startOfMonthService.GetStartOfPreviousMonth();
        
        // assert
        Assert.That(result == assertDate);
    }
}