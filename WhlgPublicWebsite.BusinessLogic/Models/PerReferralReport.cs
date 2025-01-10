namespace WhlgPublicWebsite.BusinessLogic.Models;

public class PerReferralReport
{
    public DateTime? ApplicationDate { get; set; }
    public string ReferralCode { get; set; }
    public string Uprn { get; set; }

    public string AddressLine1 { get; set; }
    public string AddressLine2 { get; set; }
    public string AddressTown { get; set; }
    public string AddressCounty { get; set; }
    public string AddressPostcode { get; set; }

    public PerReferralReport()
    {
    }

    public PerReferralReport(ReferralRequest referralRequest)
    {
        ApplicationDate = referralRequest.RequestDate;
        ReferralCode = referralRequest.ReferralCode;
        Uprn = referralRequest.Uprn;
        AddressLine1 = referralRequest.AddressLine1;
        AddressLine2 = referralRequest.AddressLine2;
        AddressTown = referralRequest.AddressTown;
        AddressCounty = referralRequest.AddressCounty;
        AddressPostcode = referralRequest.AddressPostcode;
    }
}
