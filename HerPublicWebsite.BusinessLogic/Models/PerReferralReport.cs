namespace HerPublicWebsite.BusinessLogic.Models;

public class PerReferralReport
{
    public DateTime? ApplicationDate { get; set; }
    public string ReferralCode { get; set; }
    public string Uprn { get; set; }
    public string Scheme { get; set; }

    public PerReferralReport()
    {
    }

    public PerReferralReport(ReferralRequest referralRequest)
    {
        ApplicationDate = DateTime.Now;
        ReferralCode = referralRequest.ReferralCode;
        Uprn = referralRequest.Uprn;
        Scheme = "HUG2";
    }
}
