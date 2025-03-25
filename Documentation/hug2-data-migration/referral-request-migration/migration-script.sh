#!/bin/bash

check_status_and_exit_if_error() {
    if [ $? -ne 0 ]; then
        echo "Error: $1"
        exit 1
    fi
}

# Connect to PostgreSQL and run queries
PGPASSWORD=$DB_PASSWORD psql -h "$DB_HOST" -U mysqladm -d "$DB_NAME" << EOF
BEGIN;

-- Remove constraint on deleting a ReferralRequest with a corresponding entry in NotificationDetails.
-- Add constraint to set ReferralRequestId in NotificationDetails to NULL if a corresponding ReferralRequest is deleted.
ALTER TABLE public."NotificationDetails"
    DROP CONSTRAINT "FK_ContactDetails_ReferralRequests_ReferralRequestId",
    ADD CONSTRAINT "FK_ContactDetails_ReferralRequests_ReferralRequestId"
        FOREIGN KEY ("ReferralRequestId")
            REFERENCES public."ReferralRequests"("Id")
            ON DELETE SET NULL;
            
-- Active LAs: Ensure referrals from 1st December are re-exported to S3
UPDATE public."ReferralRequests"
SET "ReferralWrittenToCsv" = FALSE
WHERE "ReferralWrittenToCsv" = TRUE
AND "RequestDate" >= '2024-12-01'
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
SET "ReferralWrittenToCsv" = FALSE
WHERE "ReferralWrittenToCsv" = TRUE
AND "WasSubmittedToPendingLocalAuthority" = TRUE;

-- WHLG demand capture: Ensure all WHLG demand capture referrals are re-exported to S3
UPDATE public."ReferralRequests"
SET "ReferralWrittenToCsv" = FALSE,
    "WasSubmittedForFutureGrants" = FALSE
WHERE "WasSubmittedForFutureGrants" = TRUE;

-- Remove referrals that previously qualified only because they had an IMD3 postcode
DELETE FROM public."ReferralRequests"
WHERE EXISTS (
    SELECT 1 FROM temp_imd3_postcodes t
    WHERE t.Postcode = public."ReferralRequests"."AddressPostcode"
)
AND "IncomeBand" IN (1,3,5);

-- Count all referrals carried over
\COPY (SELECT COUNT(*) FROM public."ReferralRequests") TO '/root/data-migration/referrals-migrated-count.csv' CSV HEADER;

COMMIT;
EOF

check_status_and_exit_if_error "Referral request migration failed"

# Copy to S3
aws s3 cp "/root/data-migration/referrals-migrated-count.csv" s3://"$S3_BUCKET"/referral-requests-migrated-count.csv
check_status_and_exit_if_error "Failed to copy file to S3"

echo "Data migration completed successfully!"