#!/bin/bash

# Get environment variables
read -p "S3 data migration bucket name e.g. hug2-dev-data-migration: " S3_BUCKET
echo "Please enter the database connection details:"
read -p "Server: " DB_HOST
read -p "Database: " DB_NAME
read -s -p "Password: " DB_PASSWORD
echo
export S3_BUCKET DB_HOST DB_NAME DB_PASSWORD

# Prompt for email list
echo "Please enter the email addresses of users who objected to their referral being carried over to WH:LG, separated by commas (no []):"
read -r USER_OBJECTION_EMAIL_LIST
USER_OBJECTION_EMAIL_ARRAY=""
USER_OBJECTION_EMAIL_ARRAY="ARRAY[$(echo "$USER_OBJECTION_EMAIL_LIST" | sed "s/,/','/g" | sed "s/^/'/;s/$/'/")]"
export USER_OBJECTION_EMAIL_ARRAY

# Copy IMD3 postcodes from S3 to EC2 container
aws s3 cp s3://"$S3_BUCKET"/imd3-postcodes/imd3-postcodes.csv /root/data-migration/imd3-postcodes.csv

# Update and install PostgreSQL client
apt-get update && apt-get install -y postgresql-client

# IMD3 postcodes
PGPASSWORD=$DB_PASSWORD psql -h "$DB_HOST" -U mysqladm -d "$DB_NAME" << EOF
BEGIN;
CREATE TABLE temp_imd3_postcodes (Postcode VARCHAR(10));
\COPY temp_imd3_postcodes(postcode) FROM '/root/data-migration/imd3-postcodes.csv' WITH CSV HEADER;
COMMIT;
EOF

# Check postcodes were successfully copied table
row_count=$(PGPASSWORD=$DB_PASSWORD psql -h "$DB_HOST" -U mysqladm -d "$DB_NAME" -t -c "SELECT COUNT(*) FROM temp_imd3_postcodes;")

# Remove any whitespace from the output
row_count=$(echo "$row_count" | tr -d ' ')

# Check if the count is greater than 0
if [ "$row_count" -gt 0 ]; then
    echo "Successfully copied $row_count rows to temp_imd3_postcodes"
else
    echo "Error: No rows were copied to temp_imd3_postcodes"
    exit 1
fi