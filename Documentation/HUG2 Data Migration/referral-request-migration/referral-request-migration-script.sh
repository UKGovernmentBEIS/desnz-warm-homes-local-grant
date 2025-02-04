#!/bin/bash

# Create folders
mkdir -p data-migration

read -p -r "S3 data migration bucket name: " S3_BUCKET
# Get environment variables
echo "Please enter the database connection details:"
read -p -r "Server: " DB_HOST
read -p -r "Database: " DB_NAME
read -s -r -p "Password: " DB_PASSWORD
echo

# Prompt for email list
echo "Please enter the email addresses of users who objected to their referral being carried over to WH:LG, separated by commas (no []):"
read -r USER_OBJECTION_EMAIL_LIST
USER_OBJECTION_EMAIL_ARRAY="ARRAY['${USER_OBJECTION_EMAIL_LIST//,/\'\'}']"

# Copy IMD3 postcodes from S3 to EC2 container
aws s3 cp s3://"$S3_BUCKET"/imd3_postcodes.csv /app/data-migration/imd3_postcodes.csv

# Update and install PostgreSQL client
apt-get update && apt-get install -y postgresql-client

# Connect to PostgreSQL and run queries
PGPASSWORD=$DB_PASSWORD psql -h "$DB_HOST" -U mysqladm -d "$DB_NAME" << EOF

-- Active LAs: Ensure referrals from 1st December are re-exported to S3
UPDATE public."ReferralRequests"
SET "ReferralWrittenToCSV" = FALSE
WHERE "ReferralWrittenToCSV" = TRUE
AND "RequestDate" > '2024-12-01'
AND "WasSubmittedToPendingLocalAuthority" = FALSE
AND "WasSubmittedForFutureGrants" = FALSE;

-- Active LAs: Ensure referrals before 1st December are deleted
DELETE FROM public."ReferralRequests"
WHERE "RequestDate" < '2024-12-01'
AND "WasSubmittedToPendingLocalAuthority" = FALSE
AND "WasSubmittedForFutureGrants" = FALSE;

-- Pending LAs: Delete pending referrals if user objected to being carried forward
DELETE FROM public."ReferralRequests"
WHERE "WasSubmittedToPendingLocalAuthority" = TRUE
AND "ContactEmailAddress" = ANY($USER_OBJECTION_EMAIL_ARRAY);

-- Pending LAs: Ensure all other pending referrals are re-exported to S3
UPDATE public."ReferralRequests"
SET "ReferralWrittenToCSV" = FALSE
WHERE "ReferralWrittenToCSV" = TRUE
AND "WasSubmittedToPendingLocalAuthority" = TRUE
AND "ContactEmailAddress" != ALL($USER_OBJECTION_EMAIL_ARRAY);

-- WHLG demand capture: Ensure all WHLG demand capture referrals are re-exported to S3
UPDATE public."ReferralRequests"
SET "ReferralWrittenToCSV" = FALSE,
    "WasSubmittedForFutureGrants" = FALSE
WHERE "WasSubmittedForFutureGrants" = TRUE;

-- IMD3 properties: Create temp table of IMD3 postcodes
CREATE TEMPORARY TABLE temp_imd3_postcodes (
    postcode VARCHAR(10)
);

-- Copy data from CSV into the table
\COPY temp_imd3_postcodes(postcode) FROM '/app/data-migration/imd3_postcodes.csv' WITH CSV HEADER;

-- Verify the data
SELECT COUNT(*) FROM temp_imd3_postcodes;

-- Remove referrals that previously qualified only because they had an IMD3 postcode
DELETE FROM public."ReferralRequests"
WHERE EXISTS (
    SELECT 1 FROM temp_imd3_postcodes t
    WHERE t.postcode = public."ReferralRequests"."AddressPostcode"
)
AND "IncomeBand" IN (1,3,5);

-- Delete temporary table
DROP TABLE IF EXISTS temp_imd3_postcodes;

-- Count all referrals carried over
\COPY (SELECT COUNT(*) FROM public."ReferralRequests") TO '/app/data-migration/referrals-migrated-count.csv' CSV HEADER;

EOF

# Copy to S3
aws s3 cp "/app/data-migration/referrals-migrated-count.csv" s3://"$S3_BUCKET"

echo "Data migration completed successfully!"