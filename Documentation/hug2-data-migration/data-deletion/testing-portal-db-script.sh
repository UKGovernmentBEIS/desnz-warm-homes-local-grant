#!/bin/bash

# Function to run psql commands
run_psql_command() {
    local table_name="$1"
    local query="$2"
    echo -n "Count for $table_name: "
    PGPASSWORD=$DB_PASSWORD psql -h "$DB_HOST" -U mysqladm -d "$DB_NAME" -t -c "$query" | tr -d ' '
}

run_psql_command "AuditDownloads" "SELECT COUNT(*) FROM public.\"AuditDownloads\""
run_psql_command "Consortia" "SELECT COUNT(*) FROM public.\"Consortia\""
run_psql_command "ConsortiumUser" "SELECT COUNT(*) FROM public.\"ConsortiumUser\""
run_psql_command "CsvFileDownloads" "SELECT COUNT(*) FROM public.\"CsvFileDownloads\""
run_psql_command "LocalAuthorities" "SELECT COUNT(*) FROM public.\"LocalAuthorities\""
run_psql_command "LocalAuthorityUser" "SELECT COUNT(*) FROM public.\"LocalAuthorityUser\""
run_psql_command "Users" "SELECT COUNT(*) FROM public.\"Users\""