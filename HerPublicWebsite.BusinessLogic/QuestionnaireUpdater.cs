using HerPublicWebsite.BusinessLogic.ExternalServices.EmailSending;
using HerPublicWebsite.BusinessLogic.ExternalServices.EpbEpc;
using HerPublicWebsite.BusinessLogic.Models;
using HerPublicWebsite.BusinessLogic.Models.Enums;
using HerPublicWebsite.BusinessLogic.Services.EligiblePostcode;
using HerPublicWebsite.BusinessLogic.Services.QuestionFlow;
using Microsoft.Extensions.Logging;

namespace HerPublicWebsite.BusinessLogic;

public class QuestionnaireUpdater
{
    private readonly IEpcApi epcApi;
    private readonly IEligiblePostcodeService eligiblePostcodeService;
    private readonly IDataAccessProvider dataAccessProvider;
    private readonly IEmailSender emailSender;
    private readonly IQuestionFlowService questionFlowService;
    private readonly ILogger logger;

    public QuestionnaireUpdater(
        IEpcApi epcApi,
        IEligiblePostcodeService eligiblePostcodeService,
        IDataAccessProvider dataAccessProvider,
        IEmailSender emailSender,
        IQuestionFlowService questionFlowService,
        ILogger<QuestionnaireUpdater> logger
    )
    {
        this.epcApi = epcApi;
        this.eligiblePostcodeService = eligiblePostcodeService;
        this.dataAccessProvider = dataAccessProvider;
        this.emailSender = emailSender;
        this.questionFlowService = questionFlowService;
        this.logger = logger;
    }

    public Questionnaire UpdateCountry(Questionnaire questionnaire, Country country, QuestionFlowStep? entryPoint)
    {
        return UpdateQuestionnaire(q => q.Country = country, questionnaire, QuestionFlowStep.Country, entryPoint);
    }


    public Questionnaire UpdateOwnershipStatus(Questionnaire questionnaire, OwnershipStatus ownershipStatus,
        QuestionFlowStep? entryPoint)
    {
        return UpdateQuestionnaire(q => q.OwnershipStatus = ownershipStatus, questionnaire,
            QuestionFlowStep.OwnershipStatus, entryPoint);
    }

    public async Task<Questionnaire> UpdateAddressAsync(Questionnaire questionnaire, Address address, QuestionFlowStep? entryPoint)
    {
        // Try to find an EPC for this property
        var epcDetails = address.Uprn != null ? await epcApi.EpcFromUprnAsync(address.Uprn) : null;

        var currentPage = address.Uprn != null ? QuestionFlowStep.SelectAddress : QuestionFlowStep.ManualAddress;

        return UpdateQuestionnaire(q =>
            {
                q.Uprn = address.Uprn;
                q.AddressPostcode = address.Postcode;
                q.AddressLine1 = address.AddressLine1;
                q.AddressLine2 = address.AddressLine2;
                q.AddressTown = address.Town;
                q.AddressCounty = address.County;
                q.CustodianCode = address.LocalCustodianCode;
                q.LocalAuthorityConfirmed = string.IsNullOrEmpty(address.LocalCustodianCode) ? null : true;
                
                q.EpcDetails = epcDetails;

                q.EpcDetailsAreCorrect = null;

                // Check LSOA eligibility
                q.IsLsoaProperty = eligiblePostcodeService.IsEligiblePostcode(address.Postcode);
            }, questionnaire, currentPage, entryPoint
        );
    }

    public Questionnaire UpdateGasBoiler(Questionnaire questionnaire, HasGasBoiler hasGasBoiler,
        QuestionFlowStep? entryPoint)
    {
        return UpdateQuestionnaire(q => q.HasGasBoiler = hasGasBoiler, questionnaire, QuestionFlowStep.GasBoiler,
            entryPoint);
    }

    public Questionnaire UpdateEpcIsCorrect(Questionnaire questionnaire, EpcConfirmation? epcIsCorrect,
        QuestionFlowStep? entryPoint)
    {
        return UpdateQuestionnaire(q => q.EpcDetailsAreCorrect = epcIsCorrect, questionnaire,
            QuestionFlowStep.ReviewEpc, entryPoint);
    }

    public Questionnaire UpdateLocalAuthority(Questionnaire questionnaire, string custodianCode,
        QuestionFlowStep? entryPoint)
    {
        return UpdateQuestionnaire(q =>
        {
            q.CustodianCode = custodianCode;
            q.LocalAuthorityConfirmed = null;
        }, questionnaire, QuestionFlowStep.SelectLocalAuthority, entryPoint);
    }

    public Questionnaire UpdateLocalAuthorityIsCorrect(Questionnaire questionnaire, bool? confirmed,
        QuestionFlowStep? entryPoint)
    {
        return UpdateQuestionnaire(q => q.LocalAuthorityConfirmed = confirmed, questionnaire,
            QuestionFlowStep.ConfirmLocalAuthority, entryPoint);
    }
    
    public Questionnaire UpdateAcknowledgedPending(Questionnaire questionnaire, bool? acknowledgedPending,
        QuestionFlowStep? entryPoint)
    {
        return UpdateQuestionnaire(q => q.AcknowledgedPending = acknowledgedPending, questionnaire,
            QuestionFlowStep.Pending, entryPoint);
    }

    public Questionnaire UpdateAcknowledgedFutureReferral(Questionnaire questionnaire, bool? acknowledgedFutureReferral,
        QuestionFlowStep? entryPoint)
    {
        return UpdateQuestionnaire(q => q.AcknowledgedFutureReferral = acknowledgedFutureReferral, questionnaire,
            QuestionFlowStep.TakingFutureReferrals, entryPoint);
    }
    
    public Questionnaire UpdateHouseholdIncome(Questionnaire questionnaire, IncomeBand incomeBand,
        QuestionFlowStep? entryPoint)
    {
        return UpdateQuestionnaire(q => q.IncomeBand = incomeBand, questionnaire, QuestionFlowStep.HouseholdIncome,
            entryPoint);
    }

    public async Task<Questionnaire> GenerateReferralAsync(Questionnaire questionnaire, string name,
        string emailAddress, string telephone)
    {
        questionnaire.LaCanContactByEmail = !string.IsNullOrEmpty(emailAddress);
        questionnaire.LaCanContactByPhone = !string.IsNullOrEmpty(telephone);
        questionnaire.LaContactEmailAddress = emailAddress;
        questionnaire.LaContactTelephone = telephone;
        questionnaire.LaContactName = name;

        var referralRequest = new ReferralRequest(questionnaire);
        referralRequest = await dataAccessProvider.PersistNewReferralRequestAsync(referralRequest);

        questionnaire.ReferralCode = referralRequest.ReferralCode;
        questionnaire.ReferralCreated = referralRequest.RequestDate;

        if (!string.IsNullOrEmpty(emailAddress))
        {
            if (questionnaire.LocalAuthorityHug2Status == LocalAuthorityData.Hug2Status.Pending)
            {
                emailSender.SendReferenceCodeEmailForPendingLocalAuthority(emailAddress, name, referralRequest);
            }
            else
            {
                emailSender.SendReferenceCodeEmailForLiveLocalAuthority(emailAddress, name, referralRequest);
            }
        }

        try
        {
            var perReferralReport = new PerReferralReport(referralRequest);
            await dataAccessProvider.PersistPerReferralReportAsync(perReferralReport);
        }
        catch (Exception e)
        {
            logger.LogError("Couldn't generate per referral report: {}", e.Message);
        }

        return questionnaire;
    }

    public async Task<Questionnaire> GenerateAnonymisedReportAsync(Questionnaire questionnaire)
    {
        try
        {
            var anonymisedReport = new AnonymisedReport(questionnaire);
            await dataAccessProvider.PersistAnonymisedReportAsync(anonymisedReport);
        }
        catch (Exception e)
        {
            logger.LogError("Couldn't generate anonymised report: {}", e.Message);
        }

        return questionnaire;
    }

    public async Task<Questionnaire> RecordNotificationConsentAsync(Questionnaire questionnaire, bool consentGranted)
    {
        questionnaire.NotificationConsent = consentGranted;
        questionnaire.NotificationEmailAddress = consentGranted ? questionnaire.LaContactEmailAddress : null;

        var notificationContactDetails = new NotificationDetails(questionnaire);
        await dataAccessProvider.PersistNotificationConsentAsync(questionnaire.ReferralCode, notificationContactDetails);

        return questionnaire;
    }

    public async Task<Questionnaire> RecordNotificationConsentAsync(Questionnaire questionnaire, bool consentGranted,
        string emailAddress)
    {
        questionnaire.NotificationConsent = consentGranted;
        questionnaire.NotificationEmailAddress = consentGranted ? emailAddress : null;

        var notificationContactDetails = new NotificationDetails(questionnaire);
        await dataAccessProvider.PersistNotificationConsentAsync(null, notificationContactDetails);

        return questionnaire;
    }

    public async Task<Questionnaire> RecordConfirmationAndNotificationConsentAsync(
        Questionnaire questionnaire,
        bool notificationConsentGranted,
        string notificationEmailAddress,
        bool confirmationConsentGranted,
        string confirmationEmailAddress)
    {
        questionnaire.NotificationConsent = notificationConsentGranted;
        questionnaire.NotificationEmailAddress = notificationConsentGranted ? notificationEmailAddress : null;
        questionnaire.ConfirmationConsent = confirmationConsentGranted;
        questionnaire.ConfirmationEmailAddress = confirmationConsentGranted ? confirmationEmailAddress : null;

        var notificationContactDetails = new NotificationDetails(questionnaire);
        await dataAccessProvider.PersistNotificationConsentAsync(questionnaire.ReferralCode, notificationContactDetails);

        if (confirmationConsentGranted)
        {
            if (questionnaire.LocalAuthorityHug2Status == LocalAuthorityData.Hug2Status.Pending)
            {
                emailSender.SendReferenceCodeEmailForPendingLocalAuthority(
                    confirmationEmailAddress, 
                    questionnaire.LaContactName,                 
                    new ReferralRequest(questionnaire));
            }
            else
            {
                emailSender.SendReferenceCodeEmailForLiveLocalAuthority(
                    confirmationEmailAddress, 
                    questionnaire.LaContactName, 
                    new ReferralRequest(questionnaire));
            }
        }

        return questionnaire;
    }
    
    public Questionnaire RecordSessionId(Questionnaire questionnaire, int sessionId)
    {
        questionnaire.SessionId = sessionId;
        
        return questionnaire;
    }

    public Questionnaire UpdateQuestionnaire(Action<Questionnaire> update, Questionnaire questionnaire,
        QuestionFlowStep currentPage, QuestionFlowStep? entryPoint = null)
    {
        if ((entryPoint is not null) && questionnaire.UneditedData is null)
        {
            questionnaire.CreateUneditedData();
        }

        update(questionnaire);

        var nextStep = questionFlowService.NextStep(currentPage, questionnaire, entryPoint);

        if ((entryPoint is not null) && nextStep is QuestionFlowStep.CheckAnswers)
        {
            questionnaire.CommitEdits();
        }

        return questionnaire;
    }
}