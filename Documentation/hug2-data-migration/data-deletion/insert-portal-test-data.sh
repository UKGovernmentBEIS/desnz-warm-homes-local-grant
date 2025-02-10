#!/bin/bash

# SQL commands
SQL_COMMANDS=$(cat <<EOF
BEGIN;
-- Script to populate the to-be-emptied Portal tables with test data

-- Clear existing data
TRUNCATE TABLE "AuditDownloads", "Consortia", "ConsortiumUser", "CsvFileDownloads", "LocalAuthorities", "LocalAuthorityUser", "Users";

INSERT INTO public."Consortia" ("Id","ConsortiumCode") VALUES
                                                           (1, 'C_0002'),
                                                           (2, 'C_0003'),
                                                           (3, 'C_0004'),
                                                           (4, 'C_0006'),
                                                           (5, 'C_0007'),
                                                           (6, 'C_0008'),
                                                           (7, 'C_0010'),
                                                           (8, 'C_0012'),
                                                           (9, 'C_0013'),
                                                           (10, 'C_0014'),
                                                           (11, 'C_0015'),
                                                           (12, 'C_0016'),
                                                           (13, 'C_0017'),
                                                           (14, 'C_0021'),
                                                           (15, 'C_0022'),
                                                           (16, 'C_0024'),
                                                           (17, 'C_0027'),
                                                           (18, 'C_0029'),
                                                           (19, 'C_0031'),
                                                           (20, 'C_0033'),
                                                           (21, 'C_0037'),
                                                           (22, 'C_0038'),
                                                           (23, 'C_0039'),
                                                           (24, 'C_0044');

INSERT INTO public."LocalAuthorities" ("Id","CustodianCode") VALUES
                                                                 (1, 9052),
                                                                 (2, 3805),
                                                                 (3, 1005),
                                                                 (4, 9053),
                                                                 (5, 9054),
                                                                 (6, 3810),
                                                                 (7, 3005),
                                                                 (8, 2205),
                                                                 (9, 3505);

INSERT INTO public."AuditDownloads" ("CustodianCode", "Year", "Month", "UserEmail", "Timestamp"
) VALUES ('9052', 2025, 1, 'example@example.com','2025-01-01 17:19:37.000000'),
         ('3810', 2024, 12, 'example@example.com', '2025-02-02 12:10:40.0000000');

INSERT INTO public."Users" ("Id", "EmailAddress", "HasLoggedIn") VALUES
                                                                     (1, 'example@example.com', FALSE),
                                                                     (2, 'example1@example.com', TRUE),
                                                                     (3, 'example2@example.com', TRUE);

INSERT INTO public."ConsortiumUser" ("ConsortiaId", "UsersId") VALUES (12, 1);

INSERT INTO public."LocalAuthorityUser" ("LocalAuthoritiesId", "UsersId") VALUES (1, 1), (4, 1), (3, 2), (2, 1), (6, 1);

INSERT INTO public."CsvFileDownloads" ("CustodianCode", "Year", "Month", "UserId", "LastDownloaded") VALUES
                                                                                                         (9052, 2025, 1, 1, '2025-02-04 10:11:30.0000000'::timestamp),
                                                                                                         (3810, 2024, 11, 2, '2024-12-24 23:12:10.0000000'::timestamp);
COMMIT;
EOF
)

# Execute the SQL commands
echo "Executing SQL commands..."
echo "$SQL_COMMANDS" | PGPASSWORD=$DB_PASSWORD psql -h "$DB_HOST" -U mysqladm -d "$DB_NAME"

echo "SQL commands executed successfully."