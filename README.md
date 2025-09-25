# Warm Homes: Local Grant BETA

This repository was cloned from [HUG2](https://github.com/UKGovernmentBEIS/desnz-home-energy-retrofit-beta) in December 2024, keeping all previous commits.

The code was then adapted into the WH:LG service.

Note, the WH:LG project is split across 2 repositories:
- This one, which runs the user site responsible for generating WH:LG referrals.
- The [desnz-warm-homes-local-grant-portal repository](https://github.com/UKGovernmentBEIS/desnz-warm-homes-local-grant-portal), which runs the admin site responsible for viewing these referrals.

## Local setup

### Pre-requisites

For quick setup (running with docker compose):
- Docker Desktop (https://www.docker.com/products/docker-desktop/)
- .Net 8 (https://dotnet.microsoft.com/en-us/download/dotnet/8.0)

### Optional requirements
Recommended if you want to run the project in Rider with access to local tools, such as the debugger and EF migrations:
- Install EF Core CLI tools (https://docs.microsoft.com/en-us/ef/core/cli/dotnet)
- Node v14+ (https://nodejs.org/en/)
- If you're using Rider then you will need to install the ".net core user secrets" plugin
- If you need to work on the S3 file writing code, download and configure Minio (see below)
    - [Windows](https://min.io/download#/windows)
    - [Mac](https://min.io/docs/minio/macos/index.html#procedure)

In WhlgPublicWebsite run `npm install`

### Minio

If using docker minio will be configured automatically.

The database of referrals is copied nightly from postgres to an S3 bucket. For local development we need a fake S3 bucket to connect to. To use [Minio](https://min.io/) to provide a local S3 bucket follow these steps:
1. Download minio
    * [Windows](https://min.io/download#/windows)
    * [Mac](https://min.io/docs/minio/macos/index.html#procedure)
2. Put the executable somewhere on your machine (e.g. c:\Program Files\Minio)
3. Decide on a folder for Minio to store its data in (e.g. c:\data\minio)
4. To run the server:
    * Windows
        * Open a PowerShell window and go to the folder that you put Minio in
        * Run `.\minio.exe server <path to data folder> --console-address :9090`
    * Mac:
        * Open any terminal
        * Run `export MINIO_CONFIG_ENV_FILE=/etc/default/minio`
        * Run `minio server <path to data folder> --console-address :9090`
5. The first time that you do this:
    1. Visit http://localhost:9090
    2. Login (default is minioadmin/minioadmin)
    3. Create a new bucket called `desnz-whlg-portal-referrals`

### APIs

The app communicates with a number of APIs. You will need to obtain and configure credentials for these APIs in your user secrets file.

You can find the values for these secrets in the DESNZ Support folder in [Keeper](https://keepersecurity.eu/vault/).

In Rider:
- Right-click on the `WhlgPublicWebsite` project
- Select `Tools`
- Select `Open Project User Secrets`

Fill in the opened `secrets.json` file with:

```json
{
    "EpbEpc": {
        "Username": "<REAL_VALUE_HERE>",
        "Password": "<REAL_VALUE_HERE>"
    },

    "GovUkNotify": {
        "ApiKey": "<REAL_VALUE_HERE>"
    },

    "GoogleAnalytics": {
        "ApiSecret": "REAL_VALUE_HERE"
    },

   "OsPlaces": {
      "Key": "REAL_VALUE_HERE"
   }
}
```

You can also add secrets with `dotnet user-secrets`, just pipe the JSON you want to be added to it e.g.
```
cat secrets.json | dotnet user-secrets set
```

### Local database setup

If using docker the database will be configured automatically.

#### Windows
- Download the installer and PostgreSQL 15 [here](https://www.postgresql.org/download/windows/)
- Follow default installation steps (no additional software is required from Stack Builder upon completion)
    - You may be prompted for a password for the postgres user and a port (good defaults are "postgres" and "5432", respectively). If you choose your own, you will have to update the connection string in appsettings.json

#### Mac
- Select a download option from [here](https://www.postgresql.org/download/macosx/) and download PostgreSQL 15
    - The [Postgres.app](https://postgresapp.com/) option works well
- Initialise a server with the following configuration (or update `PostgreSQLConnection` in `appsettings.json` to match your own):
    - Port: `5432`
    - User: `postgres`
    - Password: `postgres`

Once the server is running, you should be able to run the project locally.

If you are having issues the first time you run the project after initialising the server, you may need to manually create the `whlgdev` database first.

For further information on interacting with the database and using migrations, please see [here](Documentation/database.md).

### Running the service locally

#### Docker

If using docker, run `docker compose up --build`, then visit http://localhost:5000/questionnaire/

#### Without Docker

- Ensure Minio is running:
  * Windows
      * In your Minio folder un `.\minio.exe server <path to data folder> --console-address :9090`
  * Mac:
      * Run `export MINIO_CONFIG_ENV_FILE=/etc/default/minio`
      * Run `minio server <path to data folder> --console-address :9090`
- In Rider build the solution
- In `WhlgPublicWebsite` run `npm run watch`
- In Rider run the `WhlgPublicWebsite` project
- In a browser, visit http://localhost:5000/questionnaire/

### Running tests

#### Docker

If using docker, run `docker compose -f docker-compose.test.yml run --rm --build tests`.

#### Without Docker

- In the project explorer on the left, right click on `WhlgPublicWebsite.UnitTests` and select `Run Unit Tests`
- Note, if you've previously run `npm run watch` you'll have to terminate this (`ctrl`/`cmd` + `c`) before running the tests.

### Making frontend changes

For instructions on making changes to the frontend, see [here](Documentation/making-frontend-changes.md).

### Auto-Formatter

We use the standard Rider code cleanup tool for this project. Before committing, make sure to run the code cleanup on edited files.
See [Rider docs](https://www.jetbrains.com/help/rider/2024.3/Code_Cleanup__Index.html#running) for information on running the formatter. Use the 'DESNZ' profile when running code format.

Historically, we did not always use this formatter, so some files will be non-compliant.
Run the formatter on all files edited in a PR. There may be additional formatting changes made.
Commit these in a separate commit to your other changes.

## Deployment

The site is deployed using AWS CodePipeline.

### Deployed environments

We follow a process similar to git-flow, with 3 branches corresponding to each of the environments:
- `develop` - [Dev](https://dev.apply-warm-homes-local-grant.service.gov.uk)
- `staging` - [UAT](https://uat.apply-warm-homes-local-grant.service.gov.uk)
- `main` - [Production](https://www.apply-warm-homes-local-grant.service.gov.uk)

### Trivy

On each push to `develop`, we run a Trivy scan on the Docker image to check for vulnerabilities.
If the scan fails, we should look into the new vulnerability and either:
- Fix it
- Add to .trivyignore if the issue is a false positive.

### Database migrations

Migrations will be run automatically on deployment. If a migration needs to be rolled back for any reason there are two options:
1. Create a new inverse migration and deploy that
2. Generate and run a rollback script
    1. Check out the same commit locally
    2. [Install EF Core CLI tools](https://docs.microsoft.com/en-us/ef/core/cli/dotnet) if you haven't already
    3. Generate a rollback script using `dotnet ef migrations script 2022010112345678_BadMigration 2022010112345678_LastGoodMigration -o revert.sql` from the `WhlgPublicWebsite` directory
    4. Review the script
    5. Connect to the database and run the script

### Deployments

When code is merged to the dev, staging or main branch, the corresponding CodeDeploy pipeline is started.

You can look at the Deployment history [here](https://eu-west-2.console.aws.amazon.com/codesuite/codedeploy/deployments?region=eu-west-2). It should show you all the successful, failed and in-progress deployments for your current role.

## Environments

This app is deployed to ICS AWS platform

### Configuration

Non-secret configuration is stored in the corresponding `appsettings.<environment>.json` file:
- appsettings.DEV.json
- appsettings.UAT.json
- appsettings.Production.json

Secrets must be configured in the ECS tasks, corresponding to the variables in `secrets.json` above:
- `ConnectionStrings__PostgreSQLConnection`
- `EpbEpc__Username`
- `EpbEpc__Password`
- `GoogleAnalytics__ApiKey`
- `GovUkNotify__ApiKey`
- `GovUkNotify__ComplianceEmailRecipients`
- `GovUkNotify__PendingReferralEmailRecipients`
- `OsPlaces__Key`

To prevent public access to DEV and UAT environments, we should also override the auth credentials:
- `Auth__Password`

(These are not required for production)

The S3 configuration is also configured in ECS, as it's linked to AWS resources
- `S3__BucketName`
- `S3__Region`

## Common issues

### The styling doesn't look correct locally

This is likely because the CSS hasn't been built. To fix this:

- Open the terminal in the root folder of the project
- Run `git pull` to ensure you have the latest code
- Navigate to the `WhlgPublicWebsite` directory
- Run `npm install` to install any new dependencies
- Run `npm run build` to build the CSS

Note that running with docker compose will do this automatically.