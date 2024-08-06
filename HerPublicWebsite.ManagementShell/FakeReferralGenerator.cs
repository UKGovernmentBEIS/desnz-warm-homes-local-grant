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
            // Number, Street name
            // Street name
            .RuleFor(rr => rr.AddressLine1, f => f.Address.StreetAddress())
            // 40% blank, 60% a second line of address
            .RuleFor(rr => rr.AddressLine2, f => "")
            .RuleFor(rr => rr.AddressTown, f => f.Address.City())
            // NULL for almost all referrals
            .RuleFor(rr => rr.AddressCounty, f => f.Address.County())
            .RuleFor(rr => rr.AddressPostcode, f => f.Address.ZipCode()) // TODO: not a postcode
            .RuleFor(rr => rr.CustodianCode, f => f.PickRandom<string>(custodianCodes))
            // 8, 11 or 12 digits observed
            .RuleFor(rr => rr.Uprn, f => f.Random.Long(10000000000, 999999999999).ToString())
            // A 0%
            // B 0%
            // C 1%
            // D 20%
            // E 20%
            // F 10%
            // G 4%
            // Unknown 40%
            // Expired 15%
            .RuleFor(rr => rr.EpcRating, f => f.Random.Enum<EpcRating>())
            // true 25%
            .RuleFor(rr => rr.IsLsoaProperty, f => f.Random.Bool())
            // No 100%
            .RuleFor(rr => rr.HasGasBoiler, f => HasGasBoiler.No)
            // For data after April 2024
            //  < 36000 10% 
            //  > 36000 90%
            // For all time data
            //  < 31000 40% 
            //  > 31000 4%
            //  < 34500 1%
            //  > 34500 0%
            //  < 36000 50%
            //  > 36000 5%
            .RuleFor(rr => rr.IncomeBand,
                f => f.PickRandom(IncomeBand.UnderOrEqualTo36000, IncomeBand.GreaterThan36000))
            .RuleFor(rr => rr.RequestDate, f => f.Date.Past())
            // false only for referrals that are today
            .RuleFor(rr => rr.ReferralWrittenToCsv, f => f.Random.Bool(0.8f))
            // telephone only 5%
            // email only 35%
            // both 60%
            .RuleFor(rr => rr.ContactEmailAddress, f => f.Internet.Email())
            .RuleFor(rr => rr.ContactTelephone, f => f.Phone.PhoneNumber("0#### ######"))
            .RuleFor(rr => rr.FullName, f => f.Name.FullName())
            // oldest 2008
            // newest this month
            // 40% null
            .RuleFor(rr => rr.EpcLodgementDate, f => f.Date.Past(10))
            // not null 1%
            // yes and unknown 0% of this
            // null if EpcLodgementDate is
            .RuleFor(rr => rr.EpcConfirmation, f => f.Random.Enum<EpcConfirmation>())
            // true 75%
            .RuleFor(rr => rr.FollowUpEmailSent, f => f.Random.Bool())
            // true 15%
            .RuleFor(rr => rr.WasSubmittedToPendingLocalAuthority, f => f.Random.Bool())
            // TODO: random chance to redact one of email or phone or neither
            // TODO: random chance to redact all epc info
            .FinishWith((_, rr) => rr.UpdateReferralCode());

            return referralsSchema.Generate(count);
    }
}