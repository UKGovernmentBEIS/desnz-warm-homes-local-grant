# Data Deletion Process

This document outlines the process for deleting the contents of several tables, which are not being carried over to the new system.

## Upload the scripts
- To the `migration-scripts/data-deletion` folder of the S3 bucket (if not already there), manually upload the following files:
  - `setup-script.sh`
  - `testing-portal-db-script.sh`
  - `testing-public-db-script.sh`
  - `data-deletion-portal-db-script.sh`
  - `data-deletion-public-db-script.sh`
  - `insert-public-test-data.sh` (Not used on Production)
  - `insert-portal-test-data.sh` (Not used on Production)

## Public DB deletion
### Connect to the EC2 instance & copy scripts
Connect to the Public HUG2 external site container instance by following the instructions [here](https://softwiretech.atlassian.net/wiki/spaces/Support/pages/20606746709/DESNZ+HUG2+Common+Tasks#5.-Accessing-Database) up to step 9.
- In the EC2 instance, run the following to copy the scripts over:
```shell
cd /root
mkdir -p data-migration/deletion-scripts
apt-get update && apt-get install -y awscli
aws s3 cp s3://hug2-<ENV>-data-migration/migration-scripts/data-deletion/setup-script.sh /root/data-migration/deletion-scripts
aws s3 cp s3://hug2-<ENV>-data-migration/migration-scripts/data-deletion/data-deletion-public-db-script.sh /root/data-migration/deletion-scripts
aws s3 cp s3://hug2-<ENV>-data-migration/migration-scripts/data-deletion/testing-public-db-script.sh /root/data-migration/deletion-scripts
```
Replacing `ENV` with the actual environment.

On non-production environments, also run this additional command:
```shell
aws s3 cp s3://hug2-<ENV>-data-migration/migration-scripts/data-deletion/insert-public-test-data.sh /root/data-migration/deletion-scripts
```

### Run setup script
- Run `env` in the Public EC2 instance - you will need `Server`, `Database`, and `Password` from `ConnectionStrings__PostgreSQLConnection` in the next step
- Run the setup script to store DB credentials:
```shell
chmod +x data-migration/deletion-scripts/setup-script.sh
source data-migration/deletion-scripts/setup-script.sh
```

### Pre-Testing
#### On non-production environments:
- Run the script to insert test data. 
- <strong>CAUTION: This will also wipe the `ReferralRequest` table. Do NOT use this on Production:</strong>
```shell
chmod +x data-migration/deletion-scripts/insert-public-test-data.sh
data-migration/deletion-scripts/insert-public-test-data.sh
```
#### On all environments:
- Run the test script:
```shell
chmod +x data-migration/deletion-scripts/testing-public-db-script.sh
data-migration/deletion-scripts/testing-public-db-script.sh
```
- Record results in the `data-deletion-test-template.xlsx` spreadsheet found on the [data migration Swiki page](https://softwiretech.atlassian.net/wiki/spaces/Support/pages/21481160877/DESNZ+HUG2+Data+Migration).

### Run the deletion
- Run the deletion script with
```shell 
chmod +x data-migration/deletion-scripts/data-deletion-public-db-script.sh
data-migration/deletion-scripts/data-deletion-public-db-script.sh
```

## Post-testing
- Run the test script again
```shell
chmod +x data-migration/deletion-scripts/testing-public-db-script.sh
data-migration/deletion-scripts/testing-public-db-script.sh
```
- Record results in spreadsheet and compare to expected

## Clean up
Run:
```shell
rm -r /root/data-migration
```

## Portal DB deletion
### Connect to the EC2 instance & copy scripts
Connect to the Portal HUG2 external site container instance by following the instructions [here](https://softwiretech.atlassian.net/wiki/spaces/Support/pages/20606746709/DESNZ+HUG2+Common+Tasks#5.-Accessing-Database) up to step 9.
- In the EC2 instance, run the following to copy the scripts over:
```shell
cd /root
mkdir -p data-migration/deletion-scripts
apt-get update && apt-get install -y awscli
aws s3 cp s3://hug2-<ENV>-data-migration/migration-scripts/data-deletion/setup-script.sh /root/data-migration/deletion-scripts
aws s3 cp s3://hug2-<ENV>-data-migration/migration-scripts/data-deletion/data-deletion-portal-db-script.sh /root/data-migration/deletion-scripts
aws s3 cp s3://hug2-<ENV>-data-migration/migration-scripts/data-deletion/testing-portal-db-script.sh /root/data-migration/deletion-scripts
```
Replacing `ENV` with the actual environment.

On non-production environments, also run this additional command:
```shell
aws s3 cp s3://hug2-<ENV>-data-migration/migration-scripts/data-deletion/insert-portal-test-data.sh /root/data-migration/deletion-scripts
```
### Run setup script
- Run `env` in the Portal EC2 instance - you will need `Server`, `Database`, and `Password` from `ConnectionStrings__PostgreSQLConnection` in the next step
- Run the setup script to store DB credentials:
```shell
chmod +x data-migration/deletion-scripts/setup-script.sh
source data-migration/deletion-scripts/setup-script.sh
```

### Pre-Testing
#### On non-production environments:
- Run the script to insert test data:
```shell
chmod +x data-migration/deletion-scripts/insert-portal-test-data.sh
data-migration/deletion-scripts/insert-portal-test-data.sh
```
#### On all environments:
- Run the test script:
```shell
chmod +x data-migration/deletion-scripts/testing-portal-db-script.sh
data-migration/deletion-scripts/testing-portal-db-script.sh
```
- Record results in the `data-deletion-test-template.xlsx` spreadsheet found on the [data migration Swiki page](https://softwiretech.atlassian.net/wiki/spaces/Support/pages/21481160877/DESNZ+HUG2+Data+Migration).

### Run the deletion
- Run the deletion script with
```shell 
chmod +x data-migration/deletion-scripts/data-deletion-portal-db-script.sh
data-migration/deletion-scripts/data-deletion-portal-db-script.sh
```

## Post-testing
- Run the test script again
```shell
chmod +x data-migration/deletion-scripts/testing-portal-db-script.sh
data-migration/deletion-scripts/testing-portal-db-script.sh
```
- Record results in spreadsheet and compare to expected

## Clean up
Run:
```shell
rm -r /root/data-migration
```