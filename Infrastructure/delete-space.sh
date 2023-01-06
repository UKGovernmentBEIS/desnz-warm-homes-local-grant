cf unbind-service sea-beta-\"load-test\" sea-beta-\"load-test\"-db
cf delete-service-key sea-beta-\"load-test\"-db sea-beta-\"load-test\"-db-developerkey -f
cf delete-service sea-beta-\"load-test\"-db -f

cf delete-space sea-beta-\"load-test\"