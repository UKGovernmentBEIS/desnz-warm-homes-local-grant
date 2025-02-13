# Referral Request Migration Process

This document outlines the process for migrating existing referral requests to the new system.

## 1. Connect to the EC2 instance
Connect to the HUG2 public site container instance by following the instructions [here](https://softwiretech.atlassian.net/wiki/spaces/Support/pages/20606746709/DESNZ+HUG2+Common+Tasks#5.-Accessing-Database) up to step 9.

## 2. Upload IMD3 postcodes
- Manually upload `imd3-postcodes.csv` to the `/imd3-postcodes` folder of the `hug2-<ENV>-data-migration` S3 bucket (if it hasn't already been uploaded).
- This should be just one column of data with the heading `Postcode` - please see `imd3-postcodes.csv.example` for an example of the correct format.

## 3. Copy the scripts
- Manually upload `setup-script.sh`, `testing-script.sh`, `insert-test-data.sh`, and `migration-script.sh` to the `migration-scripts/referral-request-migration` folder of the S3 bucket (if not already there)
- Ensure the scripts are using LF line endings. After cloning, they may be reset to CRLF depending on your git config.
- In the EC2 instance, run the following to copy the scripts over:
```shell
cd /root
mkdir -p data-migration/migration-scripts
apt-get update && apt-get install -y awscli
aws s3 cp --recursive s3://hug2-<ENV>-data-migration/migration-scripts/referral-request-migration /root/data-migration/migration-scripts/
```
Replacing `ENV` with the actual environment.

## 4. Run setup script
- Ensure you have the list of emails of users that objected ready
- Run `env` in the EC2 instance - you will need `Server`, `Database`, and `Password` from `ConnectionStrings__PostgreSQLConnection` in the next step
- Run the setup script to create directories, store DB credentials and create IMD3 table:
```shell
chmod +x data-migration/migration-scripts/setup-script.sh
source data-migration/migration-scripts/setup-script.sh
```

## 5. Insert test data (optional)
This inserts test data that covers all the migration criteria (but doesn't delete existing data):
```shell
chmod +x data-migration/migration-scripts/insert-test-data.sh
data-migration/migration-scripts/insert-test-data.sh
```

## 6. Pre-Testing
- Run the test script:
```shell
chmod +x data-migration/migration-scripts/testing-script.sh
data-migration/migration-scripts/testing-script.sh
```
- Record results in the `referral-request-migration-test-template.xlsx` spreadsheet found on the [data migration Swiki page](https://softwiretech.atlassian.net/wiki/spaces/Support/pages/21481160877/DESNZ+HUG2+Data+Migration).

## 7. Run the migration
- Run the migration script with
```shell 
chmod +x data-migration/migration-scripts/migration-script.sh
data-migration/migration-scripts/migration-script.sh
```

If any steps in the migration fail, the whole transaction will be rolled back, and the DB will remain in the state it was before running the script.

## 8. Post-testing
- Run the test script again
```shell
chmod +x data-migration/migration-scripts/testing-script.sh
data-migration/migration-scripts/testing-script.sh
```
- Record results in the `referral-request-migration-test-template.xlsx` spreadsheet found on the [data migration Swiki page](https://softwiretech.atlassian.net/wiki/spaces/Support/pages/21481160877/DESNZ+HUG2+Data+Migration) and compare to the values in the 'Expected' column.

## 9. Clean up
Run:
```shell
PGPASSWORD=$DB_PASSWORD psql -h $DB_HOST -U mysqladm -d $DB_NAME -c "DROP TABLE IF EXISTS temp_imd3_postcodes;"
rm -r /root/data-migration
```

### 10. Send total referrals migrated to DESNZ (if running on Production)

The migration script also creates a csv containing a count of the number of referrals remaining after the migration and copies it to the root of the S3 bucket as `referral-requests-migrated-count.csv`.

This should be sent to Benjamin.Klein@energysecurity.gov.uk (if running on Production) as the final number of referrals carried over to WH:LG from HUG2.