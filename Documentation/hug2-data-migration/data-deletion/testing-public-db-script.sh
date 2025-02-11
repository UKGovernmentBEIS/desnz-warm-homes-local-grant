#!/bin/bash

# Function to run psql commands
run_and_print_count_query() {
    local query="$1"
    local description="$2"
    echo "=== Count For $description Table ==="
    PGPASSWORD=$DB_PASSWORD psql -h $DB_HOST -U mysqladm -d $DB_NAME -c "$query"
    echo
}

run_and_print_count_query "SELECT COUNT(*) FROM public.\"sessioncache\";" 'sessioncache'
run_and_print_count_query "SELECT COUNT(*) FROM public.\"ReferralRequestFollowUps\";" 'ReferralRequestFollowUps'