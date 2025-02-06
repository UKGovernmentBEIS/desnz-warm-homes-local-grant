#!/bin/bash

# Function to run psql commands
run_psql_command() {
    PGPASSWORD=$DB_PASSWORD psql -h "$DB_HOST" -U mysqladm -d "$DB_NAME" -c "$1"
}

echo "Deleting data from Portal DB..."

# Truncate tables
run_psql_command "TRUNCATE TABLE \"AuditDownloads\", 
                                  \"Consortia\", 
                                  \"ConsortiumUser\", 
                                  \"CsvFileDownloads\", 
                                  \"LocalAuthorities\", 
                                  \"LocalAuthorityUser\", 
                                  \"Users\";"

echo "Data deletion complete."