using System;
using HerPublicWebsite.BusinessLogic.Services.RegularJobs;
using NUnit.Framework;

namespace Tests.BusinessLogic.Services;

public class DateHelperTests
{
    private DateTime todayValue;
    private DateHelper dateHelper;

    [SetUp]
    public void Setup()
    {
        dateHelper = new DateHelper(() => todayValue);
    }
    
    [Test]
    public void GetStartOfPreviousMonth_WhenCalled_ReturnsAPreviousDate()
    {
        // Arrange
        todayValue = DateTime.Today;
        
        // Act
        var result = dateHelper.GetStartOfPreviousMonth();
        
        // Assert
        Assert.That(result <= DateTime.Today);
    }
    
    [Test]
    public void GetStartOfPreviousMonth_WhenCalled_ReturnsFirstDayOfAMonth()
    {
        // Arrange
        todayValue = DateTime.Today;
        
        // Act
        var result = dateHelper.GetStartOfPreviousMonth();
        
        // Assert
        Assert.AreEqual(result.Day, 1);
    }
    
    [TestCase("2024-10-12", "2024-09-01")]
    [TestCase("2024-09-01", "2024-08-01")]
    [TestCase("2024-02-29", "2024-01-01")] // Leap day tests
    [TestCase("2024-03-29", "2024-02-01")] // Leap day tests
    [TestCase("2024-04-30", "2024-03-01")] // End of differently sized month
    [TestCase("2024-05-31", "2024-04-01")] // End of differently sized month
    [TestCase("2024-08-31", "2024-07-01")] // End of same sized month
    [TestCase("2025-01-31", "2024-12-01")] // Across year boundary
    public void GetStartOfPreviousMonth_WhenCalled_ReturnsFirstOfPreviousMonth(
        DateTime testDate, DateTime assertDate)
    {
        // Arrange
        todayValue = testDate;
        
        // Act
        var result = dateHelper.GetStartOfPreviousMonth();
        
        // Assert
        Assert.AreEqual(result, assertDate);
    }
}