using HerPublicWebsite.BusinessLogic.Models;
using HerPublicWebsite.BusinessLogic.Models.Enums;

namespace Tests.Builders;

public class ContactDetailsBuilder
{
    private ContactDetails contactDetails;
    
    public ContactDetailsBuilder(int id)
    {
        contactDetails = new ContactDetails
        {
            ConsentedToAnswerEmail = false,
            ConsentedToReferral = true,
            ConsentedToSchemeNotificationEmails = false,
            ContactPreference = ContactPreference.Email,
            Email = $"contact{id}@example.com",
            FullName = $"Full Name{id}",
            Telephone = $"{id:D5} 123456"
        };
    }

    public ContactDetails Build()
    {
        return contactDetails;
    }
}