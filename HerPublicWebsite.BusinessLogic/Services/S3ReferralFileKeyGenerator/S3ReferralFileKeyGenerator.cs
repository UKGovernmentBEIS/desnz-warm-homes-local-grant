using HerPublicWebsite.BusinessLogic.Models;

namespace HerPublicWebsite.BusinessLogic.Services.S3ReferralFileKeyGenerator;

public class S3ReferralFileKeyGenerator
{
    public string GetS3Key(string custodianCode, int month, int year)
    {
        if (!LocalAuthorityData.LocalAuthorityDetailsByCustodianCode.ContainsKey(custodianCode))
        {
            throw new ArgumentException("Invalid custodian code: " + custodianCode);
        }

        if (year < 1000 || year > 9999)
        {
            throw new ArgumentException("Invalid year: " + year);
        }

        if (month < 1 || month > 12)
        {
            throw new ArgumentException("Invalid month: " + month);
        }

        return $"{custodianCode}/{year}_{month:D2}.csv";
    }
}
