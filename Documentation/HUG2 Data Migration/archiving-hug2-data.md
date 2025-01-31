# Archiving HUG2 data

## Portal DB 

Connect to the HUG2 portal site container instance by following these instructions [here](https://softwiretech.atlassian.net/wiki/spaces/Support/pages/20606746709/DESNZ+HUG2+Common+Tasks#5.-Accessing-Database) up to step 9.

Create folders:
```
mkdir -p data-archive/{non-personal-data,all-other-personal-data,customer-insight-analysis-data}
```

Update and connect to psql
```
apt-get update && apt-get install -y postgresql-client
env
psql -h [Server name from the connection string] -U mysqladm -d rdsPrs
```

Portal DB Non-personal data:
```
\copy (SELECT * FROM public."__EFMigrationsHistory") TO '/app/data-archive/non-personal-data/__EFMigrationsHistory.csv' CSV HEADER;
\copy (SELECT * FROM public."Consortia") TO '/app/data-archive/non-personal-data/Consortia.csv' CSV HEADER;
\copy (SELECT * FROM public."DataProtectionKeys") TO '/app/data-archive/non-personal-data/DataProtectionKeys.csv' CSV HEADER;
\copy (SELECT * FROM public."LocalAuthorities") TO '/app/data-archive/non-personal-data/LocalAuthorities.csv' CSV HEADER;
```

Portal DB All other personal data:
```
\copy (SELECT * FROM public."ConsortiumUser") TO '/app/data-archive/all-other-personal-data/ConsortiumUser.csv' CSV HEADER;
\copy (SELECT * FROM public."LocalAuthorityUser") TO '/app/data-archive/all-other-personal-data/LocalAuthorityUser.csv' CSV HEADER;

```

Portal DB Customer analysis data:
```
\copy (SELECT * FROM public."AuditDownloads") TO '/app/data-archive/customer-insight-analysis-data/AuditDownloads.csv' CSV HEADER;
\copy (SELECT * FROM public."CsvFileDownloads") TO '/app/data-archive/customer-insight-analysis-data/CsvFileDownloads.csv' CSV HEADER;
\copy (SELECT * FROM public."Users") TO '/app/data-archive/customer-insight-analysis-data/Users.csv' CSV HEADER;
```

Zip:
```
\q
cd data-archive
apt-get update && apt-get install -y zip
for dir in non-personal-data all-other-personal-data customer-insight-analysis-data; do
    zip -r --encrypt "$dir.zip" "$dir/"
done
```

Copy to S3:
```
for file in non-personal-data.zip all-other-personal-data.zip customer-insight-analysis-data.zip; do
    aws s3 cp "$file" s3://<bucket-url>/portal-db/
done
```

Tidy:
```
cd ..
rm -rf data-archive
```

## Public DB

Connect to the HUG2 public site container instance by following these instructions [here](https://softwiretech.atlassian.net/wiki/spaces/Support/pages/20606746709/DESNZ+HUG2+Common+Tasks#5.-Accessing-Database) up to step 9.

Create folders:
```
mkdir -p data-archive/{non-personal-data,all-other-personal-data,customer-insight-analysis-data}
```

Update and connect to psql
```
apt-get update
apt-get install postgresql-client
env
psql -h [Server name from the connection string] -U mysqladm -d rdsPbs
```

Public DB Non-personal data:
```
\copy (SELECT * FROM public."__EFMigrationsHistory") TO '/app/data-archive/non-personal-data/__EFMigrationsHistory.csv' CSV HEADER;
\copy (SELECT * FROM public."sessioncache") TO '/app/data-archive/non-personal-data/sessioncache.csv' CSV HEADER;
\copy (SELECT * FROM public."DataProtectionKeys") TO '/app/data-archive/non-personal-data/DataProtectionKeys.csv' CSV HEADER;
```

Public DB All other personal data:
```
\copy (SELECT * FROM public."NotificationDetails") TO '/app/data-archive/all-other-personal-data/NotificationDetails.csv' CSV HEADER;
```

Public DB Customer analysis data:
```
\copy (SELECT * FROM public."ReferralRequestFollowUps") TO '/app/data-archive/customer-insight-analysis-data/ReferralRequestFollowUps.csv' CSV HEADER;
\copy (SELECT * FROM public."ReferralRequests") TO '/app/data-archive/customer-insight-analysis-data/ReferralRequests.csv' CSV HEADER;
\copy (SELECT * FROM public."Sessions") TO '/app/data-archive/customer-insight-analysis-data/Sessions.csv' CSV HEADER;
```

Zip:
```
\q
cd data-archive
apt-get update && apt-get install -y zip
for dir in non-personal-data all-other-personal-data customer-insight-analysis-data; do
    zip -r --encrypt "$dir.zip" "$dir/"
done
```

Copy to S3:
```
for file in non-personal-data.zip all-other-personal-data.zip customer-insight-analysis-data.zip; do
    aws s3 cp "$file" s3://<bucket-url>/public-db/
done
```

Tidy:
```
cd ..
rm -rf data-archive
```