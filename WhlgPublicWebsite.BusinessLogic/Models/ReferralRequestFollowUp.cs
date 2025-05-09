namespace WhlgPublicWebsite.BusinessLogic.Models;

public class ReferralRequestFollowUp : IEntityWithRowVersioning
{
    public int Id { get; set; }
    public ReferralRequest ReferralRequest { get; set; }
    public int ReferralRequestId { get; set; }
    public string Token { get; set; }
    public bool? WasFollowedUp { get; set; }
    public DateTime? DateOfFollowUpResponse { get; set; }
    public uint Version { get; set; }

    public ReferralRequestFollowUp()
    {
    }

    public ReferralRequestFollowUp(ReferralRequest referralRequest, string token)
    {
        ReferralRequest = referralRequest;
        Token = token;
    }
}