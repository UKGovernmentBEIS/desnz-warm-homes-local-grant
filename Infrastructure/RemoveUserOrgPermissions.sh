echo "Usage: RemoveUserOrgPermissions <user>"

cf unset-org-role $1 beis-domestic-energy-advice-service OrgManager
cf unset-org-role $1 beis-domestic-energy-advice-service BillingManager
cf unset-org-role $1 beis-domestic-energy-advice-service OrgAuditor
