# Find Energy Saving Grants You May Be Eligible For BETA

## Deployment

The site is deployed using github actions.

### Database Migrations

Migrations will be run automatically on deployment. If a migration needs to be rolled back for any reason there are two options:
1. Create a new inverse migration and deploy that
2. Generate and run a rollback script
   1. Check out the same commit locally
   2. [Install EF Core CLI tools](https://docs.microsoft.com/en-us/ef/core/cli/dotnet) if you haven't already
   3. Generate a rollback script using `dotnet ef migrations script 2022010112345678_BadMigration 2022010112345678_LastGoodMigration -o revert.sql` from the `HerPublicWebsite` directory
   4. Review the script 
   5. Connect to the database and run the script

## Development

### Process

We follow a process similar to git-flow, with 3 branches corresponding to each of the environments:
- `develop` - [Dev](https://dev.check-eligibility-for-home-upgrade-grant.service.gov.uk)
- `staging` - [UAT](https://uat.check-eligibility-for-home-upgrade-grant.service.gov.uk)
- `main` - [Production](https://www.check-eligibility-for-home-upgrade-grant.service.gov.uk)

For normal development:
- Create a branch from `develop`
- Make changes on the branch, e.g. `feat/add-new-widget`
- Raise a PR back to `develop` once the feature is complete
- If the PR is accepted merge the branch into `develop`

Doing a release to staging:
- Merge `develop` into `staging`
- Deploy this branch into the UAT environment
- Run manual tests against this environment and gain sign-off to deploy

Doing a release to production:
- Ensure all sign-offs are in place
- Merge `staging` into `main`
- Deploy this branch into the production environment
- Perform any post go-live checks

For critical bug fixes on production
- Create a hotfix branch from `main`, e.g. `hotfix/update-service-name`
- Make changes on the branch
- Raise a PR back to `main` once the bug is fixed
- If the PR is accepted, merge the branch into `main`
- Then also merge the branch into `develop`

### Pre-requisites

- .Net 6 (https://dotnet.microsoft.com/en-us/download/dotnet/6.0)
- Install EF Core CLI tools (https://docs.microsoft.com/en-us/ef/core/cli/dotnet)
- Node v14+ (https://nodejs.org/en/)
- If you're using Rider then you will need to install the ".net core user secrets" plugin
- If you need to work on the S3 file writing code, download and configure Minio for Windows (https://min.io/download#/windows) (see below)

In HerPublicWebsite run `npm install`

#### Minio

The portal site lists files hosted in an S3 bucket. For local development we need a fake S3 bucket to connect to.
To use [Minio](https://min.io/) to provide a local S3 bucket follow these steps:
1. [Download minio](https://min.io/download#/windows)
2. Put the executable somewhere on your machine (e.g. c:\Program Files\Minio)
3. Decide on a folder for Minio to store its data in (e.g. c:\data\minio)
4. In a PowerShell window go to the folder that you put Minio in
5. Run `.\minio.exe server <path to data folder> --console-address :9090`
6. The first time that you do this:
   1. Visit http://localhost:9090
   2. Login (default is minioadmin/minioadmin)
   3. Create a new bucket called `desnz-her-portal-referrals`

### GovUkDesignSystem

We are using the GovUkDesignSystem library from the Cabinet Office: https://github.com/cabinetoffice/govuk-design-system-dotnet

As this library is not currently published to Nuget we have a copy of the library in a nuget package in the /Lib folder of this solution.

If you need to make changes to the GovUkDesignSystem (e.g. to add a new component) then you should:
- Clone the BEIS fork of the repository (currently https://github.com/UKGovernmentBEIS/govuk-design-system-dotnet) and check out the `master` branch.
- Create a branch for you feature
- Develop and commit your changes (don't forget automated tests as applicable)
- Build and package your branch with `dotnet pack -p:PackageVersion=1.0.0-$(git rev-parse --short HEAD) -c Release -o .` in the `GovUkDesignSystem` folder
- Copy the built package to /Lib and delete the old package
- Update the package version in the HER project
- Test that your changes work on the HER site
- Create a PR from your branch back to `master`
- Get the PR reviewed and merged
- From time to time create a PR to merge the `master` branch back to the Cabinet Office repository (https://github.com/cabinetoffice/govuk-design-system-dotnet)

#### GOV.UK Frontend

The GovUkDesignSystem project relies on the GOV.UK Frontend NPM package which contains images, fonts, styling, and JavaScript. When updating
the GovUkDesignSystem you may also need to update the GOV.UK Frontend NPM package. To do this:

- Update the version number of the GOV.UK Frontend package in package.json
- Run `npm install`
- Run `npm run update-govuk-assets`
- Run `npm run build`

### APIs

The app communicates with a number of APIs. You will need to obtain and configure credentials for these APIs in your user secrets file.

In Rider:
- Right-click on the `HerPublicWebsite` project
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

### Running Locally

- If you're working on the S3 writing code, in your Minio folder run Minio `.\minio.exe server <path to data folder> --console-address :9090`
- In Visual Studio / Rider build the solution
- In `HerPublicWebsite` run `npm run watch`
- In Visual Studio / Rider run the `HerPublicWebsite` project

## Database

### Local Database Setup

- For Windows: Download the installer and PostgreSQL 15 here: https://www.postgresql.org/download/windows/
- Follow default installation steps (no additional software is required from Stack Builder upon completion)
  - You may be prompted for a password for the postgres user and a port (good defaults are "postgres" and "5432", respectively). If you choose your own, you will have to update the connection string in appsettings.json

### Creating/updating the local database

- You can just run the website project and it will create and update the database on startup
- If you want to manually update the database (e.g. to test a new migration) in the terminal (from the solution directory) run `dotnet ef database update --project .\HerPublicWebsite`

### Adding Migrations

- In the terminal (from the solution directory) run `dotnet ef migrations add <YOUR_MIGRATION_NAME> --project .\HerPublicWebsite.Data --startup-project .\HerPublicWebsite`
- Then update the local database

### Reverting Migrations

You may want to revert a migration on your local database as part of a merge, or just because it's wrong and you need to fix it (only do this for migrations that haven't been merged to main yet)
- Run `dotnet ef database update <MIGRATION_BEFORE_YOURS> --project .\HerPublicWebsite` to rollback your local database
- Run `dotnet ef migrations remove --project .\HerPublicWebsite.Data --startup-project .\HerPublicWebsite` to delete the migration and undo the snapshot changes

#### Merging Migrations

We cannot merge branches both containing different migrations. We have marked the EF Core snapshot file as binary in git. This should mean that git throws up an error if we try to merge branches with different migrations
(because git will try to merge two sets of changes into the snapshot file and it can't merge changes in binary files).
The solution is unfortunately tedious. Given branch 1 with migration A and branch 2 with migration B:
- One branch is merged to main as normal (let's say branch 1)
- On branch 2
- Revert and remove migration B
- Merge main into branch 2
- Recreate migration B (which will now be on top of migation A)
- Merge branch 2 into main

## Environments

This app is deployed to BEIS AWS platform

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
- `OsPlaces__Key`

To prevent public access to DEV and UAT environments, we should also override the basic auth credentials:
- `BasicAuth__Username`
- `BasicAuth__Password`

(These are not required for production)

The S3 configuration is also configured in ECS, as it's linked to AWS resources
- `S3__BucketName`
- `S3__Region`
