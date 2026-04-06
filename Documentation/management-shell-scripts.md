# Management Shell Scripts

There are a number of useful scripts that can be run using `WhlgPublicWebsite.ManagementShell`.

Scripts can be run either via a Rider run configuration or directly in a Docker container.

### Rider

- Select the drop-down icon to the left of the play icon in the top right.
- Select `Edit configurations`
- Select `+` in the top-left -> .Net Project
- Update Project to `WhlgPublicWebsite.ManagementShell`
- Add the name of the script you want (see below) to run followed by any relevant arguments in program arguments.
  - Make sure you've also added the following environment variable: `ConnectionStrings__PostgreSQLConnection: UserId=postgres;Password=postgres;Server=localhost;Port=5432;Database=whlgdev;Include Error Detail=true;Pooling=true`
- Select `OK` in the bottom right.
- You can now select and run this script.

### Docker

- Find the container ID by running `docker ps` or via Docker Desktop.
- Open a shell in the container: `docker exec -it <CONTAINER_ID> /bin/bash`
- Navigate to the CLI directory: `cd cli`
- Run the desired script: `./WhlgPublicWebsite.ManagementShell <COMMAND>`

## List of scripts

- `GenerateReferrals [count]` - Generate fake referral requests and add them to the database. All generated users will have a FullName beginning with "FAKE USER". Cannot be run on Production.
- `GeneratePerMonthStatistics [LocalAuthority/Consortium]` - Output a CSV of referral statistics per Local Authority or Consortium per month to the terminal for you to copy into a CSV file.
- `ExportNewReferralRequestsToPortal` - Export all referral requests received since the last export to the referrals S3 bucket, making them available in the Portal. Can only be run on DEV or Staging deployed environments (not locally). Requires `S3__BucketName` and `S3__Region` environment variables to be set.
- `SetEmergencyMaintenanceState [Enabled/Disabled]` - Enable or disable emergency maintenance mode. When enabled, all requests to the portal are blocked with a 503 response. Only use this as part of the disaster response plan to block all public access to the site.