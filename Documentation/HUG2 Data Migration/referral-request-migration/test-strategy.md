## Testing strategy
### Populate the database (if necessary)
Create a test script and copy in the data from this codebase:
```
vim insert_test_data.sql
```

Run the script:
```
psql -h [Server name from the connection string] -U mysqladm -d rdsPrs -f insert_test_data.sql
```

Run these commands before and after and record the results:

### Test commands
Run these before and after

#### Active LAs

Check for all Active LAs after 1st Dec:
```
SELECT COUNT(*) FROM ReferralRequests
WHERE RequestDate > '2024-12-01'
AND WasSubmittedToPendingLocalAuthority = FALSE
AND WasSubmittedForFutureGrants = FALSE;
```
This should be the same before and after.

Check for exported LAs after 1st Dec:
```
SELECT COUNT(*) FROM ReferralRequests
WHERE ReferralWrittenToCSV = TRUE
AND RequestDate > '2024-12-01'
AND WasSubmittedToPendingLocalAuthority = FALSE
AND WasSubmittedForFutureGrants = FALSE;
```
This should be 0 after.

Check for Active LAs before 1st Dec
```
SELECT COUNT(*) FROM ReferralRequests
WHERE RequestDate < '2024-12-01'
AND WasSubmittedToPendingLocalAuthority = FALSE
AND WasSubmittedForFutureGrants = FALSE;
```
This should be 0 after.

#### Pending LAs
Check all pending LA referrals:
```
SELECT COUNT(*) FROM ReferralRequests
WHERE WasSubmittedToPendingLocalAuthority = TRUE;
```
This should be the same before and after.

Check for exported pending LA referrals:
```
SELECT COUNT(*) FROM ReferralRequests
WHERE WasSubmittedToPendingLocalAuthority = TRUE
AND ReferralWrittenToCSV = TRUE;
```
This should be 0 after.

#### WHLG demand capture
Check all WHLG demand capture referrals:
```
SELECT COUNT(*) FROM ReferralRequests
WHERE WasSubmittedForFutureGrants = TRUE;
```
This should be the same before and after.

Check for exported WHLG demand capture referrals:
```
SELECT COUNT(*) FROM ReferralRequests
WHERE ReferralWrittenToCSV = TRUE
AND WasSubmittedForFutureGrants = TRUE;
```
This should be 0 after.

#### IMD3 postcodes
All referrals that previously qualified only because they had an IMD3 postcode:
```
SELECT COUNT(*) FROM ReferralRequests
WHERE AddressPostcode IN ('IM3 1AA', 'IM3 2BB', 'IM3 3CC')
AND IncomeBand IN (1,3,5);;
```
This should be 0 after