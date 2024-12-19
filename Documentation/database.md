# Database

## Setup

To set up your local database please see Local Database Setup in the [README](../README.md).

## Creating/updating the local database

- You can just run the website project and it will create and update the database on startup
- If you want to manually update the database (e.g. to test a new migration) in the terminal (from the solution directory) run `dotnet ef database update --project .\WhlgPublicWebsite`

## Connecting to the database with Rider

This is a convenient way of viewing/editing the database and its data. To do this:
- Open the database tab (usually on the right)
- Select the `+` icon then select 'Connect to database'
- Select 'Use connection string' and change 'Database type' to 'PostgreSQL'.
- You shouldn't need to change the connection string.
- Select 'Connect to Database'.
- You should now be able to see `whlgdev@localhost`
- Select the `x of 6` button just next to this and ensure `whlgdev` is selected.
- Within this, ensure `public` is selected, then within this you should be able to see all of the tables.

## Adding Migrations

- In the terminal (from the solution directory) run `dotnet ef migrations add <YOUR_MIGRATION_NAME> --project WhlgPublicWebsite.Data --startup-project WhlgPublicWebsite`
- Then update the local database

## Reverting Migrations

You may want to revert a migration on your local database as part of a merge, or just because it's wrong and you need to fix it (only do this for migrations that haven't been merged to main yet)
- Run `dotnet ef database update <MIGRATION_BEFORE_YOURS> --project .\WhlgPublicWebsite` to rollback your local database
- Run `dotnet ef migrations remove --project .\WhlgPublicWebsite.Data --startup-project .\WhlgPublicWebsite` to delete the migration and undo the snapshot changes

### Merging Migrations

We cannot merge branches both containing different migrations. We have marked the EF Core snapshot file as binary in git. This should mean that git throws up an error if we try to merge branches with different migrations
(because git will try to merge two sets of changes into the snapshot file and it can't merge changes in binary files).
The solution is unfortunately tedious. Given branch 1 with migration A and branch 2 with migration B:
- One branch is merged to main as normal (let's say branch 1)
- On branch 2
- Revert and remove migration B
- Merge main into branch 2
- Recreate migration B (which will now be on top of migation A)
- Merge branch 2 into main