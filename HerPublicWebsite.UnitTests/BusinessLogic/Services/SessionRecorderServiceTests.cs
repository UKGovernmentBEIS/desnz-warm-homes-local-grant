using System;
using System.Threading.Tasks;
using FluentAssertions.Extensions;
using HerPublicWebsite.BusinessLogic;
using HerPublicWebsite.BusinessLogic.Models;
using HerPublicWebsite.BusinessLogic.Services.SessionRecorder;
using Moq;
using NUnit.Framework;
using Tests.Helpers;

namespace Tests.BusinessLogic.Services;

public class SessionRecorderServiceTests
{
    private Mock<IDataAccessProvider> mockDataAccessProvider;
    private Mock<IDateTimeProvider> mockDateTimeProvider;
    private DateTime now;
    private SessionRecorderService sessionRecorderService;

    [SetUp]
    public void Setup()
    {
        now = 1.January(2024);

        mockDataAccessProvider = new Mock<IDataAccessProvider>();
        mockDateTimeProvider = new Mock<IDateTimeProvider>();
        mockDateTimeProvider.Setup(dtp => dtp.Now()).Returns(now);

        sessionRecorderService = new SessionRecorderService(mockDataAccessProvider.Object, mockDateTimeProvider.Object);
    }

    [Test]
    public async Task RecordNewSessionStarted_WhenCalled_PersistsASessionWithCorrectTime()
    {
        // Arrange

        // Act
        await sessionRecorderService.RecordNewSessionStarted();

        // Assert
        mockDataAccessProvider.Verify(dap => dap.PersistSession(It.Is<Session>(s => s.Timestamp == now)), Times.Once);
        mockDataAccessProvider.VerifyNoOtherCalls();
    }

    [Test]
    public async Task
        SetJourneyComplete_WhenCalledWithAQuestionnaireWithASessionId_CallsDataAccessProvidersSetJourneyComplete()
    {
        // Arrange
        const int sessionId = 2;
        var questionnaire = QuestionnaireHelper.InitializeQuestionnaire();
        questionnaire.SessionId = sessionId;

        // Act
        await sessionRecorderService.SetJourneyComplete(questionnaire);

        // Assert
        mockDataAccessProvider.Verify(dap => dap.SetJourneyComplete(sessionId), Times.Once);
        mockDataAccessProvider.VerifyNoOtherCalls();
    }

    [Test]
    public void
        SetJourneyComplete_WhenCalledWithAQuestionnaireWithANullSessionId_DoesNotCallDataAccessProvidersSetJourneyComplete()
    {
        // Arrange
        var questionnaire = QuestionnaireHelper.InitializeQuestionnaire();
        questionnaire.SessionId = null;

        // Act
        var exception =
            Assert.ThrowsAsync<Exception>(async () => await sessionRecorderService.SetJourneyComplete(questionnaire));

        // Assert
        Assert.NotNull(exception);
        Assert.That(exception.Message, Is.EqualTo("Session ID is null at journey completion"));
    }
}