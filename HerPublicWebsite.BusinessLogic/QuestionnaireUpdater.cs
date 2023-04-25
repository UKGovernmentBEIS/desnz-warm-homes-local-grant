using HerPublicWebsite.BusinessLogic.Models;
using HerPublicWebsite.BusinessLogic.Models.Enums;

namespace HerPublicWebsite.BusinessLogic;

public class QuestionnaireUpdater
{
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

    public Questionnaire UpdateAddress(Questionnaire questionnaire, OsPlacesResult addressResult)
    {
        questionnaire.Uprn = addressResult.Uprn;
        questionnaire.AddressPostcode = addressResult.Postcode;
        questionnaire.AddressLine1 = $"{addressResult.BuildingNumber} {addressResult.ThoroughFareName}";
        questionnaire.AddressLine2 = addressResult.DependentLocality;
        questionnaire.AddressLine3 = addressResult.OrganisationName;
        questionnaire.AddressTown = addressResult.PostTown;
        return questionnaire;
    }
}
