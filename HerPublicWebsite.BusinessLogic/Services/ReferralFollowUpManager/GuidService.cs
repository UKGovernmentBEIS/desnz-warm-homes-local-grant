namespace HerPublicWebsite.BusinessLogic.Services.ReferralFollowUps;
public interface IGuidService { string NewGuidString(); }

public class GuidService : IGuidService
{
    public string NewGuidString() { 
        return Guid.NewGuid().ToString(); 
        }
}
