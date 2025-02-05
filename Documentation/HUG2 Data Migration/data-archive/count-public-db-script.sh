#!/bin/bash

# Function to run psql commands
run_psql_command() {
    local table_name="$1"
    local query="$2"
    echo -n "Count for $table_name: "
    PGPASSWORD=$DB_PASSWORD psql -h "$DB_HOST" -U mysqladm -d "$DB_NAME" -t -c "$query" | tr -d ' '
}

# Non-personal data
run_psql_command "__EFMigrationsHistory" "SELECT COUNT(*) FROM public.\"__EFMigrationsHistory\""
run_psql_command "sessioncache" "SELECT COUNT(*) FROM public.\"sessioncache\""
run_psql_command "DataProtectionKeys" "SELECT COUNT(*) FROM public.\"DataProtectionKeys\""

# All other personal data
run_psql_command "NotificationDetails" "SELECT COUNT(*) FROM public.\"NotificationDetails\""

# Customer analysis data
run_psql_command "ReferralRequestFollowUps" "SELECT COUNT(*) FROM public.\"ReferralRequestFollowUps\""
run_psql_command "ReferralRequests" "SELECT COUNT(*) FROM public.\"ReferralRequests\""
run_psql_command "Sessions" "SELECT COUNT(*) FROM public.\"Sessions\""