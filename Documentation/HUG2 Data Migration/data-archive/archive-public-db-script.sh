#!/bin/bash

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

# Check if the psql command was successful
if [ $? -ne 0 ]; then
    echo "Error: Database export failed"
    exit 1
fi

# Zip files
echo "Zipping exported data..."
for dir in non-personal-data all-other-personal-data customer-insight-analysis-data; do
    zip -r --encrypt "/root/data-archive/$dir.zip" "/root/data-archive/$dir/" -P "$DATA_FILE_PASSWORD"
done

# Copy to S3
echo "Copying zipped files to S3..."
for file in non-personal-data.zip all-other-personal-data.zip customer-insight-analysis-data.zip; do
    aws s3 cp "/root/data-archive/$file" "s3://$S3_BUCKET/data-archive/public-db/"
done

# Clean up
echo "Cleaning up..."
rm -r /root/data-archive

echo "Data export and archiving complete."