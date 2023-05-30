using HerPublicWebsite.BusinessLogic.ExternalServices.EpbEpc;
using HerPublicWebsite.BusinessLogic.Models;
using HerPublicWebsite.BusinessLogic.Models.Enums;
using HerPublicWebsite.BusinessLogic.Services.EligiblePostcode;

namespace HerPublicWebsite.BusinessLogic;

public class QuestionnaireUpdater
{
    private readonly IEpcApi epcApi;
    private readonly IEligiblePostcodeService eligiblePostcodeService;
    private readonly IDataAccessProvider dataAccessProvider;

    public QuestionnaireUpdater(
        IEpcApi epcApi, 
        IEligiblePostcodeService eligiblePostcodeService,
        IDataAccessProvider dataAccessProvider)
    {
        this.epcApi = epcApi;
        this.eligiblePostcodeService = eligiblePostcodeService;
        this.dataAccessProvider = dataAccessProvider;
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
        
        questionnaire.Hug2ReferralId = referralRequest.ReferralCode;
        questionnaire.ReferralCreated = referralRequest.RequestDate;

        return questionnaire;
    }

    public async Task<Questionnaire> RecordNotificationConsentAsync(Questionnaire questionnaire, bool consentGranted)
    {
        questionnaire.NotificationConsent = consentGranted;
        questionnaire.NotificationEmailAddress = consentGranted ? questionnaire.LaContactEmailAddress : null;

        var notificationContactDetails = new NotificationDetails(questionnaire);
        await dataAccessProvider.PersistNotificationConsentAsync(questionnaire.Hug2ReferralId, notificationContactDetails);

        return questionnaire;
    }
}
