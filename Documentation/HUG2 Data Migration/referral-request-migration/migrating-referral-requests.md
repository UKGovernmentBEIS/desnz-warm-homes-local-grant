# Migrating referral requests

This document outlines the process for migrating existing referral requests to the new system.

## Connect to the DB
Connect to the HUG2 external site container instance by following these instructions [here](https://softwiretech.atlassian.net/wiki/spaces/Support/pages/20606746709/DESNZ+HUG2+Common+Tasks#5.-Accessing-Database) up to step 9.

Create folders:
```
mkdir -p data-migration
```

Copy IMD3 postcodes from S3 to EC2 container:
```
aws s3 cp s3://your-bucket-name/imd3_postcodes.csv /app/data-migration/imd3_postcodes.csv
```

Update and connect to psql
```
apt-get update && apt-get install -y postgresql-client
env --to get connection string and credentials
psql -h [Server name from the connection string] -U mysqladm -d rdsPrs
```

### Active LAs
Ensure referrals from 1st December are re-exported to S3:
```
UPDATE public."ReferralRequests"
SET "ReferralWrittenToCSV" = FALSE
WHERE "ReferralWrittenToCSV" = TRUE
AND "RequestDate" > '2024-12-01'
AND "WasSubmittedToPendingLocalAuthority" = FALSE
AND "WasSubmittedForFutureGrants" = FALSE;
```
Ensure referrals before 1st December are deleted:
```
DELETE FROM public."ReferralRequests"
WHERE "RequestDate" < '2024-12-01'
AND "WasSubmittedToPendingLocalAuthority" = FALSE
AND "WasSubmittedForFutureGrants" = FALSE;
```

### Pending LAs
Delete pending referrals if user objected to being carried forward:
```
DELETE FROM public."ReferralRequests"
WHERE "WasSubmittedToPendingLocalAuthority" = TRUE
AND "ContactEmailAddress" IN ('email1@example.com', 'email2@example.com', 'email3@example.com');
```

Ensure all other pending referrals are re-exported to S3:
```
UPDATE public."ReferralRequests"
SET "ReferralWrittenToCSV" = FALSE
WHERE "ReferralWrittenToCSV" = TRUE
AND "WasSubmittedToPendingLocalAuthority" = TRUE
AND "ContactEmailAddress" NOT IN ('email1@example.com', 'email2@example.com', 'email3@example.com');
```

### WHLG demand capture
Ensure all WHLG demand capture referrals are re-exported to S3:
Important - make sure this is done AFTER the deletion of active referrals before 1st December above.
```
UPDATE public."ReferralRequests"
SET "ReferralWrittenToCSV" = FALSE
AND "WasSubmittedForFutureGrants" = FALSE
WHERE "WasSubmittedForFutureGrants" = TRUE;
```

### IMD3 properties
Create temp table of IMD3 postcodes:
```
-- Create the temporary table
CREATE TEMPORARY TABLE temp_imd3_postcodes (
    postcode VARCHAR(10)
);

-- Copy data from CSV into the table
\COPY temp_imd3_postcodes(postcode) FROM '/app/data-migration/imd3_postcodes.csv' WITH CSV HEADER;

-- Verify the data
SELECT COUNT(*) FROM temp_imd3_postcodes;
```
Remove referrals that previously qualified only because they had an IMD3 postcode:
```
DELETE FROM ReferralRequests
WHERE EXISTS (
    SELECT 1 FROM temp_imd3_postcodes t
    WHERE t.postcode = public."ReferralRequests"."AddressPostcode"
)
AND IncomeBand IN (1,3,5);
```
Delete temporary table after:
```
DROP TEMPORARY TABLE IF EXISTS temp_imd3_postcodes;
```

### Count all referrals carried over
The remaining referrals will all be carried over, therefore count them to get final migrated number:
```
\copy (SELECT * FROM public."AuditDownloads") TO '/app/data-migration/referrals-migrated-count.csv' CSV HEADER;
```

Copy to S3:
```
\q
cd data-migration
aws s3 cp "referrals-migrated-count.csv" s3://<bucket-url>
```