using System;
using System.Linq;
using FluentAssertions;
using WhlgPublicWebsite.BusinessLogic.ExternalServices.EpbEpc;
using WhlgPublicWebsite.BusinessLogic.Models;
using WhlgPublicWebsite.BusinessLogic.Models.Enums;
using NUnit.Framework;

namespace Tests.BusinessLogic.ExternalServices.EpbEpc;

[TestFixture]
public class EpcAssessmentDtoTests
{
    [TestCaseSource(nameof(EpcParseTestCases))]
    public void CanParseEpcDto(EpcTestCase testCase)
    {
        // Act
        var epc = testCase.Input.Parse();
        
        // Assert
        epc.Should().BeEquivalentTo(testCase.Output);
    }

    private static readonly EpcTestCase[] AddressTestCases =
        {
            new(
                "Can handle null addresss",
                new EpcAssessmentDto
                {
                    Address = null
                },
                new EpcDetails
                {
                    AddressLine1 = null,
                    AddressLine2 = null,
                    AddressLine3 = null,
                    AddressLine4 = null,
                    AddressTown = null,
                    AddressPostcode = null,
                    EpcRating = EpcRating.Unknown
                }),
            new(
                "Can parse addresss",
                new EpcAssessmentDto
                {
                    Address = new EpcAddressDto
                    {
                        AddressLine1 = "line1",
                        AddressLine2 = "line2",
                        AddressLine3 = "line3",
                        AddressLine4 = "line4",
                        Town = "town1",
                        Postcode = "ab1 2cd"
                    }
                },
                new EpcDetails
                {
                    AddressLine1 = "line1",
                    AddressLine2 = "line2",
                    AddressLine3 = "line3",
                    AddressLine4 = "line4",
                    AddressTown = "town1",
                    AddressPostcode = "ab1 2cd",
                    EpcRating = EpcRating.Unknown
                })
        };
    
    private static readonly EpcTestCase[] LodgementDateTestCases =
        new (string Descrption, string inputLodgementDate, DateTime? expectedLodgementDate)[]
            {
                ("Can handle null lodgement year", null, null),
                ("Can parse lodgement year", "2012-12-22", new DateTime(2012, 12, 22)),
            }
            .Select(p => new EpcTestCase(
                p.Descrption, 
                new EpcAssessmentDto
                { 
                    Address = new EpcAddressDto(),
                    LodgementDate = p.inputLodgementDate
                }, 
                new EpcDetails
                {
                    LodgementDate = p.expectedLodgementDate,
                    EpcRating = EpcRating.Unknown
                }))
            .ToArray();
    
    private static readonly EpcTestCase[] ExpiryDateTestCases =
        new (string Descrption, string inputExpiryDate, DateTime? expectedExpiryDate)[]
            {
                ("Can handle null expiry year", null, null),
                ("Can parse expiry year", "2012-12-22", new DateTime(2012, 12, 22)),
            }
            .Select(p => new EpcTestCase(
                p.Descrption, 
                new EpcAssessmentDto
                { 
                    Address = new EpcAddressDto(),
                    ExpiryDate = p.inputExpiryDate
                }, 
                new EpcDetails
                {
                    ExpiryDate = p.expectedExpiryDate,
                    EpcRating = EpcRating.Unknown
                }))
            .ToArray();
    
    private static readonly EpcTestCase[] CurrentBandTestCases =
        new (string Descrption, string inputBand, EpcRating expectedRating)[]
            {
                ("Can handle null band", null, EpcRating.Unknown),
                ("Can handle unexpected band", "not a band", EpcRating.Unknown),
                ("Can parse band A", "A", EpcRating.A),
                ("Can parse band B", "B", EpcRating.B),
                ("Can parse band C", "C", EpcRating.C),
                ("Can parse band D", "D", EpcRating.D),
                ("Can parse band E", "E", EpcRating.E),
                ("Can parse band F", "F", EpcRating.F),
                ("Can parse band G", "G", EpcRating.G),
            }
            .Select(p => new EpcTestCase(
                p.Descrption, 
                new EpcAssessmentDto
                { 
                    Address = new EpcAddressDto(),
                    CurrentBand = p.inputBand
                }, 
                new EpcDetails
                {
                    EpcRating = p.expectedRating
                }))
            .ToArray();

    private static readonly EpcTestCase[] PropertyTypeTestCases =
        new (string Descrption, string inputPropertyType, PropertyType? expectedPropertyType)[]
            {
                ("Can handle null property type", null, null),
                ("Can parse property type house", "house", PropertyType.House),
                ("Can parse property type bungalow", "bungalow", PropertyType.Bungalow),
                ("Can parse property type flat", "flat", PropertyType.ApartmentFlatOrMaisonette),
                ("Can parse property type maisonette", "maisonette", PropertyType.ApartmentFlatOrMaisonette),
            }
            .Select(p => new EpcTestCase(
                p.Descrption, 
                new EpcAssessmentDto
                { 
                    Address = new EpcAddressDto(),
                    PropertyType = p.inputPropertyType
                }, 
                new EpcDetails
                {
                    PropertyType = p.expectedPropertyType,
                    EpcRating = EpcRating.Unknown
                }))
            .ToArray();
    
    private static readonly EpcTestCase[] HouseTypeTestCases =
        new (string Descrption, string inputPropertyType, HouseType? expectedHouseType)[]
            {
                ("Can parse house type detached", "detached house", HouseType.Detached),
                ("Can parse house type semi-detached", "semi-detached house", HouseType.SemiDetached),
                ("Can parse house type mid-terrace", "mid-terrace house", HouseType.Terraced),
                ("Can parse house type end terrace", "end-terrace house", HouseType.EndTerrace),
            }
            .Select(p => new EpcTestCase(
                p.Descrption, 
                new EpcAssessmentDto
                {
                    Address = new EpcAddressDto(),
                    PropertyType = p.inputPropertyType
                }, 
                new EpcDetails
                {
                    PropertyType = PropertyType.House,
                    HouseType = p.expectedHouseType,
                    EpcRating = EpcRating.Unknown
                }))
            .ToArray();
    
    private static readonly EpcTestCase[] BungalowTypeTestCases =
        new (string Descrption, string inputPropertyType, BungalowType? expectedBungalowType)[]
            {
                ("Can parse bungalow type detached", "detached bungalow", BungalowType.Detached),
                ("Can parse bungalow type semi-detached", "semi-detached bungalow", BungalowType.SemiDetached),
                ("Can parse bungalow type mid-terrace", "mid-terrace bungalow", BungalowType.Terraced),
                ("Can parse bungalow type end terrace", "end-terrace bungalow", BungalowType.EndTerrace),
            }
            .Select(p => new EpcTestCase(
                p.Descrption, 
                new EpcAssessmentDto
                {
                    Address = new EpcAddressDto(),
                    PropertyType = p.inputPropertyType
                }, 
                new EpcDetails
                {
                    PropertyType = PropertyType.Bungalow,
                    BungalowType = p.expectedBungalowType,
                    EpcRating = EpcRating.Unknown
                }))
            .ToArray();
    
    private static readonly EpcTestCase[] FlatTypeTestCases =
        new (string Descrption, string inputPropertyType, FlatType? expectedFlatType)[]
            {
                ("Can parse flat type ground flat (basement)", "basement flat", FlatType.GroundFloor),
                ("Can parse flat type ground flat", "ground flat", FlatType.GroundFloor),
                ("Can parse flat type middle flat", "mid flat", FlatType.MiddleFloor),
                ("Can parse flat type top flat", "top flat", FlatType.TopFloor),
            }
            .Select(p => new EpcTestCase(
                p.Descrption, 
                new EpcAssessmentDto
                {
                    Address = new EpcAddressDto(),
                    PropertyType = p.inputPropertyType
                }, 
                new EpcDetails
                {
                    PropertyType = PropertyType.ApartmentFlatOrMaisonette,
                    FlatType = p.expectedFlatType,
                    EpcRating = EpcRating.Unknown
                }))
            .ToArray();

    private static readonly EpcTestCase[] EpcParseTestCases =
        Array.Empty<EpcTestCase>()
            .Concat(AddressTestCases)
            .Concat(LodgementDateTestCases)
            .Concat(ExpiryDateTestCases)
            .Concat(CurrentBandTestCases)
            .Concat(PropertyTypeTestCases)
            .Concat(HouseTypeTestCases)
            .Concat(BungalowTypeTestCases)
            .Concat(FlatTypeTestCases)
            .ToArray();

    public class EpcTestCase
    {
        public readonly string Description;
        public readonly EpcAssessmentDto Input;
        public readonly EpcDetails Output;

        public EpcTestCase(
            string description, EpcAssessmentDto input, EpcDetails output)
        {
            Description = description;
            Input = input;
            Output = output;
        }

        public override string ToString()
        {
            return Description;
        }
    }
}
