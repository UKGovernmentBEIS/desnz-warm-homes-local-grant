# Archiving HUG2 Data Process

We'll need to archive the data from both the Portal DB and the Public DB. As these exist in separate containers - we'll have to follow a similar process for both.

## Upload scripts to S3 bucket
- Manually upload the following scripts to the `migration-scripts/data-archive` folder of the `hug2-<ENV>-data-migration` S3 bucket (if they aren't already there):
  - `archive-portal-db-script.sh`
  - `archive-public-db-script.sh`
  - `count-portal-db-script.sh`
  - `count-public-db-script.sh`
  - `setup-script.sh`

## Portal DB

### 1. Connecting to the EC2 instance
To connect to the portal container instance, follow the instructions for '5. Accessing Database' [here](https://softwiretech.atlassian.net/wiki/spaces/Support/pages/20606746709/DESNZ+HUG2+Common+Tasks#5.-Accessing-Database) up to step 9.

### 2. Copy the scripts
- In the EC2 instance, run the following to copy the scripts over from the S3 bucket:
```
cd /root
mkdir -p data-archive/migration-scripts
apt-get update && apt-get install -y awscli
aws s3 cp s3://hug2-<ENV>-data-migration/migration-scripts/data-archive/setup-script.sh /root/data-archive/migration-scripts/
aws s3 cp s3://hug2-<ENV>-data-migration/migration-scripts/data-archive/count-portal-db-script.sh /root/data-archive/migration-scripts/
aws s3 cp s3://hug2-<ENV>-data-migration/migration-scripts/data-archive/archive-portal-db-script.sh /root/data-archive/migration-scripts/
```
Replacing `ENV` with the actual environment.

### 3. Run setup script
- Run `env` in the EC2 instance - you will need `Server`, `Database`, and `Password` from `ConnectionStrings__PostgreSQLConnection` in the next step
- Run the setup script to enter credentials and config:
```shell
chmod +x data-archive/migration-scripts/setup-script.sh
source data-archive/migration-scripts/setup-script.sh
```

### 4. Count expected exports
- Run the following script to count the number of records in each table and therefore the number of expected exported records for each table:
```shell
chmod +x data-archive/migration-scripts/count-portal-db-script.sh
data-archive/migration-scripts/count-portal-db-script.sh
```
- Record results in the `data-archive-test-template.xlsx` spreadsheet found on the [data migration Swiki page](https://softwiretech.atlassian.net/wiki/spaces/Support/pages/21481160877/DESNZ+HUG2+Data+Migration).

### 5. Run archive script
- Run the script to export the portal DB data:
```shell
chmod +x data-archive/migration-scripts/archive-portal-db-script.sh
data-archive/migration-scripts/archive-portal-db-script.sh
```

If there are any issues during the copying from the DB, all the copy operations will be aborted and no files will be copied to S3.

### 6. Count actual exports
- Go to the `data-archive/portal-db/` of the relevant S3 bucket, and count the number of rows for each exported table.
- Record results in the `data-archive-test-template.xlsx` spreadsheet found on the [data migration Swiki page](https://softwiretech.atlassian.net/wiki/spaces/Support/pages/21481160877/DESNZ+HUG2+Data+Migration) and check it equals those recorded in step 4.

## Public DB

### 1. Connecting to the EC2 instance
To connect to the public container instance, follow the instructions for '5. Accessing Database' [here](https://softwiretech.atlassian.net/wiki/spaces/Support/pages/20606746709/DESNZ+HUG2+Common+Tasks#5.-Accessing-Database) up to step 9.

### 2. Copy the scripts
- In the EC2 instance, run the following to copy the scripts over from the S3 bucket:
```
cd /root
mkdir -p data-archive/migration-scripts
apt-get update && apt-get install -y awscli
aws s3 cp s3://hug2-<ENV>-data-migration/migration-scripts/data-archive/setup-script.sh /root/data-archive/migration-scripts/
aws s3 cp s3://hug2-<ENV>-data-migration/migration-scripts/data-archive/count-public-db-script.sh /root/data-archive/migration-scripts/
aws s3 cp s3://hug2-<ENV>-data-migration/migration-scripts/data-archive/archive-public-db-script.sh /root/data-archive/migration-scripts/
```
Replacing `ENV` with the actual environment.

### 3. Run setup script
- Run `env` in the EC2 instance - you will need `Server`, `Database`, and `Password` from `ConnectionStrings__PostgreSQLConnection` in the next step
- Run the setup script to enter credentials and config:
```shell
chmod +x data-archive/migration-scripts/setup-script.sh
source data-archive/migration-scripts/setup-script.sh
```

### 4. Count expected exports
- Run the following script to count the number of records in each table and therefore the number of expected exported records for each table:
```shell
chmod +x data-archive/migration-scripts/count-public-db-script.sh
data-archive/migration-scripts/count-public-db-script.sh
```
- Record the results.

### 5. Run archive script
- Run the script to export the public DB data:
```shell
chmod +x data-archive/migration-scripts/archive-public-db-script.sh
data-archive/migration-scripts/archive-public-db-script.sh
```

If there are any issues during the copying from the DB, all the copy operations will be aborted and no files will be copied to S3.

### 6. Count actual exports
- Go to the `data-archive/public-db/` of the relevant S3 bucket, and count the number of rows for each exported table and compare it the expected result.