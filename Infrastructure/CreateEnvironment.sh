
########################
# Start of configuration

# Name - This is the bit after 'sea-beta-' - e.g. for sea-beta-DEV, PAAS_ENV_SHORTNAME would just be 'DEV'
#
#        **** NOTE: For the production environment you must use "Production" ****
#
echo "What name do you want to give to the new environment? (just the part after the 'sea-beta-' prefix please)"
read PAAS_ENV_SHORTNAME

# Database size - we normally use 'small-13' for testing and 'small-ha-13' for production
if [ "$PAAS_ENV_SHORTNAME" == "Production" ]
then
  DATABASE_SIZE="small-ha-13"
else
  DATABASE_SIZE="small-13"
fi

# App scale settings
APP_INSTANCES=2     # Use at least 2 so that we can do rolling deployments
APP_DISK="1G"       # Not sure what this default should be - we'll have to test this and set a default later
APP_MEMORY="4G"     # Not sure what this needs to be for live load. The alpha worked fine on 1GB

#################
# Start of script

#------
# Login
./LoginToGovPaas.sh


#-----------------
# Create the space
cf create-space "sea-beta-${PAAS_ENV_SHORTNAME}" -o "beis-domestic-energy-advice-service"

# - Target future commands at this space
cf target -s "sea-beta-${PAAS_ENV_SHORTNAME}"


#-----------------------------------------
# Create Postgres database backing service
# - Create the database itself
cf create-service postgres "${DATABASE_SIZE}" "sea-beta-${PAAS_ENV_SHORTNAME}-db"

echo "Waiting for the Postgres database to be created (normally takes 5-10 mins)"

RESULT=$(cf service "sea-beta-${PAAS_ENV_SHORTNAME}-db")
while [ -z "${RESULT##*create in progress*}" ]
do
	echo -n "."
	sleep 5s
	RESULT=$(cf service "sea-beta-${PAAS_ENV_SHORTNAME}-db")
done
echo "" # To create a new-line

if [ -z "${RESULT##*create succeeded*}" ]
then
	echo "Create successful"
	cf service "sea-beta-${PAAS_ENV_SHORTNAME}-db"
else
	echo "Create failed"
	cf service "sea-beta-${PAAS_ENV_SHORTNAME}-db"
	exit 1
fi

# - Create a key to access the service
#   Note: this is only needed to access the database from outside of Gov.UK PaaS (e.g. from a developer machine)
cf create-service-key "sea-beta-${PAAS_ENV_SHORTNAME}-db" "sea-beta-${PAAS_ENV_SHORTNAME}-db-developerkey"

# - Get the key (and print to the console)
cf service-key "sea-beta-${PAAS_ENV_SHORTNAME}-db" "sea-beta-${PAAS_ENV_SHORTNAME}-db-developerkey"


#-----------
# Create App
# - Create the App
cf create-app "sea-beta-${PAAS_ENV_SHORTNAME}"

# - Scale to the right size
cf scale "sea-beta-${PAAS_ENV_SHORTNAME}" -i ${APP_INSTANCES} -k ${APP_DISK} -m ${APP_MEMORY} -f
echo "This will say FAILED, but it has probably worked (it failed to START the app because there isn't currently an app deployed)"


# - Set the ASP.Net Core environment
cf set-env "sea-beta-${PAAS_ENV_SHORTNAME}" ASPNETCORE_ENVIRONMENT ${PAAS_ENV_SHORTNAME}

# - Bind app to database
cf bind-service "sea-beta-${PAAS_ENV_SHORTNAME}" "sea-beta-${PAAS_ENV_SHORTNAME}-db"

# - Bind an autoscaling policy for production
if [ "$PAAS_ENV_SHORTNAME" == "Production" ]
then
  # - Install autoscaling plugin
  cf install-plugin -r CF-Community app-autoscaler-plugin
  
  # - Create autoscaling service
  cf create-service autoscaler autoscaler-free-plan scale-sea-beta-Production
  
  # - Bind autoscaling service
  cf bind-service sea-beta-Production scale-sea-beta-Production
  
  # - Attach autoscaling policy 
  cf attach-autoscaling-policy sea-beta-Production autoscaling-policy.Production.json
fi

# - Add health check
cf set-health-check "sea-beta-${PAAS_ENV_SHORTNAME}" http --endpoint "/health-check"


# Wait for user input - just to make sure the window doesn't close without them noticing
echo ""
echo "NOTE: Now give the Github build action user the SpaceDeveloper permission on this space"
echo "NOTE: Now give the monitoring user the SpaceAuditor permission on this space"
# e.g. cf set-space-role test@example.com beis-domestic-energy-advice-service sea-beta-DEV SpaceDeveloper
echo ""
read  -n 1 -p "Press Enter to finish:" unused
