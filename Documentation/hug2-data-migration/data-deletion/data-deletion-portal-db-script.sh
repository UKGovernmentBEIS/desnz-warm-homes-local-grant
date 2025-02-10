#!/bin/bash

echo "Deleting data from Portal DB..."

# Connect to PostgreSQL and run queries
PGPASSWORD=$DB_PASSWORD psql -h "$DB_HOST" -U mysqladm -d "$DB_NAME" << EOF
BEGIN;

TRUNCATE TABLE "AuditDownloads", 
                "Consortia", 
                "ConsortiumUser", 
                "CsvFileDownloads", 
                "LocalAuthorities", 
                "LocalAuthorityUser", 
                "Users";

COMMIT;
EOF

echo "Data deletion complete."