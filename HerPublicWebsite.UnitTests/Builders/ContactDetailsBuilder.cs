using HerPublicWebsite.BusinessLogic.Models;

namespace Tests.Builders;

public class ContactDetailsBuilder
{
    private ContactDetails contactDetails;
    
    public ContactDetailsBuilder(int id)
    {
        contactDetails = new ContactDetails
        {
            LaContactEmailAddress = $"contact{id}@example.com",
            FullName = $"Full Name{id}",
            LaContactTelephone = $"{id:D5} 123456"
        };
    }

    public ContactDetails Build()
    {
        return contactDetails;
    }
}
