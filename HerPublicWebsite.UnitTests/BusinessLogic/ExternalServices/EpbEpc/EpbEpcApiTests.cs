using FluentAssertions;
using NUnit.Framework;
using HerPublicWebsite.BusinessLogic.Models;
using HerPublicWebsite.BusinessLogic.Models.Enums;
using HerPublicWebsite.BusinessLogic.ExternalServices.EpbEpc;
using Microsoft.Extensions.Options;
using System.Net.Http;
using RichardSzalay.MockHttp;
using HerPublicWebsite.BusinessLogic.ExternalServices.Common;
using System.Diagnostics;
using System.Threading.Tasks;
using System;

namespace Tests.BusinessLogic.ExternalServices.EpbEpc;

[TestFixture]
public class EpbEpcApiTests
{
    private IEpcApi epcApi;
    private MockHttpMessageHandler mockHttpHandler;

    [SetUp]
    public void Setup()
    {
        epcApi = new EpbEpcApi(Options.Create(new EpbEpcConfiguration
        {
            Token = "foobar",
            BaseUrl = "http://test.com"
        }));
        mockHttpHandler = new MockHttpMessageHandler();
        HttpRequestHelper.handler = mockHttpHandler;
    }

    private readonly EpcAssessment correctAssessment = new()
    {
        Address = new EpcAddress
        {
            AddressLine1 = "22 Acacia Avenue", AddressLine2 = "Upper Wellgood", AddressLine3 = "",
            AddressLine4 = "", Town = "Fulchester", Postcode = "FL23 4JA"
        },
        Uprn = "001234567890", LodgementDate = new DateTime(2020, 2, 29), CurrentBand = EpcRating.D
    };
    
    [Test]
    public async Task GetEpcFromUpn()
    {
        mockHttpHandler.Expect("http://test.com/retrofit-funding/assessments")
            .WithHeaders("Authorization", "Basic foobar")
            .Respond("application/json", @"{
          'data': {
            'assessment': {
              'address': {
                'addressLine1': '22 Acacia Avenue',
                'addressLine2': 'Upper Wellgood',
                'addressLine3': '',
                'addressLine4': '',
                'town': 'Fulchester',
                'postcode': 'FL23 4JA'
              },
              'uprn': '001234567890',
              'lodgementDate': '2020-02-29',
              'currentBand': 'D'
            }
          }
        }");

        var assessment = await epcApi.EpcFromUprn("001234567890");
        assessment.Should().Be(correctAssessment);
    }
}
