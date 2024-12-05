namespace HerPublicWebsite.BusinessLogic.Models;
public class ReferralRequestFollowUp
{
    public int Id { get; set; }
    public ReferralRequest ReferralRequest { get; set; }
    public int ReferralRequestId { get; set; }
    public string Token {get; set;}
    public bool? WasFollowedUp { get; set; }
    public DateTime? DateOfFollowUpResponse { get; set; }
    public ReferralRequestFollowUp() {}
    
    public ReferralRequestFollowUp(ReferralRequest referralRequest, string token)
    {
        ReferralRequest = referralRequest;
        Token = token;
    }
}
