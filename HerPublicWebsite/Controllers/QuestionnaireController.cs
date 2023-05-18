using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using HerPublicWebsite.BusinessLogic.ExternalServices.OsPlaces;
using HerPublicWebsite.BusinessLogic.Extensions;
using HerPublicWebsite.BusinessLogic.Models;
using HerPublicWebsite.BusinessLogic.Models.Enums;
using HerPublicWebsite.BusinessLogic.Services.QuestionFlow;
using HerPublicWebsite.ExternalServices.GoogleAnalytics;
using HerPublicWebsite.Models.Questionnaire;
using HerPublicWebsite.Services;
using HerPublicWebsite.Services.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;

namespace HerPublicWebsite.Controllers;

[Route("questionnaire")]
public class QuestionnaireController : Controller
{
    private readonly IQuestionFlowService questionFlowService;
    private readonly CookieService cookieService;
    private readonly GoogleAnalyticsService googleAnalyticsService;
    private readonly QuestionnaireService questionnaireService;
    private readonly IOsPlacesApi osPlaces;
    private readonly ILogger logger;

    public QuestionnaireController(
        IQuestionFlowService questionFlowService,
        CookieService cookieService,
        GoogleAnalyticsService googleAnalyticsService,
        QuestionnaireService questionnaireService,
        IOsPlacesApi osPlaces,
        ILogger<QuestionnaireController> logger
    )
    {
        this.questionFlowService = questionFlowService;
        this.cookieService = cookieService;
        this.googleAnalyticsService = googleAnalyticsService;
        this.questionnaireService = questionnaireService;
        this.osPlaces = osPlaces;
        this.logger = logger;
    }

    [HttpGet("")]
    public IActionResult Index()
    {
        return RedirectToAction(nameof(StaticPagesController.Index), "StaticPages");
    }

    [HttpGet("boiler")]
    public IActionResult GasBoiler_Get()
    {
        var questionnaire = questionnaireService.GetQuestionnaire();
        var viewModel = new GasBoilerViewModel
        {
            HasGasBoiler = questionnaire.HasGasBoiler,
            BackLink = GetBackUrl(QuestionFlowStep.GasBoiler, questionnaire)
        };

        return View("GasBoiler", viewModel);
    }

    [HttpPost("boiler")]
    public IActionResult GasBoiler_Post(GasBoilerViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            return GasBoiler_Get();
        }

        var questionnaire = questionnaireService.UpdateGasBoiler(viewModel.HasGasBoiler!.Value);
        var nextStep = questionFlowService.NextStep(QuestionFlowStep.GasBoiler, questionnaire);

        return RedirectToNextStep(nextStep);
    }

    [HttpGet("direct-to-eco/")]
    public IActionResult DirectToEco_Get()
    {
        var viewModel = new DirectToEcoViewModel
        {
            BackLink = GetBackUrl(QuestionFlowStep.DirectToEco)
        };

        return View("DirectToEco", viewModel);
    }

    [HttpGet("country/")]
    public IActionResult Country_Get()
    {
        var questionnaire = questionnaireService.GetQuestionnaire();
        var viewModel = new CountryViewModel
        {
            Country = questionnaire.Country,
            BackLink = GetBackUrl(QuestionFlowStep.Country, questionnaire)
        };

        return View("Country", viewModel);
    }

    [HttpPost("country/")]
    public IActionResult Country_Post(CountryViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            return Country_Get();
        }

        var questionnaire = questionnaireService.UpdateCountry(viewModel.Country!.Value);
        var nextStep = questionFlowService.NextStep(QuestionFlowStep.Country, questionnaire);

        return RedirectToNextStep(nextStep);
    }

    [HttpGet("service-unsuitable/")]
    public IActionResult ServiceUnsuitable_Get()
    {
        var questionnaire = questionnaireService.GetQuestionnaire();

        var viewModel = new ServiceUnsuitableViewModel
        {
            BackLink = GetBackUrl(QuestionFlowStep.ServiceUnsuitable, questionnaire)
        };

        return View("ServiceUnsuitable", viewModel);
    }

    [HttpGet("ownership-status/")]
    public IActionResult OwnershipStatus_Get()
    {
        var questionnaire = questionnaireService.GetQuestionnaire();

        var viewModel = new OwnershipStatusViewModel()
        {
            OwnershipStatus = questionnaire.OwnershipStatus,
            BackLink = GetBackUrl(QuestionFlowStep.OwnershipStatus, questionnaire)
        };

        return View("OwnershipStatus", viewModel);
    }

    [HttpPost("ownership-status/")]
    public IActionResult OwnershipStatus_Post(OwnershipStatusViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            return Country_Get();
        }

        var questionnaire = questionnaireService.UpdateOwnershipStatus(viewModel.OwnershipStatus!.Value);
        var nextStep = questionFlowService.NextStep(QuestionFlowStep.OwnershipStatus, questionnaire);

        return RedirectToNextStep(nextStep);
    }

    [HttpGet("address/")]
    public IActionResult Address_Get()
    {
        var questionnaire = questionnaireService.GetQuestionnaire();

        var viewModel = new AddressViewModel()
        {
            BackLink = GetBackUrl(QuestionFlowStep.Address, questionnaire)
        };

        return View("Address", viewModel);
    }

    [HttpPost("address/")]
    public IActionResult Address_Post(AddressViewModel viewModel)
    {
        if (viewModel.Postcode is not null && !viewModel.Postcode.IsValidUkPostcodeFormat())
        {
            ModelState.AddModelError(nameof(AddressViewModel.Postcode), "Enter a valid UK postcode");
        }

        if (!ModelState.IsValid)
        {
            return Address_Get();
        }
        var questionnaire = questionnaireService.GetQuestionnaire();
        var nextStep = questionFlowService.NextStep(QuestionFlowStep.Address, questionnaire);
        var forwardArgs = GetActionArgumentsForQuestion(
            nextStep,
            extraRouteValues: new Dictionary<string, object>
            {
                { "postcode", viewModel.Postcode.NormaliseToUkPostcodeFormat() },
                { "buildingNameOrNumber", viewModel.BuildingNameOrNumber }
            }
        );

        return RedirectToAction(forwardArgs.Action, forwardArgs.Controller, forwardArgs.Values);
    }

    [HttpGet("address/{postcode}/{buildingNameOrNumber}")]
    public async Task<IActionResult> SelectAddress_Get(string postcode, string buildingNameOrNumber)
    {
        var questionnaire = questionnaireService.GetQuestionnaire();
        var viewModel = new SelectAddressViewModel()
        {
            Addresses = await osPlaces.GetAddressesAsync(postcode, buildingNameOrNumber),
            BackLink = GetBackUrl(QuestionFlowStep.SelectAddress, questionnaire)
        };

        TempData["Addresses"] = JsonSerializer.Serialize(viewModel.Addresses);

        return View("SelectAddress", viewModel);
    }

    [HttpPost("address/{postcode}/{buildingNameOrNumber}")]
    public async Task<IActionResult> SelectAddress_Post(SelectAddressViewModel viewModel, string postcode, string buildingNameOrNumber)
    {
        if (!ModelState.IsValid)
        {
            return await SelectAddress_Get(postcode, buildingNameOrNumber);
        }

        try
        {
            var addressResults = JsonSerializer.Deserialize<List<Address>>(TempData["Addresses"] as string ?? throw new InvalidOperationException());
            var selectedAddress = addressResults[Convert.ToInt32(viewModel.SelectedAddressIndex)];
            var questionnaire = await questionnaireService.UpdateAddressAsync(selectedAddress);

            var nextStep = questionFlowService.NextStep(QuestionFlowStep.SelectAddress, questionnaire);
            return RedirectToNextStep(nextStep);
        }
        catch (Exception e)
        {
            // This shouldn't ever happen unless something has really gone wrong, or someone's messed with the page
            // so just redirect to the beginning of the address entry
            logger.LogError("Couldn't deserialize and retrieve address from selection: {}", e.Message);
            return RedirectToAction(nameof(Address_Get), "Questionnaire");
        }
    }

    [HttpGet("review-epc")]
    public IActionResult ReviewEpc_Get()
    {
        var questionnaire = questionnaireService.GetQuestionnaire();
        var viewModel = new ReviewEpcViewModel(questionnaire.EpcDetails, questionnaire.EpcDetailsAreCorrect)
        {
            BackLink = GetBackUrl(QuestionFlowStep.ReviewEpc, questionnaire)
        };

        return View("ReviewEpc", viewModel);
    }

    [HttpPost("review-epc")]
    public IActionResult ReviewEpc_Post(ReviewEpcViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            return ReviewEpc_Get();
        }

        var questionnaire = questionnaireService.UpdateEpcIsCorrect(viewModel.EpcIsCorrect == ReviewEpcViewModel.YesOrNo.Yes);

        var nextStep = questionFlowService.NextStep(QuestionFlowStep.ReviewEpc, questionnaire);
        return RedirectToNextStep(nextStep);
    }

    [HttpGet("address/manual")]
    public IActionResult ManualAddress_Get()
    {
        var questionnaire = questionnaireService.GetQuestionnaire();
        var viewModel = new ManualAddressViewModel()
        {
            AddressLine1 = questionnaire.AddressLine1,
            AddressLine2 = questionnaire.AddressLine2,
            Town = questionnaire.AddressTown,
            County = questionnaire.AddressCounty,
            Postcode = questionnaire.AddressPostcode,
            BackLink = GetBackUrl(QuestionFlowStep.SelectAddress, questionnaire)
        };

        return View("ManualAddress", viewModel);
    }

    [HttpPost("address/manual")]
    public async Task<IActionResult> ManualAddress_Post(ManualAddressViewModel viewModel)
    {
        if (viewModel.Postcode is not null && !viewModel.Postcode.IsValidUkPostcodeFormat())
        {
            ModelState.AddModelError(nameof(ManualAddressViewModel.Postcode), "Enter a valid UK postcode");
        }

        if (!ModelState.IsValid)
        {
            return ManualAddress_Get();
        }

        var address = new Address
        {
            AddressLine1 = viewModel.AddressLine1,
            AddressLine2 = viewModel.AddressLine2,
            County = viewModel.County,
            Town = viewModel.Town,
            Postcode = viewModel.Postcode
        };

        var questionnaire = await questionnaireService.UpdateAddressAsync(address);
        var nextStep = questionFlowService.NextStep(QuestionFlowStep.ManualAddress, questionnaire);
        return RedirectToNextStep(nextStep);
    }

    [HttpGet("income")]
    public IActionResult HouseholdIncome_Get()
    {
        return RedirectToAction(nameof(StaticPagesController.Index), "StaticPages");
    }

    [HttpPost("income")]
    public IActionResult HouseholdIncome_Post()
    {
        return RedirectToAction(nameof(StaticPagesController.Index), "StaticPages");
    }

    private string GetBackUrl(
        QuestionFlowStep currentStep,
        Questionnaire questionnaire = null,
        QuestionFlowStep? entryPoint = null)
    {
        var backStep = questionFlowService.PreviousStep(currentStep, questionnaire, entryPoint);
        var args = GetActionArgumentsForQuestion(backStep, entryPoint);
        return Url.Action(args.Action, args.Controller, args.Values);
    }

    private RedirectToActionResult RedirectToNextStep(QuestionFlowStep nextStep, QuestionFlowStep? entryPoint = null)
    {
        var forwardArgs = GetActionArgumentsForQuestion(nextStep, entryPoint);
        return RedirectToAction(forwardArgs.Action, forwardArgs.Controller, forwardArgs.Values);
    }

    private PathByActionArguments GetActionArgumentsForQuestion(
        QuestionFlowStep question,
        QuestionFlowStep? entryPoint = null,
        IDictionary<string, object> extraRouteValues = null)
    {
        return question switch
        {
            QuestionFlowStep.Start => new PathByActionArguments(nameof(Index), "Questionnaire"),
            QuestionFlowStep.GasBoiler => new PathByActionArguments(nameof(GasBoiler_Get), "Questionnaire", GetRouteValues(extraRouteValues)),
            QuestionFlowStep.DirectToEco => new PathByActionArguments(nameof(DirectToEco_Get), "Questionnaire"),
            QuestionFlowStep.Country => new PathByActionArguments(nameof(Country_Get), "Questionnaire", GetRouteValues(extraRouteValues)),
            QuestionFlowStep.ServiceUnsuitable => new PathByActionArguments(nameof(ServiceUnsuitable_Get), "Questionnaire", GetRouteValues(extraRouteValues)),
            QuestionFlowStep.OwnershipStatus => new PathByActionArguments(nameof(OwnershipStatus_Get), "Questionnaire", GetRouteValues(extraRouteValues)),
            QuestionFlowStep.Address => new PathByActionArguments(nameof(Address_Get), "Questionnaire", GetRouteValues(extraRouteValues)),
            QuestionFlowStep.SelectAddress => new PathByActionArguments(nameof(SelectAddress_Get), "Questionnaire", GetRouteValues(extraRouteValues)),
            QuestionFlowStep.ReviewEpc => new PathByActionArguments(nameof(ReviewEpc_Get), "Questionnaire"),
            QuestionFlowStep.ManualAddress => new PathByActionArguments(nameof(ManualAddress_Get), "Questionnaire", GetRouteValues(extraRouteValues)),
            QuestionFlowStep.HouseholdIncome => new PathByActionArguments(nameof(HouseholdIncome_Get), "Questionnaire", GetRouteValues(extraRouteValues)),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private RouteValueDictionary GetRouteValues(
        IDictionary<string, object> extraRouteValues,
        QuestionFlowStep? entryPoint = null)
    {
        // If entryPoint is null then it won't appear in the URL
        var ret = new RouteValueDictionary { { "entryPoint", entryPoint } };

        if (extraRouteValues != null)
        {
            foreach (var extraRouteValue in extraRouteValues)
            {
                ret[extraRouteValue.Key] = extraRouteValue.Value;
            }
        }

        return ret;
    }

    private class PathByActionArguments
    {
        public readonly string Action;
        public readonly string Controller;
        public readonly RouteValueDictionary Values;

        public PathByActionArguments(string action, string controller, RouteValueDictionary values = null)
        {
            Action = action;
            Controller = controller;
            Values = values;
        }
    }
}
