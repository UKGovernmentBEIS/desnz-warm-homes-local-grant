echo "Usage: RemoveUserSpacePermissions <user> <space>"

cf unset-space-role $1 beis-domestic-energy-advice-service $2 SpaceManager
cf unset-space-role $1 beis-domestic-energy-advice-service $2 SpaceDeveloper
cf unset-space-role $1 beis-domestic-energy-advice-service $2 SpaceAuditor
