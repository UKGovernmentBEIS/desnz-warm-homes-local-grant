#!/bin/bash

# Function to run SQL query and output results
run_and_print_count_query() {
    local query="$1"
    local description="$2"
    echo "=== $description ==="
    PGPASSWORD=$DB_PASSWORD psql -h $DB_HOST -U mysqladm -d $DB_NAME -c "$query"
    echo
}

echo "Running test queries..."

# Active LAs
run_and_print_count_query "SELECT COUNT(*) FROM public.\"ReferralRequests\" WHERE \"RequestDate\" >= '2024-12-01' AND \"WasSubmittedToPendingLocalAuthority\" = FALSE AND \"WasSubmittedForFutureGrants\" = FALSE;" 'Referrals for active LAs after 1st Dec'
run_and_print_count_query "SELECT COUNT(*) FROM public.\"ReferralRequests\" WHERE \"ReferralWrittenToCsv\" = TRUE AND \"RequestDate\" >= '2024-12-01' AND \"WasSubmittedToPendingLocalAuthority\" = FALSE AND \"WasSubmittedForFutureGrants\" = FALSE;" 'Referrals for exported active LAs after 1st Dec'

run_and_print_count_query "SELECT COUNT(*) FROM public.\"ReferralRequests\" WHERE \"RequestDate\" < '2024-12-01' AND \"WasSubmittedToPendingLocalAuthority\" = FALSE AND \"WasSubmittedForFutureGrants\" = FALSE;" 'Referrals for active LAs before 1st Dec'

# Pending LAs
run_and_print_count_query "SELECT COUNT(*) FROM public.\"ReferralRequests\" WHERE \"WasSubmittedToPendingLocalAuthority\" = TRUE AND \"ContactEmailAddress\" = ANY($USER_OBJECTION_EMAIL_ARRAY);" 'Pending referrals where user objected'

run_and_print_count_query "SELECT COUNT(*) FROM public.\"ReferralRequests\" WHERE \"WasSubmittedToPendingLocalAuthority\" = TRUE;" 'All Pending LA referrals'

run_and_print_count_query "SELECT COUNT(*) FROM public.\"ReferralRequests\" WHERE \"WasSubmittedToPendingLocalAuthority\" = TRUE AND \"ReferralWrittenToCsv\" = TRUE;" 'Exported Pending LA referrals'

# WHLG demand capture
run_and_print_count_query "SELECT COUNT(*) FROM public.\"ReferralRequests\" WHERE \"WasSubmittedForFutureGrants\" = TRUE;" 'All WHLG demand capture referrals'

run_and_print_count_query "SELECT COUNT(*) FROM public.\"ReferralRequests\" WHERE \"ReferralWrittenToCsv\" = TRUE AND \"WasSubmittedForFutureGrants\" = TRUE;" 'Exported WHLG demand capture referrals'

run_and_print_count_query "SELECT COUNT(*) FROM public.\"ReferralRequests\" WHERE \"WasSubmittedForFutureGrants\" = TRUE AND \"RequestDate\" >= '2024-12-01';" 'WHLG demand capture referrals with date >= 2024-12-01'

run_and_print_count_query "SELECT COUNT(*) FROM public.\"ReferralRequests\" WHERE \"WasSubmittedForFutureGrants\" = TRUE AND \"RequestDate\" < '2024-12-01';" 'WHLG demand capture referrals with date < 2024-12-01'
# IMD3 properties
run_and_print_count_query "SELECT COUNT(*) FROM public.\"ReferralRequests\" WHERE \"AddressPostcode\" IN (SELECT \"postcode\" FROM \"temp_imd3_postcodes\") AND \"IncomeBand\" IN ('1','3','5');" 'Referrals that previously qualified only because they had an IMD3 postcode'

run_and_print_count_query "SELECT COUNT(*) FROM public.\"ReferralRequests\" WHERE \"AddressPostcode\" IN ('TS25 3BQ', 'TS5 5BD', 'WC2N 4EH') AND \"IncomeBand\" IN ('1','3','5') AND \"RequestDate\" >= '2024-12-01';" 'Referrals that previously qualified only because they had an IMD3 postcode with date >= 2024-12-01'

run_and_print_count_query "SELECT COUNT(*) FROM public.\"ReferralRequests\" WHERE \"AddressPostcode\" IN ('TS25 3BQ', 'TS5 5BD', 'WC2N 4EH') AND \"IncomeBand\" IN ('1','3','5') AND \"RequestDate\" < '2024-12-01';" 'Referrals that previously qualified only because they had an IMD3 postcode with date < 2024-12-01'

echo "Test queries completed."