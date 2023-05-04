using HerPublicWebsite.BusinessLogic.Models.Enums;

namespace HerPublicWebsite.BusinessLogic.Models;

public class ContactDetails
{
    public string FullName { get; set; }
    public string Telephone { get; set; }
    public string Email { get; set; }
    
    public ContactPreference ContactPreference { get; set; }
    
    public bool ConsentedToReferral { get; set; }
    public bool ConsentedToAnswerEmail { get; set; }
    public bool ConsentedToSchemeNotificationEmails { get; set; }
}
