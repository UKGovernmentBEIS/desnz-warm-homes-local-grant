#!/bin/bash

# Function to run psql commands
run_and_print_psql_count_command() {
    local table_name="$1"
    local query="$2"
    echo -n "Count for $table_name: "
    PGPASSWORD=$DB_PASSWORD psql -h "$DB_HOST" -U mysqladm -d "$DB_NAME" -t -c "$query" | tr -d ' '
}

# Non-personal data
run_and_print_psql_count_command "__EFMigrationsHistory" "SELECT COUNT(*) FROM public.\"__EFMigrationsHistory\""
run_and_print_psql_count_command "Consortia" "SELECT COUNT(*) FROM public.\"Consortia\""
run_and_print_psql_count_command "DataProtectionKeys" "SELECT COUNT(*) FROM public.\"DataProtectionKeys\""
run_and_print_psql_count_command "LocalAuthorities" "SELECT COUNT(*) FROM public.\"LocalAuthorities\""

# All other personal data
run_and_print_psql_count_command "ConsortiumUser" "SELECT COUNT(*) FROM public.\"ConsortiumUser\""
run_and_print_psql_count_command "LocalAuthorityUser" "SELECT COUNT(*) FROM public.\"LocalAuthorityUser\""

# Customer analysis data
run_and_print_psql_count_command "AuditDownloads" "SELECT COUNT(*) FROM public.\"AuditDownloads\""
run_and_print_psql_count_command "CsvFileDownloads" "SELECT COUNT(*) FROM public.\"CsvFileDownloads\""
run_and_print_psql_count_command "Users" "SELECT COUNT(*) FROM public.\"Users\""