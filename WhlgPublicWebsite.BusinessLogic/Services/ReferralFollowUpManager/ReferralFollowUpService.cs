using HerPublicWebsite.BusinessLogic.Models;

namespace HerPublicWebsite.BusinessLogic.Services.ReferralFollowUps;

public interface IReferralFollowUpService
{
    public Task<ReferralRequestFollowUp> CreateReferralRequestFollowUp(ReferralRequest referralRequest);
    public Task<ReferralRequestFollowUp> GetReferralRequestFollowUpByToken(string token);
    public Task RecordFollowUpResponseForToken(string token, bool hasFollowedUp);
}

public class ReferralFollowUpService : IReferralFollowUpService
{
    private readonly IDataAccessProvider dataAccessProvider;
    private readonly IGuidService guidService;
    public ReferralFollowUpService(IDataAccessProvider dataAccessProvider, IGuidService guidService)
    {
        this.dataAccessProvider = dataAccessProvider;
        this.guidService = guidService;
    }

    public async Task<ReferralRequestFollowUp> CreateReferralRequestFollowUp(ReferralRequest referralRequest)
    {
        var token = GenerateFollowUpToken();
        var referralRequestFollowUp = new ReferralRequestFollowUp(referralRequest, token);
        await dataAccessProvider.PersistReferralFollowUpToken(referralRequestFollowUp);
        return referralRequestFollowUp;
    }
    public async Task<ReferralRequestFollowUp> GetReferralRequestFollowUpByToken(string token)
    {
        return await dataAccessProvider.GetReferralFollowUpByToken(token);
    }
    public async Task RecordFollowUpResponseForToken(string token, bool hasFollowedUp)
    {
        var referralRequestFollowUp = await dataAccessProvider.GetReferralFollowUpByToken(token);
        if (referralRequestFollowUp.WasFollowedUp is not null){
            throw new InvalidOperationException();
        }
        await dataAccessProvider.UpdateReferralFollowUpByTokenWithWasFollowedUp(token, hasFollowedUp);
    }

    private string GenerateFollowUpToken()
    {
        return guidService.NewGuidString();
    }
}
