# EPB EPC API

The service uses the EpbEpc API to check whether a property meets the validity criteria for the grant. If it has a 
non-expired EPC certificate with a rating of A, B or C then it is not valid.

The live version of this API is used for the production environment and the staging version of this API is used for all 
other environments:
* Staging - https://api.epb-staging.digital.communities.gov.uk/api/
* Live - https://api.epb.digital.communities.gov.uk/api/

## Finding properties with a particular EPC certificates

To find properties with particular EPC certificates, so that you can trigger a particular path for development or 
testing (e.g. to find A/B/C certificates to trigger the EPC ineligible path), we can use the Gov UK find energy 
certificate web service.

The staging version of this site aligns with the data returned by the staging EPC API, so we should use this for development and testing. The site can be found at the following links:
* Staging - https://find-energy-certificate-staging.digital.communities.gov.uk/find-a-certificate/search-by-postcode
* Live - https://www.gov.uk/find-energy-certificate