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

    public Questionnaire UpdateAddress(Questionnaire questionnaire, Address address)
    {
        questionnaire.Uprn = address.Uprn;
        questionnaire.AddressPostcode = address.AddressPostcode;
        questionnaire.AddressLine1 = address.AddressLine1;
        questionnaire.AddressLine2 = address.AddressLine2;
        questionnaire.AddressLine3 = address.AddressLine3;
        questionnaire.AddressTown = address.AddressTown;
        questionnaire.AddressCounty = address.AddressCounty;
        questionnaire.Uprn = address.Uprn;

        return questionnaire;
    }
}
