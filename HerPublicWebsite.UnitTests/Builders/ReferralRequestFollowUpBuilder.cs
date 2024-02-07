using System;
using HerPublicWebsite.BusinessLogic.Models;

namespace Tests.Builders;

public class ReferralRequestFollowUpBuilder
{
    private readonly ReferralRequestFollowUp referralRequestFollowUp;

    public ReferralRequestFollowUpBuilder(int id)
    {
        referralRequestFollowUp = new ReferralRequestFollowUp
        {
            Id = id,
            WasFollowedUp = null,
        };
    }

    public ReferralRequestFollowUp Build(ReferralRequest referralRequest)
    {
        referralRequestFollowUp.ReferralRequest = referralRequest;
        referralRequestFollowUp.ReferralRequestId = referralRequest.Id;
        return referralRequestFollowUp;
    }

    public ReferralRequestFollowUpBuilder WithToken(string token)
    {
        referralRequestFollowUp.Token = token;
        return this;
    }

    public ReferralRequestFollowUpBuilder WithWasFollowedUp(bool? wasFollowedUp)
    {
        referralRequestFollowUp.WasFollowedUp = wasFollowedUp;
        return this;
    }

    public ReferralRequestFollowUpBuilder WithRequestFollowUpDate(DateTime dateOfFollowUpResponse)
    {
        referralRequestFollowUp.DateOfFollowUpResponse = dateOfFollowUpResponse;
        return this;
    }
}
