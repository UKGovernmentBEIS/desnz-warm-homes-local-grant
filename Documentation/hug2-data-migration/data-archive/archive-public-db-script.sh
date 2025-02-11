#!/bin/bash

check_status_and_exit_if_error() {
    if [ $? -ne 0 ]; then
        echo "Error: $1"
        exit 1
    fi
}
echo "Exporting data from Public DB..."

PGPASSWORD=$DB_PASSWORD psql -h "$DB_HOST" -U mysqladm -d "$DB_NAME" << EOF
BEGIN TRANSACTION ISOLATION LEVEL REPEATABLE READ;

-- Non-personal data
\copy (SELECT * FROM public."__EFMigrationsHistory") TO '/root/data-archive/non-personal-data/__EFMigrationsHistory.csv' CSV HEADER;
\copy (SELECT * FROM public."sessioncache") TO '/root/data-archive/non-personal-data/sessioncache.csv' CSV HEADER;
\copy (SELECT * FROM public."DataProtectionKeys") TO '/root/data-archive/non-personal-data/DataProtectionKeys.csv' CSV HEADER;

-- All other personal data
\copy (SELECT * FROM public."NotificationDetails") TO '/root/data-archive/all-other-personal-data/NotificationDetails.csv' CSV HEADER;

-- Customer analysis data
\copy (SELECT * FROM public."ReferralRequestFollowUps") TO '/root/data-archive/customer-insight-analysis-data/ReferralRequestFollowUps.csv' CSV HEADER;
\copy (SELECT * FROM public."ReferralRequests") TO '/root/data-archive/customer-insight-analysis-data/ReferralRequests.csv' CSV HEADER;
\copy (SELECT * FROM public."Sessions") TO '/root/data-archive/customer-insight-analysis-data/Sessions.csv' CSV HEADER;

COMMIT;
EOF

check_status_and_exit_if_error "Database export failed"

# Zip files
echo "Zipping exported data..."
cd /root/data-archive
for dir in non-personal-data all-other-personal-data customer-insight-analysis-data; do
    zip -r --encrypt "$dir.zip" "$dir/" -P "$DATA_FILE_PASSWORD"
done

check_status_and_exit_if_error "Failed to zip files"

# Copy to S3
echo "Copying zipped files to S3..."
for file in non-personal-data.zip all-other-personal-data.zip customer-insight-analysis-data.zip; do
    aws s3 cp "/root/data-archive/$file" "s3://$S3_BUCKET/data-archive/public-db/"
done

check_status_and_exit_if_error "Failed to copy zipped files to S3"

# Clean up
echo "Cleaning up..."
cd /root
rm -r /root/data-archive

echo "Data export and archiving complete."