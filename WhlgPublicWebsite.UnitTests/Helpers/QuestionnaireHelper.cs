using WhlgPublicWebsite.BusinessLogic.Models;
using WhlgPublicWebsite.BusinessLogic.Models.Enums;

namespace Tests.Helpers;

public static class QuestionnaireHelper
{
    public static Questionnaire InitializeQuestionnaire()
    {
        return new Questionnaire
        {
            Country = Country.England,
            OwnershipStatus = OwnershipStatus.OwnerOccupancy,
            AddressLine1 = "Address Line 1",
            AddressLine2 = "Address Line 2",
            AddressTown = "A Town",
            AddressCounty = "A County",
            AddressPostcode = "PST C0D",
            CustodianCode = "5210",
            LocalAuthorityConfirmed = true,
            AcknowledgedPending = false,
            AcknowledgedFutureReferral = false,
            Uprn = "123456789123",
            EpcDetails = new EpcDetails(),
            EpcDetailsAreCorrect = EpcConfirmation.Yes,
            IsLsoaProperty = true,
            HasGasBoiler = HasGasBoiler.No,
            IncomeBand = IncomeBand.UnderOrEqualTo36000,
            ReferralCreated = default,
            ReferralCode = "WHLG1023",
            LaContactName = "Contact Name",
            LaCanContactByEmail = true,
            LaCanContactByPhone = true,
            LaContactEmailAddress = "person@example.com",
            LaContactTelephone = "07123456789",
            NotificationConsent = true,
            ConfirmationConsent = true,
            NotificationEmailAddress = "person@example.com",
            ConfirmationEmailAddress = "person@example.com",
            UneditedData = new Questionnaire()
        };
    }
}