#!/bin/bash

# Function to run psql commands
run_psql_command() {
    PGPASSWORD=$DB_PASSWORD psql -h "$DB_HOST" -U mysqladm -d "$DB_NAME" -c "$1"
}

echo "Exporting data from the DB..."

# Non-personal data
run_psql_command "\copy (SELECT * FROM public.\"__EFMigrationsHistory\") TO '/app/data-archive/non-personal-data/__EFMigrationsHistory.csv' CSV HEADER;"
run_psql_command "\copy (SELECT * FROM public.\"Consortia\") TO '/app/data-archive/non-personal-data/Consortia.csv' CSV HEADER;"
run_psql_command "\copy (SELECT * FROM public.\"DataProtectionKeys\") TO '/app/data-archive/non-personal-data/DataProtectionKeys.csv' CSV HEADER;"
run_psql_command "\copy (SELECT * FROM public.\"LocalAuthorities\") TO '/app/data-archive/non-personal-data/LocalAuthorities.csv' CSV HEADER;"

# All other personal data
run_psql_command "\copy (SELECT * FROM public.\"ConsortiumUser\") TO '/app/data-archive/all-other-personal-data/ConsortiumUser.csv' CSV HEADER;"
run_psql_command "\copy (SELECT * FROM public.\"LocalAuthorityUser\") TO '/app/data-archive/all-other-personal-data/LocalAuthorityUser.csv' CSV HEADER;"

# Customer analysis data
run_psql_command "\copy (SELECT * FROM public.\"AuditDownloads\") TO '/app/data-archive/customer-insight-analysis-data/AuditDownloads.csv' CSV HEADER;"
run_psql_command "\copy (SELECT * FROM public.\"CsvFileDownloads\") TO '/app/data-archive/customer-insight-analysis-data/CsvFileDownloads.csv' CSV HEADER;"
run_psql_command "\copy (SELECT * FROM public.\"Users\") TO '/app/data-archive/customer-insight-analysis-data/Users.csv' CSV HEADER;"

# Zip files
echo "Zipping exported data..."
cd data-archive
for dir in non-personal-data all-other-personal-data customer-insight-analysis-data; do
    zip -r --encrypt "$dir.zip" "$dir/" -P "$DATA_FILE_PASSWORD"
done

# Copy to S3
echo "Copying zipped files to S3..."
for file in non-personal-data.zip all-other-personal-data.zip customer-insight-analysis-data.zip; do
    aws s3 cp "$file" "s3://$S3_BUCKET/data-archive/portal-db/"
done

# Clean up
echo "Cleaning up..."
cd ..
rm -rf data-archive

echo "Data export and archiving complete."