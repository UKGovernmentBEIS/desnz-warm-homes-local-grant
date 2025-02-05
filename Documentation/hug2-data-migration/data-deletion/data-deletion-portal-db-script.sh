#!/bin/bash

# Function to run psql commands
run_psql_command() {
    PGPASSWORD=$DB_PASSWORD psql -h "$DB_HOST" -U mysqladm -d "$DB_NAME" -c "$1"
}

echo "Deleting data from Portal DB..."

# Truncate tables
run_psql_command "TRUNCATE TABLE AuditDownloads;"
run_psql_command "TRUNCATE TABLE Consortia;"
run_psql_command "TRUNCATE TABLE ConsortiumUser;"
run_psql_command "TRUNCATE TABLE CsvFileDownloads;"
run_psql_command "TRUNCATE TABLE LocalAuthorities;"
run_psql_command "TRUNCATE TABLE LocalAuthorityUser;"
run_psql_command "TRUNCATE TABLE Users;"

echo "Data deletion complete."