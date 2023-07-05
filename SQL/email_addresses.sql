-- Get email addresses of users that have consented to receive future notifications
SELECT "FutureSchemeNotificationEmail" FROM "NotificationDetails" WHERE "FutureSchemeNotificationConsent" = TRUE;
