#!/bin/bash

# Function to run psql commands
run_and_print_count_queryd() {
    local query="$1"
    local description="$2"
    echo "=== Count For $description Table ==="
    PGPASSWORD=$DB_PASSWORD psql -h $DB_HOST -U mysqladm -d $DB_NAME -c "$query"
    echo
}

run_and_print_count_query "SELECT COUNT(*) FROM public.\"AuditDownloads\";" 'AuditDownloads'
run_and_print_count_query "SELECT COUNT(*) FROM public.\"Consortia\";" 'Consortia'
run_and_print_count_query "SELECT COUNT(*) FROM public.\"ConsortiumUser\";" 'ConsortiumUser'
run_and_print_count_query "SELECT COUNT(*) FROM public.\"CsvFileDownloads\";" 'CsvFileDownloads'
run_and_print_count_query "SELECT COUNT(*) FROM public.\"LocalAuthorities\";" 'LocalAuthorities'
run_and_print_count_query "SELECT COUNT(*) FROM public.\"LocalAuthorityUser\";" 'LocalAuthorityUser'
run_and_print_count_query "SELECT COUNT(*) FROM public.\"Users\";" 'Users'