﻿namespace WhlgPublicWebsite.BusinessLogic.Models;

public class NotificationDetails : IEntityWithRowVersioning
{
    public string FutureSchemeNotificationEmail { get; set; }
    public bool FutureSchemeNotificationConsent { get; set; }

    public int? ReferralRequestId { get; set; }

    public ReferralRequest ReferralRequest { get; set; }

    public uint Version { get; set; }

    public NotificationDetails()
    {
    }

    public NotificationDetails(Questionnaire questionnaire)
    {
        FutureSchemeNotificationEmail = questionnaire.NotificationEmailAddress;
        FutureSchemeNotificationConsent = questionnaire.NotificationConsent!.Value;
    }
}