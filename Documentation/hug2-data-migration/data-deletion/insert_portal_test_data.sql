-- Script to populate the "ReferralRequests" table with test data

-- Clear existing data
-- TRUNCATE TABLE "AuditDownloads";
-- TRUNCATE TABLE "Consortia";
-- TRUNCATE TABLE "ConsortiumUser";
-- TRUNCATE TABLE "CsvFileDownloads";
-- TRUNCATE TABLE "LocalAuthorities";
-- TRUNCATE TABLE "LocalAuthorityUser";
-- TRUNCATE TABLE "Users";


INSERT INTO public."Consortia" ("ConsortiumCode") VALUES
                                               ('C_0002'),
                                               ('C_0003'),
                                               ('C_0004'), 
                                               ('C_0006'),
                                               ('C_0007'),
                                               ('C_0008'),
                                               ('C_0010'),
                                               ('C_0012'), 
                                               ('C_0013'), 
                                               ('C_0014'),
                                               ('C_0015'),
                                               ('C_0016'),
                                               ('C_0017'),
                                               ('C_0021'),
                                               ('C_0022'),
                                               ('C_0024'),
                                               ('C_0027'),
                                               ('C_0029'),
                                               ('C_0031'),
                                               ('C_0033'), 
                                               ('C_0037'), 
                                               ('C_0038'),
                                               ('C_0039'),
                                               ('C_0044');

INSERT INTO public."LocalAuthorities" ("CustodianCode") VALUES
                                                            (9052),
                                                            (3805),
                                                            (1005),
                                                            (9053),
                                                            (9054),
                                                            (3810),
                                                            (3005),
                                                            (2205),
                                                            (3505);

INSERT INTO public."AuditDownloads" ("CustodianCode", "Year", "Month", "UserEmail", "Timestamp"
    ) VALUES ('9052', 2025, 1, 'example@example.com','2025-01-01 17:19:37.000000'),
             ('3810', 2024, 12, 'example@example.com', '2025-02-02 12:10:40.0000000');

INSERT INTO public."Users" ("EmailAddress", "HasLoggedIn") VALUES 
                                                               ('example@example.com', FALSE), 
                                                               ('example@example.com', TRUE), 
                                                               ('example@example.com', TRUE);

INSERT INTO public."ConsortiumUser" ("ConsortiaId", "UsersId") VALUES (12, 1);

INSERT INTO public."LocalAuthorityUser" ("LocalAuthoritiesId", "UsersId") VALUES (1, 1), (4, 1), (3, 2), (2, 1), (6, 1);

INSERT INTO public."CSVFileDownloads" ("CustodianCode", "Year", "Month", "UserId", "LastDownloaded") VALUES 
                                                                                                         (9052, 2025, 1, 1, '2025-02-04 10:11:30.0000000'),
                                                                                                         (3810, 2024, 11, 2, '2024-12-24 23:12:10.0000000');