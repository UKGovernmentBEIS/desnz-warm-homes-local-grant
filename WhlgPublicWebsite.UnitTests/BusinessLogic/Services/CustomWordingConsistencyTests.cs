using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using WhlgPublicWebsite.BusinessLogic.Models;

namespace Tests.BusinessLogic.Services;

[TestFixture]
public class CustomWordingConsistencyTests
{
    // If any of these tests start failing, an LA with custom view partials or email overrides
    // has likely changed status. Review whether the custom wordings for that LA are still correct,
    // update any wording if necessary, then update the expected status in this list. 
    // It may also be the case that there is a new consortium with custom wordings, or that the LAs 
    // that are part of a consortium have changed. In this case, update the list
    // and the LaCodesWithCustomWordings_ContainsExactlyAllLasWithCustomWordings test
    private static readonly (string CustodianCode, LocalAuthorityData.LocalAuthorityStatus ExpectedStatus)[]
        LaCodesWithCustomWordings =
        [
            // West Midlands Combined Authority (custom: Eligible, Confirmation, NotParticipating, email template)
            ("4605", LocalAuthorityData.LocalAuthorityStatus.Live), // Birmingham City Council
            ("4610", LocalAuthorityData.LocalAuthorityStatus.Live), // Coventry City Council
            ("4615", LocalAuthorityData.LocalAuthorityStatus.Live), // Dudley Borough Council
            ("4620", LocalAuthorityData.LocalAuthorityStatus.Live), // Sandwell Borough Council
            ("4625", LocalAuthorityData.LocalAuthorityStatus.Live), // Solihull Borough Council
            ("4630", LocalAuthorityData.LocalAuthorityStatus.Live), // Walsall Metropolitan Borough Council
            ("4635", LocalAuthorityData.LocalAuthorityStatus.Live), // Wolverhampton City Council

            // Broadland District Council (custom: Eligible, Confirmation)
            ("2610", LocalAuthorityData.LocalAuthorityStatus.Live), // Broadland District Council
            ("2605", LocalAuthorityData.LocalAuthorityStatus.Live), // Breckland District Council
            ("2635", LocalAuthorityData.LocalAuthorityStatus.Live), // Borough Council of King's Lynn & West Norfolk
            ("2620", LocalAuthorityData.LocalAuthorityStatus.Live), // North Norfolk District Council
            ("2630", LocalAuthorityData.LocalAuthorityStatus.Live), // South Norfolk District Council

            // Portsmouth City Council (custom: Eligible, Confirmation, email overrides)
            ("1775", LocalAuthorityData.LocalAuthorityStatus.ReferralsPaused), // Portsmouth City Council
            ("3805", LocalAuthorityData.LocalAuthorityStatus.ReferralsPaused), // Adur District Council
            ("3810", LocalAuthorityData.LocalAuthorityStatus.ReferralsPaused), // Arun District Council
            ("1705", LocalAuthorityData.LocalAuthorityStatus.ReferralsPaused), // Basingstoke and Deane Borough Council
            ("235", LocalAuthorityData.LocalAuthorityStatus.ReferralsPaused), // Bedford Borough Council
            ("335", LocalAuthorityData.LocalAuthorityStatus.ReferralsPaused), // Bracknell Forest Borough Council
            ("1445", LocalAuthorityData.LocalAuthorityStatus.ReferralsPaused), // Brighton and Hove City Council
            ("3815", LocalAuthorityData.LocalAuthorityStatus.ReferralsPaused), // Chichester District Council
            ("3820", LocalAuthorityData.LocalAuthorityStatus.ReferralsPaused), // Crawley Borough Council
            ("5240", LocalAuthorityData.LocalAuthorityStatus.ReferralsPaused), // Croydon Council
            ("1710", LocalAuthorityData.LocalAuthorityStatus.ReferralsPaused), // East Hampshire District Council
            ("1715", LocalAuthorityData.LocalAuthorityStatus.ReferralsPaused), // Eastleigh Borough Council
            ("1720", LocalAuthorityData.LocalAuthorityStatus.ReferralsPaused), // Fareham Borough Council
            ("1725", LocalAuthorityData.LocalAuthorityStatus.ReferralsPaused), // Gosport Borough Council
            ("1730", LocalAuthorityData.LocalAuthorityStatus.ReferralsPaused), // Hart District Council
            ("1735", LocalAuthorityData.LocalAuthorityStatus.ReferralsPaused), // Havant Borough Council
            ("3825", LocalAuthorityData.LocalAuthorityStatus.ReferralsPaused), // Horsham District Council
            ("2114", LocalAuthorityData.LocalAuthorityStatus.ReferralsPaused), // Isle of Wight Council
            ("230", LocalAuthorityData.LocalAuthorityStatus.ReferralsPaused), // Luton Borough Council
            ("435", LocalAuthorityData.LocalAuthorityStatus.ReferralsPaused), // Milton Keynes Council
            ("3830", LocalAuthorityData.LocalAuthorityStatus.ReferralsPaused), // Mid Sussex District Council
            ("1740", LocalAuthorityData.LocalAuthorityStatus.ReferralsPaused), // New Forest District Council
            ("345", LocalAuthorityData.LocalAuthorityStatus.ReferralsPaused), // Reading Borough Council
            ("355", LocalAuthorityData.LocalAuthorityStatus.ReferralsPaused), // Royal Borough of Windsor and Maidenhead
            ("1750", LocalAuthorityData.LocalAuthorityStatus.ReferralsPaused), // Rushmoor Borough Council
            ("2470", LocalAuthorityData.LocalAuthorityStatus.ReferralsPaused), // Rutland County Council
            ("1780", LocalAuthorityData.LocalAuthorityStatus.ReferralsPaused), // Southampton City Council
            ("1760", LocalAuthorityData.LocalAuthorityStatus.ReferralsPaused), // Test Valley Borough Council
            ("340", LocalAuthorityData.LocalAuthorityStatus.ReferralsPaused), // West Berkshire Council
            ("1765", LocalAuthorityData.LocalAuthorityStatus.ReferralsPaused), // Winchester City Council
            ("360", LocalAuthorityData.LocalAuthorityStatus.ReferralsPaused), // Wokingham Borough Council
            ("3835", LocalAuthorityData.LocalAuthorityStatus.ReferralsPaused), // Worthing Borough Council

            // Greater London Authority (custom: Eligible, Confirmation, ReferralsPaused, email overrides)
            ("5090", LocalAuthorityData.LocalAuthorityStatus.Live), // Barnet Council
            ("5150", LocalAuthorityData.LocalAuthorityStatus.Live), // Brent Council
            ("5180", LocalAuthorityData.LocalAuthorityStatus.Live), // London Borough of Bromley
            ("5210", LocalAuthorityData.LocalAuthorityStatus.Live), // London Borough of Camden
            ("5030", LocalAuthorityData.LocalAuthorityStatus.Live), // City of London Corporation
            ("5270", LocalAuthorityData.LocalAuthorityStatus.Live), // Ealing Council
            ("5300", LocalAuthorityData.LocalAuthorityStatus.Live), // Enfield Council
            ("5360", LocalAuthorityData.LocalAuthorityStatus.Live), // Hackney Council
            ("5420", LocalAuthorityData.LocalAuthorityStatus.Live), // Haringey Council
            ("5450", LocalAuthorityData.LocalAuthorityStatus.Live), // London Borough of Harrow
            ("5480", LocalAuthorityData.LocalAuthorityStatus.Live), // London Borough of Havering
            ("5510", LocalAuthorityData.LocalAuthorityStatus.Live), // Hillingdon Council
            ("5540", LocalAuthorityData.LocalAuthorityStatus.Live), // London Borough of Hounslow
            ("5570", LocalAuthorityData.LocalAuthorityStatus.Live), // Islington Council
            ("5120", LocalAuthorityData.LocalAuthorityStatus.Live), // London Borough of Bexley
            ("5660", LocalAuthorityData.LocalAuthorityStatus.Live), // Lambeth Council
            ("5690", LocalAuthorityData.LocalAuthorityStatus.Live), // Lewisham Council
            ("5390", LocalAuthorityData.LocalAuthorityStatus.Live), // London Borough of Hammersmith and Fulham
            ("5720", LocalAuthorityData.LocalAuthorityStatus.Live), // London Borough of Merton
            ("5750", LocalAuthorityData.LocalAuthorityStatus.Live), // Newham Council
            ("5780", LocalAuthorityData.LocalAuthorityStatus.Live), // London Borough of Redbridge
            ("5810", LocalAuthorityData.LocalAuthorityStatus.Live), // London Borough of Richmond upon Thames
            ("5330", LocalAuthorityData.LocalAuthorityStatus.Live), // Royal Borough of Greenwich
            ("5600", LocalAuthorityData.LocalAuthorityStatus.Live), // Royal Borough of Kensington and Chelsea
            ("5630", LocalAuthorityData.LocalAuthorityStatus.Live), // Royal Borough of Kingston upon Thames
            ("5840", LocalAuthorityData.LocalAuthorityStatus.Live), // Southwark Council
            ("5870", LocalAuthorityData.LocalAuthorityStatus.Live), // London Borough of Sutton
            ("5900", LocalAuthorityData.LocalAuthorityStatus.Live), // Tower Hamlets Council
            ("5960", LocalAuthorityData.LocalAuthorityStatus.Live), // Wandsworth London Borough Council
            ("5990", LocalAuthorityData.LocalAuthorityStatus.Live), // Westminster City Council

            // Oxfordshire County Council (custom: Eligible, email overrides)
            ("3100", LocalAuthorityData.LocalAuthorityStatus.Live), // Oxfordshire County Council
            ("3105", LocalAuthorityData.LocalAuthorityStatus.Live), // Cherwell District Council
            ("3110", LocalAuthorityData.LocalAuthorityStatus.Live), // Oxford City Council
            ("3115", LocalAuthorityData.LocalAuthorityStatus.Live), // South Oxfordshire District Council
            ("3120", LocalAuthorityData.LocalAuthorityStatus.Live), // Vale of White Horse District Council
            ("3125", LocalAuthorityData.LocalAuthorityStatus.Live), // West Oxfordshire District Council

            // Liverpool City Region Combined Authority - managed only (custom: Eligible)
            ("650", LocalAuthorityData.LocalAuthorityStatus.Live), // Halton Borough Council
            ("4305", LocalAuthorityData.LocalAuthorityStatus.Live), // Knowsley Council
            ("4315", LocalAuthorityData.LocalAuthorityStatus.Live), // St. Helens Borough Council
            ("4325", LocalAuthorityData.LocalAuthorityStatus.Live), // Wirral Council

            // Greater Manchester Combined Authority (custom: NotParticipating)
            ("4205", LocalAuthorityData.LocalAuthorityStatus.NotParticipating), // Bolton Metropolitan Borough Council
            ("4210", LocalAuthorityData.LocalAuthorityStatus.NotParticipating), // Bury Metropolitan Borough Council
            ("4215", LocalAuthorityData.LocalAuthorityStatus.NotParticipating), // Manchester City Council
            ("4220", LocalAuthorityData.LocalAuthorityStatus.NotParticipating), // Oldham Metropolitan Borough Council
            ("4225", LocalAuthorityData.LocalAuthorityStatus.NotParticipating), // Rochdale Metropolitan Borough Council
            ("4230", LocalAuthorityData.LocalAuthorityStatus.NotParticipating), // Salford City Council
            ("4240", LocalAuthorityData.LocalAuthorityStatus.NotParticipating), // Tameside Metropolitan Borough Council
            ("4245", LocalAuthorityData.LocalAuthorityStatus.NotParticipating), // Trafford Metropolitan Borough Council
            ("4250", LocalAuthorityData.LocalAuthorityStatus.NotParticipating) // Wigan Metropolitan Borough Council
        ];

    [Test]
    public void LaCodesWithCustomWordings_ContainsExactlyAllLasWithCustomWordings()
    {
        var expectedCodes = new HashSet<string>(
            LocalAuthorityData.FilterCustodianCodes(ConsortiumNames.WestMidlandsCombinedAuthority)
                .Concat(LocalAuthorityData.FilterCustodianCodes(ConsortiumNames.BroadlandDistrictCouncil))
                .Concat(LocalAuthorityData.FilterCustodianCodes(ConsortiumNames.PortsmouthCityCouncil))
                .Concat(LocalAuthorityData.FilterCustodianCodes(ConsortiumNames.GreaterLondonAuthority))
                .Concat(LocalAuthorityData.FilterCustodianCodes(ConsortiumNames.OxfordshireCountyCouncil))
                .Concat(LocalAuthorityData.FilterCustodianCodes(ConsortiumNames.GreaterManchesterCombinedAuthority))
                .Concat(LocalAuthorityData.ManagedByLcrcaCodes)
        );

        var actualCodes = new HashSet<string>(
            LaCodesWithCustomWordings.Select(t => t.CustodianCode)
        );

        actualCodes.Should().BeEquivalentTo(expectedCodes);
    }

    [TestCaseSource(nameof(LaCodesWithCustomWordings))]
    public void LocalAuthorityWithCustomWordingHasExpectedStatus(
        (string CustodianCode, LocalAuthorityData.LocalAuthorityStatus ExpectedStatus) testCase)
    {
        LocalAuthorityData.LocalAuthorityDetailsByCustodianCode[testCase.CustodianCode].Status
            .Should().Be(testCase.ExpectedStatus);
    }
}