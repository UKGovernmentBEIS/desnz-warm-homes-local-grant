# Data Deletion Process

This document outlines the process for deleting the contents of several tables, which are not being carried over to the new system.

## Upload the scripts
- Manually upload `setup-script.sh`, `testing-script.sh` and `migration-script.sh` to the `migration-scripts/referral-request-migration` folder of the S3 bucket (if not already there)


## Public DB deletion
### Connect to the EC2 instance & copy scripts
Connect to the Public HUG2 external site container instance by following the instructions [here](https://softwiretech.atlassian.net/wiki/spaces/Support/pages/20606746709/DESNZ+HUG2+Common+Tasks#5.-Accessing-Database) up to step 9.
- In the EC2 instance, run the following to copy the scripts over:
```
aws s3 cp --recursive s3://"$S3_BUCKET"/migration-scripts/data-deletion /app/data-migration/deletion-scripts
```

### Run setup script
- Run `env` in the Public EC2 instance - you will need `Server`, `Database`, and `Password` from `ConnectionStrings__PostgreSQLConnection` in the next step
- Run the setup script to store DB credentials:
```
data-migration/deletion-scripts/setup-script.sh
```

### Pre-Testing
- Run the test script with `data-migration/deletion-scripts/testing-public-db-script.sh`.
- Record results in spreadsheet

### Run the deletion
- Run the deletion script with `data-migration/deletion-scripts/data-deletion-public-db-script.sh`

### Post-testing
- Run the test script again
- Record results in spreadsheet

## Portal DB deletion
### Connect to the EC2 instance & copy scripts
Connect to the Portal HUG2 external site container instance by following the instructions [here](https://softwiretech.atlassian.net/wiki/spaces/Support/pages/20606746709/DESNZ+HUG2+Common+Tasks#5.-Accessing-Database) up to step 9.
- In the EC2 instance, run the following to copy the scripts over:
```
aws s3 cp --recursive s3://"$S3_BUCKET"/migration-scripts/data-deletion /app/data-migration/deletion-scripts
```
### Run setup script
- Run `env` in the Portal EC2 instance - you will need `Server`, `Database`, and `Password` from `ConnectionStrings__PostgreSQLConnection` in the next step
- Run the setup script to store DB credentials:
```
data-migration/deletion-scripts/setup-script.sh
```

### Pre-Testing
- Run the test script with `data-migration/deletion-scripts/testing-portal-db-script.sh`.
- Record results in spreadsheet

### Run the deletion
- Run the deletion script with `data-migration/deletion-scripts/data-deletion-portal-db-script.sh`

### Post-testing
- Run the test script again
- Record results in spreadsheet