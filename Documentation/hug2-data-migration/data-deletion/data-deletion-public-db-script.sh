#!/bin/bash

echo "Deleting data from Public DB..."

# Connect to PostgreSQL and run queries
PGPASSWORD=$DB_PASSWORD psql -h "$DB_HOST" -U mysqladm -d "$DB_NAME" << EOF
BEGIN;

TRUNCATE Table "sessioncache", "ReferralRequestFollowUps";

COMMIT;
EOF

echo "Data deletion complete."