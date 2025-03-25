#!/bin/bash

# Prompt the user for the OBJECTION_EMAIL
read -p "Enter the email of a user from the list of user objections to carrying over pending referral: " OBJECTION_EMAIL

# Escape single quotes in the email address
OBJECTION_EMAIL_ESCAPED=${OBJECTION_EMAIL//\'/\'\'}

# SQL commands
SQL_COMMANDS=$(cat <<EOF
-- Script to populate the "ReferralRequests" table with test data
BEGIN;

-- Insert test data for Active LAs after 1st Dec
INSERT INTO public."ReferralRequests" (
    "AddressLine1", "AddressPostcode", "CustodianCode", "EpcRating", "IsLsoaProperty", "HasGasBoiler",
    "IncomeBand", "FullName", "ContactEmailAddress", "RequestDate",
    "WasSubmittedToPendingLocalAuthority", "WasSubmittedForFutureGrants", "ReferralWrittenToCsv"
) VALUES
      ('123 Active LA St', 'AC1 1LA', 'ACT001', 7, TRUE, 0, 4, 'John Doe', 'john@example.com',
       '2024-12-15', FALSE, FALSE, TRUE),
      ('456 Active LA St', 'AC2 2LA', 'ACT002', 7, FALSE, 0, 4, 'Jane Smith', 'jane@example.com',
       '2024-12-20', FALSE, FALSE, TRUE),
      ('789 Active LA St', 'AC3 3LA', 'ACT003', 7, TRUE, 0, 4, 'Bob Johnson', 'bob@example.com',
       '2024-12-25', FALSE, FALSE, TRUE);

-- Insert test data for Active LAs before 1st Dec
INSERT INTO "ReferralRequests" (
    "AddressLine1", "AddressPostcode", "CustodianCode", "EpcRating", "IsLsoaProperty", "HasGasBoiler",
    "IncomeBand", "FullName", "ContactEmailAddress", "RequestDate",
    "WasSubmittedToPendingLocalAuthority", "WasSubmittedForFutureGrants", "ReferralWrittenToCsv"
) VALUES
      ('456 Old LA Ave', 'OL2 2LA', 'OLD002', 7, FALSE, 0, 4, 'Jane Smith', 'jane@example.com',
       '2024-11-15', FALSE, FALSE, TRUE),
      ('789 Old LA Ave', 'OL3 3LA', 'OLD003', 7, TRUE, 0, 4, 'Bob Johnson', 'bob@example.com',
       '2024-11-20', FALSE, FALSE, TRUE),
      ('123 Old LA Ave', 'OL1 1LA', 'OLD001', 7, TRUE, 0, 4, 'John Doe', 'john@example.com',
       '2024-11-25', FALSE, FALSE, TRUE);

-- Insert test data for Pending LAs
INSERT INTO "ReferralRequests" (
    "AddressLine1", "AddressPostcode", "CustodianCode", "EpcRating", "IsLsoaProperty", "HasGasBoiler",
    "IncomeBand", "FullName", "ContactEmailAddress", "RequestDate",
    "WasSubmittedToPendingLocalAuthority", "ReferralWrittenToCsv"
) VALUES
      ('789 Pending Rd', 'PE3 3LA', 'PEN003', 7, TRUE, 0, 4, 'Bob Johnson', 'bob@example.com',
       '2024-12-10', TRUE, TRUE),
      ('456 Pending Rd', 'PE2 2LA', 'PEN002', 7, FALSE, 0, 4, 'Jane Smith', 'jane@example.com',
       '2024-12-15', TRUE, TRUE),
      ('123 Pending Rd', 'PE1 1LA', 'PEN001', 7, TRUE, 0, 4, 'John Doe', 'john@example.com',
       '2024-12-20', TRUE, TRUE);

-- Insert test data for Pending LAs (objections)
INSERT INTO "ReferralRequests" (
    "AddressLine1", "AddressPostcode", "CustodianCode", "EpcRating", "IsLsoaProperty", "HasGasBoiler",
    "IncomeBand", "FullName", "ContactEmailAddress", "RequestDate",
    "WasSubmittedToPendingLocalAuthority", "ReferralWrittenToCsv"
) VALUES
      ('101 Objection Ln', 'OB4 4LA', 'OBJ004', 7, FALSE, 0, 4, 'Alice Brown', '$OBJECTION_EMAIL_ESCAPED',
       '2024-12-20', TRUE, TRUE),
      ('789 Objection Rd', 'OB3 3LA', 'OBJ003', 7, TRUE, 0, 4, 'Bob Johnson', '$OBJECTION_EMAIL_ESCAPED',
       '2024-12-25', TRUE, TRUE),
      ('456 Objection Rd', 'OB2 2LA', 'OBJ002', 7, FALSE, 0, 4, 'Jane Smith', '$OBJECTION_EMAIL_ESCAPED',
       '2024-12-30', TRUE, TRUE);

-- Insert test data for WHLG demand capture
INSERT INTO "ReferralRequests" (
    "AddressLine1", "AddressPostcode", "CustodianCode", "EpcRating", "IsLsoaProperty", "HasGasBoiler",
    "IncomeBand", "FullName", "ContactEmailAddress", "RequestDate",
    "WasSubmittedForFutureGrants", "ReferralWrittenToCsv"
) VALUES
      ('202 Future St', 'FU5 5LA', 'FUT005', 7, TRUE, 0, 4, 'Charlie Green', 'charlie@future.com',
       '2024-12-25', TRUE, TRUE),
      ('101 Future Rd', 'FU4 4LA', 'FUT004', 7, FALSE, 0, 4, 'Alice Brown', 'alice@future.com',
       '2024-12-30', TRUE, TRUE),
      ('789 Future Rd', 'FU3 3LA', 'FUT003', 7, TRUE, 0, 4, 'Bob Johnson', 'bob@example.com',
       '2024-11-25', TRUE, TRUE);

-- Insert test data for IMD3
INSERT INTO "ReferralRequests" (
    "AddressLine1", "AddressPostcode", "CustodianCode", "EpcRating", "IsLsoaProperty", "HasGasBoiler",
    "IncomeBand", "FullName", "ContactEmailAddress", "RequestDate",
    "WasSubmittedForFutureGrants", "ReferralWrittenToCsv"
) VALUES
      ('789 IMD3 Rd', 'TS25 3BQ', 'IMD003', 7, TRUE, 0, 1, 'Bob Johnson', 'bob@example.com',
       '2024-12-25', TRUE, TRUE),
      ('456 IMD3 Rd', 'TS5 5BD', 'IMD002', 7, FALSE, 0, 3, 'Jane Smith', 'jane@example.com',
       '2024-12-30', TRUE, TRUE),
      ('123 IMD3 Rd', 'WC2N 4EH', 'IMD001', 7, TRUE, 0, 5, 'John Doe', 'john@example.com',
       '2024-11-25', TRUE, TRUE);

COMMIT;
EOF
)

# Execute the SQL commands
echo "Executing SQL commands..."
echo "$SQL_COMMANDS" | PGPASSWORD=$DB_PASSWORD psql -h "$DB_HOST" -U mysqladm -d "$DB_NAME"

echo "SQL commands executed successfully."