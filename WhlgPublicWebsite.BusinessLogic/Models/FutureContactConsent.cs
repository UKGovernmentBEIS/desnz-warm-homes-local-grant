namespace WhlgPublicWebsite.BusinessLogic.Models;

public class FutureContactConsent : IEntityWithRowVersioning
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }

    public bool ConsentToGrants { get; set; }
    public bool ConsentToAdvice { get; set; }
    public bool ConsentToUpdates { get; set; }

    public bool ContactByEmail { get; set; }
    public bool ContactByPhone { get; set; }
    public bool ContactBySms { get; set; }

    public int? ReferralRequestId { get; set; }
    public ReferralRequest ReferralRequest { get; set; }

    public DateTime CreatedAt { get; set; }

    public uint Version { get; set; }

    public FutureContactConsent()
    {
    }

    public FutureContactConsent(Questionnaire questionnaire)
    {
        Name = questionnaire.FutureContactName;
        Email = questionnaire.FutureContactEmail;
        PhoneNumber = questionnaire.FutureContactPhone;

        ConsentToGrants = questionnaire.FutureConsentToGrants;
        ConsentToAdvice = questionnaire.FutureConsentToAdvice;
        ConsentToUpdates = questionnaire.FutureConsentToUpdates;

        ContactByEmail = questionnaire.FutureContactByEmail;
        ContactByPhone = questionnaire.FutureContactByPhone;
        ContactBySms = questionnaire.FutureContactBySms;

        CreatedAt = DateTime.UtcNow;
    }
}
