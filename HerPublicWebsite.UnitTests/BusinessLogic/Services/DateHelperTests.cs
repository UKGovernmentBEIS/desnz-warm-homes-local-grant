using System;
using HerPublicWebsite.BusinessLogic.Services.RegularJobs;
using NUnit.Framework;

namespace Tests.BusinessLogic.Services;

public class DateHelperTests
{
    [Test]
    public void StartOfMonthService_WhenCalledWithoutParameter_ReturnsAPreviousDate()
    {
        // arrange
        var startOfMonthService = new DateHelper();
        
        // act
        var result = startOfMonthService.GetStartOfPreviousMonth();
        
        // assert
        Assert.That(result <= DateTime.Today);
    }
    
    [Test]
    public void StartOfMonthService_WhenCalledWithoutParameter_ReturnsFirstDayOfAMonth()
    {
        // arrange
        var startOfMonthService = new DateHelper();
        
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
        var startOfMonthService = new DateHelper(() => date);
        
        // act
        var result = startOfMonthService.GetStartOfPreviousMonth();
        
        // assert
        Assert.That(result <= date);
    }
    
    [TestCase("2024-10-12", "2024-9-1")]
    [TestCase("2024-9-1", "2024-8-1")]
    [TestCase("2024-2-29", "2024-1-1")] // Leap day tests
    [TestCase("2024-3-29", "2024-2-1")] // Leap day tests
    [TestCase("2024-4-30", "2024-3-1")] // End of differently sized month
    [TestCase("2024-5-31", "2024-4-1")] // End of differently sized month
    [TestCase("2024-8-31", "2024-7-1")] // End of same sized month
    [TestCase("2025-1-31", "2024-12-1")] // Across year boundary
    public void StartOfMonthService_WhenCalledWithParameter_ReturnsFirstOfPreviousMonth(DateTime testDate, DateTime assertDate)
    {
        // arrange
        var startOfMonthService = new DateHelper(() => testDate);
        
        // act
        var result = startOfMonthService.GetStartOfPreviousMonth();
        
        // assert
        Assert.That(result == assertDate);
    }
}