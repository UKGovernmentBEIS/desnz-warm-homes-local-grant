using HerPublicWebsite.BusinessLogic.ExternalServices.EpbEpc;
using HerPublicWebsite.BusinessLogic.Models;
using HerPublicWebsite.BusinessLogic.Models.Enums;
using HerPublicWebsite.BusinessLogic.Services.EligiblePostcode;

namespace HerPublicWebsite.BusinessLogic;

public class QuestionnaireUpdater
{
    private readonly IEpcApi epcApi;
    private readonly EligiblePostcodeService eligiblePostcodeService;
    
    public QuestionnaireUpdater(IEpcApi epcApi, EligiblePostcodeService eligiblePostcodeService)
    {
        this.epcApi = epcApi;
        this.eligiblePostcodeService = eligiblePostcodeService;
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
        
        // Try to find an EPC for this property
        if (address.Uprn != null)
        {
            var epcDetails = await epcApi.EpcFromUprnAsync(address.Uprn);
            questionnaire.EpcDetails = epcDetails;
        }
        else
        {
            questionnaire.EpcDetails = null;
        }

        // Check LSOA eligibility
        questionnaire.IsLsoaProperty = eligiblePostcodeService.IsEligiblePostcode(address.Postcode);

        return questionnaire;
    }
    
    public Questionnaire UpdateEpcDetails(Questionnaire questionnaire, EpcDetails epcDetails)
    {
        questionnaire.EpcDetails = epcDetails;
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
}
