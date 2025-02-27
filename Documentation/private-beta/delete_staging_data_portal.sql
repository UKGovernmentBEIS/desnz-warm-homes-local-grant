DO LANGUAGE plpgsql $$
    BEGIN
        IF NOW() > '2025-03-18' THEN
            RAISE NOTICE 'This script should only be run during the WH:LG Private Beta Phase on the Staging Environment';
            RAISE NOTICE 'Exiting without making changes...';
            RETURN;
        END IF;

        TRUNCATE 
            "AuditDownloads", 
            "Consortia",
            "ConsortiumUser",
            "CsvFileDownloads",
            "DataProtectionKeys",
            "LocalAuthorities",
            "LocalAuthorityUser",
            "Users";
    END;
$$