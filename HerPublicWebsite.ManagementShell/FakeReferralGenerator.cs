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
        
        var referralsSchema = new Faker<ReferralRequest>("en_GB")
            .RuleFor(rr => rr.Id, f => id++)
            // Number, Street name
            // Street name
            .RuleFor(rr => rr.AddressLine1, f => f.Address.StreetAddress())
            // 40% blank, 60% a second line of address
            .RuleFor(rr => rr.AddressLine2, f => f.Address.City().OrNull(f, 0.4f))
            .RuleFor(rr => rr.AddressTown, f => f.Address.City())
            // NULL for almost all referrals
            .RuleFor(rr => rr.AddressCounty, f => null)
            .RuleFor(rr => rr.AddressPostcode, f => f.Address.ZipCode())
            .RuleFor(rr => rr.CustodianCode, f => f.PickRandom<string>(custodianCodes))
            // 8, 11 or 12 digits observed
            .RuleFor(rr => rr.Uprn, f => f.Random.Long(10000000, 999999999999).ToString())
            // Unknown 40%
            // When it is known
            //   A 1%
            //   B 1%
            //   C 1%
            //   D 25%
            //   E 25%
            //   F 15%
            //   G 7%
            //   Expired 25%
            .RuleFor(rr => rr.EpcRating, f => f.Random.WeightedRandom(
                new []
                {
                    EpcRating.A, 
                    EpcRating.B, 
                    EpcRating.C, 
                    EpcRating.D, 
                    EpcRating.E, 
                    EpcRating.F, 
                    EpcRating.G, 
                    EpcRating.Expired
                },
                new []{ 0.01f, 0.01f, 0.01f, 0.25f, 0.25f, 0.15f, 0.07f, 0.25f }))
            // true 25%
            .RuleFor(rr => rr.IsLsoaProperty, f => f.Random.Bool(0.25f))
            // No 100%
            .RuleFor(rr => rr.HasGasBoiler, f => HasGasBoiler.No)
            // < 31000 40% 
            // > 31000 4%
            // < 34500 1%
            // > 34500 0%
            // < 36000 50%
            // > 36000 5%
            .RuleFor(rr => rr.IncomeBand,
                f => f.Random.WeightedRandom(
                    new []
                    {
                        IncomeBand.UnderOrEqualTo31000,
                        IncomeBand.GreaterThan31000,
                        IncomeBand.UnderOrEqualTo34500,
                        IncomeBand.GreaterThan34500,
                        IncomeBand.UnderOrEqualTo36000,
                        IncomeBand.GreaterThan36000
                    },
                    new []{ 0.4f, 0.03f, 0.01f, 0.01f, 0.5f, 0.05f }
                    ))
            .RuleFor(rr => rr.RequestDate, f => f.Date.Past())
            // false only for referrals that are today
            .RuleFor(rr => rr.ReferralWrittenToCsv, _ => true)
            // telephone only 5%
            // email only 35%
            // both 60%
            .RuleFor(rr => rr.ContactEmailAddress, f => f.Internet.Email())
            .RuleFor(rr => rr.ContactTelephone, f => f.Phone.PhoneNumber())
            .RuleFor(rr => rr.FullName, f => f.Name.FullName())
            .RuleFor(rr => rr.EpcLodgementDate, f => f.Date.Past(10))
            // not null 1%
            // yes and unknown 0% of this
            // null if EpcLodgementDate is
            .RuleFor(rr => rr.EpcConfirmation, f => f.PickRandom(EpcConfirmation.Yes, EpcConfirmation.No, EpcConfirmation.Unknown).OrNull(f, 0.99f))
            // true 75%
            .RuleFor(rr => rr.FollowUpEmailSent, f => f.Random.Bool(0.75f))
            // true 15%
            .RuleFor(rr => rr.WasSubmittedToPendingLocalAuthority, f => f.Random.Bool(0.15f))
            .FinishWith((f, rr) =>
            {
                rr.UpdateReferralCode();
                // only referrals not written to csv are those made today
                if (rr.RequestDate.Date == DateTime.Today)
                {
                    rr.ReferralWrittenToCsv = false;
                }

                // 40% chance to redact all EPC info
                var epcFound = f.Random.Bool(0.6f);
                if (!epcFound)
                {
                    rr.EpcRating = EpcRating.Unknown;
                    rr.EpcLodgementDate = null;
                    rr.EpcConfirmation = null;
                }
                
                // decide what contact info the user gave (and redact if they did not)
                var contactInfoGiven = f.Random.WeightedRandom(new [] { ContactInfoGiven.PhoneNumber, ContactInfoGiven.Email, ContactInfoGiven.PhoneNumberAndEmail }, 
                    new []{ 0.05f, 0.35f, 0.6f });
                switch (contactInfoGiven)
                {
                    case ContactInfoGiven.PhoneNumber:
                        rr.ContactEmailAddress = null;
                        break;
                    case ContactInfoGiven.Email:
                        rr.ContactTelephone = null;
                        break;
                    case ContactInfoGiven.PhoneNumberAndEmail: default:
                        break;
                }
            });

            return referralsSchema.Generate(count);
    }
    
    private enum ContactInfoGiven
    {
        PhoneNumber,
        Email,
        PhoneNumberAndEmail
    }
}