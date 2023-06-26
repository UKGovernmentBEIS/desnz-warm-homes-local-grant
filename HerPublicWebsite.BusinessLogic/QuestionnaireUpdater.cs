using HerPublicWebsite.BusinessLogic.ExternalServices.EmailSending;
using HerPublicWebsite.BusinessLogic.ExternalServices.EpbEpc;
using HerPublicWebsite.BusinessLogic.Models;
using HerPublicWebsite.BusinessLogic.Models.Enums;
using HerPublicWebsite.BusinessLogic.Services.EligiblePostcode;
using Microsoft.Extensions.Logging;

namespace HerPublicWebsite.BusinessLogic;

public class QuestionnaireUpdater
{
    private readonly IEpcApi epcApi;
    private readonly IEligiblePostcodeService eligiblePostcodeService;
    private readonly IDataAccessProvider dataAccessProvider;
    private readonly IEmailSender emailSender;
    private readonly ILogger logger;

    public QuestionnaireUpdater(
        IEpcApi epcApi,
        IEligiblePostcodeService eligiblePostcodeService,
        IDataAccessProvider dataAccessProvider,
        IEmailSender emailSender,
        ILogger<QuestionnaireUpdater> logger
    )
    {
        this.epcApi = epcApi;
        this.eligiblePostcodeService = eligiblePostcodeService;
        this.dataAccessProvider = dataAccessProvider;
        this.emailSender = emailSender;
        this.logger = logger;
    }

    public Questionnaire UpdateCountry(Questionnaire questionnaire, Country country)
    {
        questionnaire.Country = country;
        return questionnaire;
    }


    public Questionnaire UpdateOwnershipStatus(Questionnaire questionnaire, OwnershipStatus ownershipStatus)
    {
        questionnaire.OwnershipStatus = ownershipStatus;
        return questionnaire;
    }

    public async Task<Questionnaire> UpdateAddressAsync(Questionnaire questionnaire, Address address)
    {
        questionnaire.Uprn = address.Uprn;
        questionnaire.AddressPostcode = address.Postcode;
        questionnaire.AddressLine1 = address.AddressLine1;
        questionnaire.AddressLine2 = address.AddressLine2;
        questionnaire.AddressTown = address.Town;
        questionnaire.AddressCounty = address.County;
        questionnaire.CustodianCode = address.LocalCustodianCode;
        questionnaire.LocalAuthorityConfirmed = string.IsNullOrEmpty(address.LocalCustodianCode) ? null : true;

        // Try to find an EPC for this property
        if (address.Uprn != null)
        {
            questionnaire.EpcDetails = await epcApi.EpcFromUprnAsync(address.Uprn);
        }
        else
        {
            questionnaire.EpcDetails = null;
        }

        questionnaire.EpcDetailsAreCorrect = null;

        // Check LSOA eligibility
        questionnaire.IsLsoaProperty = eligiblePostcodeService.IsEligiblePostcode(address.Postcode);

        return questionnaire;
    }

    public Questionnaire UpdateGasBoiler(Questionnaire questionnaire, HasGasBoiler hasGasBoiler)
    {
        questionnaire.HasGasBoiler = hasGasBoiler;
        return questionnaire;
    }

    public Questionnaire UpdateEpcIsCorrect(Questionnaire questionnaire, bool epcIsCorrect)
    {
        questionnaire.EpcDetailsAreCorrect = epcIsCorrect;

        return questionnaire;
    }

    public Questionnaire UpdateLocalAuthority(Questionnaire questionnaire, string custodianCode)
    {
        questionnaire.CustodianCode = custodianCode;
        questionnaire.LocalAuthorityConfirmed = null;
        return questionnaire;
    }

    public Questionnaire UpdateLocalAuthorityIsCorrect(Questionnaire questionnaire, bool? confirmed)
    {
        questionnaire.LocalAuthorityConfirmed = confirmed;
        return questionnaire;
    }

    public Questionnaire UpdateHouseholdIncome(Questionnaire questionnaire, IncomeBand incomeBand)
    {
        questionnaire.IncomeBand = incomeBand;

        return questionnaire;
    }

    public async Task<Questionnaire> GenerateReferralAsync(Questionnaire questionnaire, string name, string emailAddress, string telephone)
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
            emailSender.SendReferenceCodeEmail
            (
                emailAddress,
                name,
                referralRequest.ReferralCode,
                referralRequest.CustodianCode
            );
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

    public async Task<Questionnaire> RecordNotificationConsentAsync(Questionnaire questionnaire, bool consentGranted, string emailAddress)
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
            emailSender.SendReferenceCodeEmail
            (
                confirmationEmailAddress,
                questionnaire.LaContactName,
                questionnaire.ReferralCode,
                questionnaire.CustodianCode
            );
        }

        return questionnaire;
    }
}
