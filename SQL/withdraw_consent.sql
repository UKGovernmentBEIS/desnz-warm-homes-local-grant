-- Revoke consent to receive future notifications
DELETE FROM "NotificationDetails" WHERE "FutureSchemeNotificationEmail" = '<withdrawn email address>';
