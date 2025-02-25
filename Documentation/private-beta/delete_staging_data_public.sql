DO LANGUAGE plpgsql $$
    BEGIN
        IF NOW() > '2025-04-01' THEN
            RAISE NOTICE 'This script should only be run during the WH:LG Private Beta Phase on the Staging Environment';
            RAISE NOTICE 'Exiting without making changes...';
            RETURN;
        END IF;
        
        TRUNCATE "NotificationDetails";
        TRUNCATE "ReferralRequestFollowUps";
        TRUNCATE "ReferralRequests";
        TRUNCATE "sessioncache";
        TRUNCATE "Sessions";
    END;
$$