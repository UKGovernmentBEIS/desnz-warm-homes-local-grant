using Bogus;
using HerPublicWebsite.BusinessLogic.Models;
using HerPublicWebsite.BusinessLogic.Models.Enums;

namespace HerPublicWebsite.ManagementShell;

public interface IFakeReferralGenerator
{
    IEnumerable<ReferralRequest> GenerateFakeReferralRequests(int count);
}

public class FakeReferralGenerator : IFakeReferralGenerator
{
    public IEnumerable<ReferralRequest> GenerateFakeReferralRequests(int count)
    {
        var id = 100;
        // TODO: make sure this ID is unique
        var custodianCodes = LocalAuthorityData.LocalAuthorityDetailsByCustodianCode.Keys;

        var referralsSchema = new Faker<ReferralRequest>()
            .RuleFor(rr => rr.Id, f => id++)
            .RuleFor(rr => rr.AddressLine1, f => f.Address.StreetAddress())
            .RuleFor(rr => rr.AddressLine2, f => "")
            .RuleFor(rr => rr.AddressTown, f => f.Address.City())
            .RuleFor(rr => rr.AddressCounty, f => f.Address.County())
            .RuleFor(rr => rr.AddressPostcode, f => f.Address.ZipCode()) // TODO: not a postcode
            .RuleFor(rr => rr.CustodianCode, f => f.PickRandom<string>(custodianCodes))
            .RuleFor(rr => rr.Uprn, f => f.Random.Long(10000000000, 999999999999).ToString())
            .RuleFor(rr => rr.EpcRating, f => f.Random.Enum<EpcRating>())
            .RuleFor(rr => rr.IsLsoaProperty, f => f.Random.Bool())
            .RuleFor(rr => rr.HasGasBoiler, f => HasGasBoiler.No)
            .RuleFor(rr => rr.IncomeBand,
                f => f.PickRandom(IncomeBand.UnderOrEqualTo36000, IncomeBand.GreaterThan36000))
            .RuleFor(rr => rr.RequestDate, f => f.Date.Past())
            .RuleFor(rr => rr.ReferralWrittenToCsv, f => f.Random.Bool(0.8f))
            .RuleFor(rr => rr.ContactEmailAddress, f => f.Internet.Email())
            .RuleFor(rr => rr.ContactTelephone, f => f.Phone.PhoneNumber("0#### ######"))
            .RuleFor(rr => rr.FullName, f => f.Name.FullName())
            .RuleFor(rr => rr.EpcLodgementDate, f => f.Date.Past(10))
            .RuleFor(rr => rr.EpcConfirmation, f => f.Random.Enum<EpcConfirmation>())
            .RuleFor(rr => rr.FollowUpEmailSent, f => f.Random.Bool())
            .RuleFor(rr => rr.WasSubmittedToPendingLocalAuthority, f => f.Random.Bool())
            // TODO: random chance to redact one of email or phone or neither
            // TODO: random chance to redact all epc info
            .FinishWith((_, rr) => rr.UpdateReferralCode());

            return referralsSchema.Generate(count);
    }
}