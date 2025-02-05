#!/bin/bash

# Function to run psql commands
run_psql_command() {
    local table_name="$1"
    local query="$2"
    echo -n "Count for $table_name: "
    PGPASSWORD=$DB_PASSWORD psql -h "$DB_HOST" -U mysqladm -d "$DB_NAME" -t -c "$query" | tr -d ' '
}

run_psql_command "sessioncache" "SELECT COUNT(*) FROM public.\"sessioncache\""
run_psql_command "ReferralRequestFollowUps" "SELECT COUNT(*) FROM public.\"ReferralRequestFollowUps\""