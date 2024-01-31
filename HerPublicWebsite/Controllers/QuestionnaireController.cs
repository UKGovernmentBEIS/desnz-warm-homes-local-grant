using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using HerPublicWebsite.BusinessLogic.ExternalServices.OsPlaces;
using HerPublicWebsite.BusinessLogic.Extensions;
using HerPublicWebsite.BusinessLogic.Models;
using HerPublicWebsite.BusinessLogic.Models.Enums;
using HerPublicWebsite.BusinessLogic.Services.QuestionFlow;
using HerPublicWebsite.Extensions;
using HerPublicWebsite.ExternalServices.GoogleAnalytics;
using HerPublicWebsite.Filters;
using HerPublicWebsite.Models.Enums;
using HerPublicWebsite.Models.Questionnaire;
using HerPublicWebsite.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;

namespace HerPublicWebsite.Controllers;

[Route("questionnaire")]
[SessionExpiry]
public class QuestionnaireController : Controller
{
    private readonly IQuestionFlowService questionFlowService;
    private readonly GoogleAnalyticsService googleAnalyticsService;
    private readonly QuestionnaireService questionnaireService;
    private readonly IOsPlacesApi osPlaces;
    private readonly ILogger logger;

    public QuestionnaireController(
        IQuestionFlowService questionFlowService,
        GoogleAnalyticsService googleAnalyticsService,
        QuestionnaireService questionnaireService,
        IOsPlacesApi osPlaces,
        ILogger<QuestionnaireController> logger
    )
    {
        this.questionFlowService = questionFlowService;
        this.googleAnalyticsService = googleAnalyticsService;
        this.questionnaireService = questionnaireService;
        this.osPlaces = osPlaces;
        this.logger = logger;
    }

    [HttpGet("")]
    [ExcludeFromSessionExpiry]
    public IActionResult Index()
    {
        return RedirectToAction(nameof(GasBoiler_Get), "Questionnaire");
    }

    [HttpGet("boiler")]
    [ExcludeFromSessionExpiry]
    public async Task<IActionResult> GasBoiler_Get(QuestionFlowStep? entryPoint, bool triggerEvent = true)
    {
        var questionnaire = questionnaireService.GetQuestionnaire();

        // This metric isn't very reliable, but we can cut out false triggers from editing answers and from validation
        // failures.
        if (questionnaire.HasGasBoiler is null && triggerEvent)
        {
            await googleAnalyticsService.SendBoilerQuestionViewedEventAsync(Request);
        }
        
        var viewModel = new GasBoilerViewModel
        {
            HasGasBoiler = questionnaire.HasGasBoiler,
            BackLink = "https://www.gov.uk/apply-home-upgrade-grant"
        };

        return View("GasBoiler", viewModel);
    }

    [HttpPost("boiler")]
    [ExcludeFromSessionExpiry]
    public async Task<IActionResult> GasBoiler_Post(GasBoilerViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            return await GasBoiler_Get(viewModel.EntryPoint, false);
        }

        var questionnaire = questionnaireService.UpdateGasBoiler(viewModel.HasGasBoiler!.Value, viewModel.EntryPoint);
        var nextStep = questionFlowService.NextStep(QuestionFlowStep.GasBoiler, questionnaire, viewModel.EntryPoint);

        return RedirectToNextStep(nextStep, viewModel.EntryPoint);
    }

    [HttpGet("direct-to-eco/")]
    public IActionResult DirectToEco_Get(QuestionFlowStep? entryPoint)
    {
        var viewModel = new DirectToEcoViewModel
        {
            BackLink = GetBackUrl(QuestionFlowStep.DirectToEco, entryPoint: entryPoint)
        };

        return View("DirectToEco", viewModel);
    }

    [HttpGet("country/")]
    public IActionResult Country_Get(QuestionFlowStep? entryPoint)
    {
        var questionnaire = questionnaireService.GetQuestionnaire();
        var viewModel = new CountryViewModel
        {
            Country = questionnaire.Country,
            BackLink = GetBackUrl(QuestionFlowStep.Country, questionnaire, entryPoint)
        };

        return View("Country", viewModel);
    }

    [HttpPost("country/")]
    public IActionResult Country_Post(CountryViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            return Country_Get(viewModel.EntryPoint);
        }

        var questionnaire = questionnaireService.UpdateCountry(viewModel.Country!.Value, viewModel.EntryPoint);
        var nextStep = questionFlowService.NextStep(QuestionFlowStep.Country, questionnaire, viewModel.EntryPoint);

        return RedirectToNextStep(nextStep, viewModel.EntryPoint);
    }

    [HttpGet("ineligible-wales/")]
    public IActionResult IneligibleWales_Get(QuestionFlowStep? entryPoint)
    {
        var questionnaire = questionnaireService.GetQuestionnaire();

        var viewModel = new IneligibleWalesViewModel()
        {
            BackLink = GetBackUrl(QuestionFlowStep.IneligibleWales, questionnaire, entryPoint)
        };

        return View("IneligibleWales", viewModel);
    }

    [HttpGet("ineligible-scotland/")]
    public IActionResult IneligibleScotland_Get(QuestionFlowStep? entryPoint)
    {
        var questionnaire = questionnaireService.GetQuestionnaire();

        var viewModel = new IneligibleScotlandViewModel()
        {
            BackLink = GetBackUrl(QuestionFlowStep.IneligibleScotland, questionnaire, entryPoint)
        };

        return View("IneligibleScotland", viewModel);
    }

    [HttpGet("ineligible-northern-ireland/")]
    public IActionResult IneligibleNorthernIreland_Get(QuestionFlowStep? entryPoint)
    {
        var questionnaire = questionnaireService.GetQuestionnaire();

        var viewModel = new IneligibleNorthernIrelandViewModel()
        {
            BackLink = GetBackUrl(QuestionFlowStep.IneligibleNorthernIreland, questionnaire, entryPoint)
        };

        return View("IneligibleNorthernIreland", viewModel);
    }

    [HttpGet("ownership-status/")]
    public IActionResult OwnershipStatus_Get(QuestionFlowStep? entryPoint)
    {
        var questionnaire = questionnaireService.GetQuestionnaire();

        var viewModel = new OwnershipStatusViewModel()
        {
            OwnershipStatus = questionnaire.OwnershipStatus,
            BackLink = GetBackUrl(QuestionFlowStep.OwnershipStatus, questionnaire, entryPoint)
        };

        return View("OwnershipStatus", viewModel);
    }

    [HttpPost("ownership-status/")]
    public IActionResult OwnershipStatus_Post(OwnershipStatusViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            return OwnershipStatus_Get(viewModel.EntryPoint);
        }

        var questionnaire = questionnaireService.UpdateOwnershipStatus(viewModel.OwnershipStatus!.Value, viewModel.EntryPoint);
        var nextStep = questionFlowService.NextStep(QuestionFlowStep.OwnershipStatus, questionnaire, viewModel.EntryPoint);

        return RedirectToNextStep(nextStep, viewModel.EntryPoint);
    }

    [HttpGet("ineligible-tenure/")]
    public IActionResult IneligibleTenure_Get(QuestionFlowStep? entryPoint)
    {
        var questionnaire = questionnaireService.GetQuestionnaire();

        var viewModel = new OwnershipStatusViewModel
        {
            BackLink = GetBackUrl(QuestionFlowStep.IneligibleTenure, questionnaire, entryPoint)
        };

        return View("IneligibleTenure", viewModel);
    }

    [HttpGet("address/")]
    public IActionResult Address_Get(QuestionFlowStep? entryPoint)
    {
        var questionnaire = questionnaireService.GetQuestionnaire();

        var viewModel = new AddressViewModel()
        {
            BackLink = GetBackUrl(QuestionFlowStep.Address, questionnaire, entryPoint)
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
            return Address_Get(viewModel.EntryPoint);
        }
        var questionnaire = questionnaireService.GetQuestionnaire();
        var nextStep = questionFlowService.NextStep(QuestionFlowStep.Address, questionnaire, viewModel.EntryPoint);
        var forwardArgs = GetActionArgumentsForQuestion(
            nextStep,
            viewModel.EntryPoint,
            extraRouteValues: new Dictionary<string, object>
            {
                { "postcode", viewModel.Postcode.NormaliseToUkPostcodeFormat() },
                { "buildingNameOrNumber", viewModel.BuildingNameOrNumber }
            }
        );

        return RedirectToAction(forwardArgs.Action, forwardArgs.Controller, forwardArgs.Values);
    }

    [HttpGet("address/{postcode}/{buildingNameOrNumber?}")]
    public async Task<IActionResult> SelectAddress_Get(string postcode, string buildingNameOrNumber, QuestionFlowStep? entryPoint)
    {
        var questionnaire = questionnaireService.GetQuestionnaire();

        var addresses = await osPlaces.GetAddressesAsync(postcode, buildingNameOrNumber);
        var viewModel = new SelectAddressViewModel()
        {
            Addresses = addresses,
            BackLink = GetBackUrl(QuestionFlowStep.SelectAddress, questionnaire, entryPoint),
            EntryPoint = entryPoint,
            IsSingleAddress = addresses.Count == 1,
            IsMultipleAddresses = addresses.Count > 1
        };

        TempData["Addresses"] = JsonSerializer.Serialize(viewModel.Addresses);

        return View("SelectAddress", viewModel);
    }

    [HttpPost("address/{postcode}/{buildingNameOrNumber?}")]
    public async Task<IActionResult> SelectAddress_Post(SelectAddressViewModel viewModel, string postcode, string buildingNameOrNumber)
    {
        if (!ModelState.IsValid)
        {
            return await SelectAddress_Get(postcode, buildingNameOrNumber, viewModel.EntryPoint);
        }

        if (viewModel.IsSingleAddress && viewModel.IsAddressCorrect is YesOrNo.No) {
            var forwardArgs = GetActionArgumentsForQuestion(QuestionFlowStep.ManualAddress, viewModel.EntryPoint);
            return RedirectToAction(forwardArgs.Action, forwardArgs.Controller, forwardArgs.Values);
        }

        try
        {
            var addressResults = JsonSerializer.Deserialize<List<Address>>(TempData["Addresses"] as string ?? throw new InvalidOperationException());
            var selectedAddress = addressResults[viewModel.IsSingleAddress ? 0 : Convert.ToInt32(viewModel.SelectedAddressIndex)]; // If there's only one address, just use the first item
            var questionnaire = await questionnaireService.UpdateAddressAsync(selectedAddress, viewModel.EntryPoint);

            var nextStep = questionFlowService.NextStep(QuestionFlowStep.SelectAddress, questionnaire, viewModel.EntryPoint);
            return RedirectToNextStep(nextStep, viewModel.EntryPoint);
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
    public IActionResult ReviewEpc_Get(QuestionFlowStep? entryPoint)
    {
        var questionnaire = questionnaireService.GetQuestionnaire();
        var viewModel = new ReviewEpcViewModel(questionnaire.EpcDetails, questionnaire.EpcDetailsAreCorrect)
        {
            BackLink = GetBackUrl(QuestionFlowStep.ReviewEpc, questionnaire, entryPoint)
        };

        return View("ReviewEpc", viewModel);
    }

    [HttpPost("review-epc")]
    public IActionResult ReviewEpc_Post(ReviewEpcViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            return ReviewEpc_Get(viewModel.EntryPoint);
        }

        var questionnaire = questionnaireService.UpdateEpcIsCorrect(viewModel.EpcIsCorrect, viewModel.EntryPoint);

        var nextStep = questionFlowService.NextStep(QuestionFlowStep.ReviewEpc, questionnaire, viewModel.EntryPoint);
        return RedirectToNextStep(nextStep, viewModel.EntryPoint);
    }

    [HttpGet("address/manual")]
    public IActionResult ManualAddress_Get(QuestionFlowStep? entryPoint)
    {
        var questionnaire = questionnaireService.GetQuestionnaire();
        var viewModel = new ManualAddressViewModel()
        {
            AddressLine1 = questionnaire.AddressLine1,
            AddressLine2 = questionnaire.AddressLine2,
            Town = questionnaire.AddressTown,
            County = questionnaire.AddressCounty,
            Postcode = questionnaire.AddressPostcode,
            BackLink = GetBackUrl(QuestionFlowStep.ManualAddress, questionnaire, entryPoint)
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
            return ManualAddress_Get(viewModel.EntryPoint);
        }

        var address = new Address
        {
            AddressLine1 = viewModel.AddressLine1,
            AddressLine2 = viewModel.AddressLine2,
            County = viewModel.County,
            Town = viewModel.Town,
            Postcode = viewModel.Postcode
        };

        var questionnaire = await questionnaireService.UpdateAddressAsync(address, viewModel.EntryPoint);
        var nextStep = questionFlowService.NextStep(QuestionFlowStep.ManualAddress, questionnaire, viewModel.EntryPoint);
        return RedirectToNextStep(nextStep, viewModel.EntryPoint);
    }

    [HttpGet("select-local-authority")]
    public IActionResult SelectLocalAuthority_Get
    (
        [FromQuery] SelectLocalAuthorityViewModel viewModel
    ) {
        var questionnaire = questionnaireService.GetQuestionnaire();

        viewModel.BackLink = GetBackUrl(QuestionFlowStep.SelectLocalAuthority, questionnaire, viewModel.EntryPoint);

        return View("SelectLocalAuthority", viewModel);
    }

    [HttpGet("select-local-authority/{custodianCode}")]
    public IActionResult LocalAuthoritySelected_Get(string custodianCode, QuestionFlowStep? entryPoint)
    {
        if (!LocalAuthorityData.LocalAuthorityDetailsByCustodianCode.ContainsKey(custodianCode))
        {
            // This should only happen if someone messes with the URL
            logger.LogError("Unrecognised custodian code received: " + custodianCode);
            return SelectLocalAuthority_Get(new SelectLocalAuthorityViewModel { EntryPoint = entryPoint });
        }

        var questionnaire = questionnaireService.UpdateLocalAuthority(custodianCode, entryPoint);

        var nextStep = questionFlowService.NextStep(QuestionFlowStep.SelectLocalAuthority, questionnaire, entryPoint);
        return RedirectToNextStep(nextStep, entryPoint);
    }

    [HttpGet("confirm-local-authority")]
    public IActionResult ConfirmLocalAuthority_Get(QuestionFlowStep? entryPoint)
    {
        var questionnaire = questionnaireService.GetQuestionnaire();
        var viewModel = new ConfirmLocalAuthorityViewModel()
        {
            LocalAuthorityName = LocalAuthorityData.LocalAuthorityDetailsByCustodianCode[questionnaire.CustodianCode].Name,
            LaIsCorrect = questionnaire.LocalAuthorityConfirmed.ToNullableYesOrNo(),
            EntryPoint = entryPoint,
            BackLink = GetBackUrl(QuestionFlowStep.ConfirmLocalAuthority, questionnaire, entryPoint)
        };

        return View("ConfirmLocalAuthority", viewModel);
    }

    [HttpPost("confirm-local-authority")]
    public IActionResult ConfirmLocalAuthority_Post(ConfirmLocalAuthorityViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            return ConfirmLocalAuthority_Get(viewModel.EntryPoint);
        }

        var questionnaire = questionnaireService.UpdateLocalAuthorityIsCorrect(viewModel.LaIsCorrect == YesOrNo.Yes, viewModel.EntryPoint);

        var nextStep = questionFlowService.NextStep(QuestionFlowStep.ConfirmLocalAuthority, questionnaire, viewModel.EntryPoint);
        return RedirectToNextStep(nextStep, viewModel.EntryPoint);
    }

    [HttpGet("not-taking-part")]
    public IActionResult NotTakingPart_Get(QuestionFlowStep? entryPoint, bool emailPreferenceSubmitted = false)
    {
        var questionnaire = questionnaireService.GetQuestionnaire();
        var viewModel = new NotTakingPartViewModel()
        {
            LocalAuthorityName = questionnaire.LocalAuthorityName,
            Submitted = emailPreferenceSubmitted,
            EmailAddress = questionnaire.NotificationEmailAddress,
            CanContactByEmailAboutFutureSchemes = questionnaire.NotificationConsent.ToNullableYesOrNo(),
            EntryPoint = entryPoint,
            BackLink = GetBackUrl(QuestionFlowStep.NotTakingPart, questionnaire, entryPoint)
        };

        return View("NotTakingPart", viewModel);
    }

    [HttpPost("not-taking-part")]
    public async Task<IActionResult> NotTakingPart_Post(IneligibleViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            return NotTakingPart_Get(viewModel.EntryPoint, false);
        }

        var questionnaire = await questionnaireService.RecordNotificationConsentAsync(
                viewModel.CanContactByEmailAboutFutureSchemes is YesOrNo.Yes,
                viewModel.EmailAddress
                );

        var nextStep = questionFlowService.NextStep(QuestionFlowStep.NotTakingPart, questionnaire, viewModel.EntryPoint);
        var forwardArgs = GetActionArgumentsForQuestion(
                            nextStep,
                            viewModel.EntryPoint,
                            extraRouteValues: new Dictionary<string, object>
                            {
                                { "emailPreferenceSubmitted", true }
                            }
                        );
        return RedirectToAction(forwardArgs.Action, forwardArgs.Controller, forwardArgs.Values);
    }
    
    [HttpGet("pending")]
    public IActionResult Pending_Get(QuestionFlowStep? entryPoint, bool emailPreferenceSubmitted = false)
    {
        var questionnaire = questionnaireService.GetQuestionnaire();
        var viewModel = new PendingViewModel()
        {
            LocalAuthorityName = questionnaire.LocalAuthorityName,
            LocalAuthorityMessagePartialViewPath = GetLocalAuthorityPendingMessagePartialViewPath(questionnaire),
            Submitted = emailPreferenceSubmitted,
            EmailAddress = questionnaire.NotificationEmailAddress,
            CanContactByEmailAboutFutureSchemes = questionnaire.NotificationConsent.ToNullableYesOrNo(),
            EntryPoint = entryPoint,
            BackLink = GetBackUrl(QuestionFlowStep.Pending, questionnaire, entryPoint)
        };

        return View("Pending", viewModel);
    }

    [HttpPost("pending")]
    public async Task<IActionResult> Pending_Post(IneligibleViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            return Pending_Get(viewModel.EntryPoint, false);
        }

        var questionnaire = await questionnaireService.RecordNotificationConsentAsync(
            viewModel.CanContactByEmailAboutFutureSchemes is YesOrNo.Yes,
            viewModel.EmailAddress
        );

        var nextStep = questionFlowService.NextStep(QuestionFlowStep.Pending, questionnaire, viewModel.EntryPoint);
        var forwardArgs = GetActionArgumentsForQuestion(
            nextStep,
            viewModel.EntryPoint,
            extraRouteValues: new Dictionary<string, object>
            {
                { "emailPreferenceSubmitted", true }
            }
        );
        return RedirectToAction(forwardArgs.Action, forwardArgs.Controller, forwardArgs.Values);
    }

    [HttpGet("income")]
    public IActionResult HouseholdIncome_Get(QuestionFlowStep? entryPoint)
    {
        var questionnaire = questionnaireService.GetQuestionnaire();
        var viewModel = new HouseholdIncomeViewModel
        {
            CustodianCode = questionnaire.CustodianCode,
            IncomeBand = questionnaire.IncomeBand,
            BackLink = GetBackUrl(QuestionFlowStep.HouseholdIncome, questionnaire, entryPoint)
        };

        return View("HouseholdIncome", viewModel);
    }

    [HttpPost("income")]
    public IActionResult HouseholdIncome_Post(HouseholdIncomeViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            return HouseholdIncome_Get(viewModel.EntryPoint);
        }

        var questionnaire = questionnaireService.UpdateHouseholdIncome(viewModel.IncomeBand!.Value, viewModel.EntryPoint);
        var nextStep = questionFlowService.NextStep(QuestionFlowStep.HouseholdIncome, questionnaire, viewModel.EntryPoint);

        return RedirectToNextStep(nextStep, viewModel.EntryPoint);

    }

    [HttpGet("check")]
    public IActionResult CheckAnswers_Get()
    {
        var questionnaire = questionnaireService.GetQuestionnaire();

        // If the user comes back to this page and their changes haven't been saved,
        // we need to revert to their old answers.
        if (questionnaire.UneditedData is not null)
        {
            questionnaire.RevertToUneditedData();
            questionnaireService.SaveQuestionnaireToSession(questionnaire);
        }
        
        var viewModel = new CheckAnswersViewModel()
        {
            Questionnaire = questionnaire,
            BackLink = GetBackUrl(QuestionFlowStep.CheckAnswers, questionnaire)
        };

        return View("CheckAnswers", viewModel);
    }

    [HttpPost("check")]
    public async Task<IActionResult> CheckAnswers_Post()
    {
        await googleAnalyticsService.SendQuestionnaireCompletedEventAsync(Request);
        var questionnaire = await questionnaireService.ConfirmQuestionnaireAnswers();
        var nextStep = questionFlowService.NextStep(QuestionFlowStep.CheckAnswers, questionnaire);

        return RedirectToNextStep(nextStep);
    }

    [HttpGet("eligible")]
    public IActionResult Eligible_Get()
    {
        var questionnaire = questionnaireService.GetQuestionnaire();
        var viewModel = new EligibleViewModel()
        {
            LocalAuthorityName = questionnaire.LocalAuthorityName,
            LocalAuthorityIsLiveWithHug2 = questionnaire.LocalAuthorityHug2Status is LocalAuthorityData.Hug2Status.Live,
            CanContactByEmail = questionnaire.LaCanContactByEmail.ToNullableYesOrNo(),
            CanContactByPhone = questionnaire.LaCanContactByPhone.ToNullableYesOrNo(),
            Name = questionnaire.LaContactName,
            EmailAddress = questionnaire.LaContactEmailAddress,
            Telephone = questionnaire.LaContactTelephone,
            BackLink = GetBackUrl(QuestionFlowStep.Eligible, questionnaire)
        };

        return View("Eligible", viewModel);
    }

    [HttpPost("eligible")]
    public async Task<IActionResult> Eligible_Post(EligibleViewModel viewModel)
    {
        if (viewModel.CanContactByEmail is YesOrNo.No && viewModel.CanContactByPhone is YesOrNo.No)
        {
            ModelState.AddModelError(string.Empty, "Select at least one method to be contacted by");
        }

        if (!ModelState.IsValid)
        {
            return Eligible_Get();
        }

        var questionnaire = await questionnaireService.GenerateReferralAsync(
            viewModel.Name,
            viewModel.CanContactByEmail is YesOrNo.Yes ? viewModel.EmailAddress : null,
            viewModel.CanContactByPhone is YesOrNo.Yes ? viewModel.Telephone : null);

        await googleAnalyticsService.SendReferralGeneratedEventAsync(Request);

        var nextStep = questionFlowService.NextStep(QuestionFlowStep.Eligible, questionnaire, viewModel.EntryPoint);

        return RedirectToNextStep(nextStep, viewModel.EntryPoint);
    }

    [HttpGet("confirmation")]
    public IActionResult Confirmation_Get(bool emailPreferenceSubmitted)
    {
        var questionnaire = questionnaireService.GetQuestionnaire();
        var viewModel = new ConfirmationViewModel()
        {
            ReferenceCode = questionnaire.ReferralCode,
            LocalAuthorityName = questionnaire.LocalAuthorityName,
            LocalAuthorityMessagePartialViewPath = GetLocalAuthorityConfirmationMessagePartialViewPath(questionnaire),
            LocalAuthorityWebsite = questionnaire.LocalAuthorityWebsite,
            LocalAuthorityIsLiveWithHug2 = questionnaire.LocalAuthorityHug2Status is LocalAuthorityData.Hug2Status.Live,
            ConfirmationSentToEmailAddress = questionnaire.LaContactEmailAddress ?? questionnaire.ConfirmationEmailAddress,
            RequestEmailAddress = questionnaire.LaCanContactByEmail is not true,
            CanNotifyAboutFutureSchemes = questionnaire.NotificationConsent.ToNullableYesOrNo(),
            SendConfirmationDetails = questionnaire.ConfirmationConsent.ToNullableYesOrNo(),
            ConfirmationEmailAddress = questionnaire.ConfirmationEmailAddress,
            NotificationEmailAddress = questionnaire.NotificationEmailAddress,
            EmailPreferenceSubmitted = emailPreferenceSubmitted,
            BackLink = GetBackUrl(QuestionFlowStep.Confirmation, questionnaire)
        };

        return View("Confirmation", viewModel);
    }

    [HttpPost("confirmation")]
    public async Task<IActionResult> Confirmation_Post(ConfirmationViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            return Confirmation_Get(viewModel.EmailPreferenceSubmitted);
        }

        var questionnaire = questionnaireService.GetQuestionnaire();

        if (questionnaire.LaCanContactByEmail is true)
        {
            questionnaire = await questionnaireService.RecordNotificationConsentAsync(
                viewModel.CanNotifyAboutFutureSchemes is YesOrNo.Yes);
        }
        else
        {
            questionnaire = await questionnaireService.RecordConfirmationAndNotificationConsentAsync(
                viewModel.CanNotifyAboutFutureSchemes is YesOrNo.Yes,
                viewModel.NotificationEmailAddress,
                viewModel.SendConfirmationDetails is YesOrNo.Yes,
                viewModel.ConfirmationEmailAddress);
        }

        var nextStep = questionFlowService.NextStep(QuestionFlowStep.Confirmation, questionnaire, viewModel.EntryPoint);
        var forwardArgs = GetActionArgumentsForQuestion(
            nextStep,
            viewModel.EntryPoint,
            extraRouteValues: new Dictionary<string, object>
            {
                { "emailPreferenceSubmitted", true }
            }
        );

        return RedirectToAction(forwardArgs.Action, forwardArgs.Controller, forwardArgs.Values);
    }

    [HttpGet("ineligible")]
    public IActionResult Ineligible_Get(bool emailPreferenceSubmitted = false)
    {
        var questionnaire = questionnaireService.GetQuestionnaire();
        var viewModel = new IneligibleViewModel
        {
            EpcIsTooHigh = questionnaire.EpcIsTooHigh,
            IncomeIsTooHigh = questionnaire.IncomeIsTooHigh,
            LocalAuthorityName = questionnaire.LocalAuthorityName,
            LocalAuthorityWebsite = questionnaire.LocalAuthorityWebsite,
            EmailAddress = questionnaire.NotificationEmailAddress,
            CanContactByEmailAboutFutureSchemes = questionnaire.NotificationConsent.ToNullableYesOrNo(),
            Submitted = emailPreferenceSubmitted,
            BackLink = GetBackUrl(QuestionFlowStep.Ineligible, questionnaire)
        };

        return View("Ineligible", viewModel);
    }

    [HttpPost("ineligible")]
    public async Task<IActionResult> Ineligible_Post(IneligibleViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            return Ineligible_Get(false);
        }

        var questionnaire = await questionnaireService.RecordNotificationConsentAsync(
                viewModel.CanContactByEmailAboutFutureSchemes is YesOrNo.Yes,
                viewModel.EmailAddress
                );

        var nextStep = questionFlowService.NextStep(QuestionFlowStep.Ineligible, questionnaire, viewModel.EntryPoint);
        var forwardArgs = GetActionArgumentsForQuestion(
                            nextStep,
                            viewModel.EntryPoint,
                            extraRouteValues: new Dictionary<string, object>
                            {
                                { "emailPreferenceSubmitted", true }
                            }
                        );
        return RedirectToAction(forwardArgs.Action, forwardArgs.Controller, forwardArgs.Values);
    }

    [HttpGet("no-consent")]
    public IActionResult NoConsent_Get()
    {
        var questionnaire = questionnaireService.GetQuestionnaire();
        var viewModel = new NoConsentViewModel()
        {
            LocalAuthorityName = questionnaire.LocalAuthorityName,
            LocalAuthorityWebsite = questionnaire.LocalAuthorityWebsite,
            BackLink = GetBackUrl(QuestionFlowStep.NoConsent, questionnaire)
        };
        return View("NoConsent", viewModel);
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
            QuestionFlowStep.GasBoiler => new PathByActionArguments(nameof(GasBoiler_Get), "Questionnaire", GetRouteValues(extraRouteValues, entryPoint)),
            QuestionFlowStep.DirectToEco => new PathByActionArguments(nameof(DirectToEco_Get), "Questionnaire", GetRouteValues(extraRouteValues, entryPoint)),
            QuestionFlowStep.Country => new PathByActionArguments(nameof(Country_Get), "Questionnaire", GetRouteValues(extraRouteValues, entryPoint)),
            QuestionFlowStep.IneligibleWales => new PathByActionArguments(nameof(IneligibleWales_Get), "Questionnaire", GetRouteValues(extraRouteValues, entryPoint)),
            QuestionFlowStep.IneligibleScotland => new PathByActionArguments(nameof(IneligibleScotland_Get), "Questionnaire", GetRouteValues(extraRouteValues, entryPoint)),
            QuestionFlowStep.IneligibleNorthernIreland => new PathByActionArguments(nameof(IneligibleNorthernIreland_Get), "Questionnaire", GetRouteValues(extraRouteValues, entryPoint)),
            QuestionFlowStep.OwnershipStatus => new PathByActionArguments(nameof(OwnershipStatus_Get), "Questionnaire", GetRouteValues(extraRouteValues, entryPoint)),
            QuestionFlowStep.IneligibleTenure => new PathByActionArguments(nameof(IneligibleTenure_Get), "Questionnaire", GetRouteValues(extraRouteValues, entryPoint)),
            QuestionFlowStep.Address => new PathByActionArguments(nameof(Address_Get), "Questionnaire", GetRouteValues(extraRouteValues, entryPoint)),
            QuestionFlowStep.SelectAddress => new PathByActionArguments(nameof(SelectAddress_Get), "Questionnaire", GetRouteValues(extraRouteValues, entryPoint)),
            QuestionFlowStep.ReviewEpc => new PathByActionArguments(nameof(ReviewEpc_Get), "Questionnaire", GetRouteValues(extraRouteValues, entryPoint)),
            QuestionFlowStep.ManualAddress => new PathByActionArguments(nameof(ManualAddress_Get), "Questionnaire", GetRouteValues(extraRouteValues, entryPoint)),
            QuestionFlowStep.SelectLocalAuthority => new PathByActionArguments(nameof(SelectLocalAuthority_Get), "Questionnaire", GetRouteValues(extraRouteValues, entryPoint)),
            QuestionFlowStep.ConfirmLocalAuthority => new PathByActionArguments(nameof(ConfirmLocalAuthority_Get), "Questionnaire", GetRouteValues(extraRouteValues, entryPoint)),
            QuestionFlowStep.NotTakingPart => new PathByActionArguments(nameof(NotTakingPart_Get), "Questionnaire", GetRouteValues(extraRouteValues, entryPoint)),
            QuestionFlowStep.Pending => new PathByActionArguments(nameof(Pending_Get), "Questionnaire", GetRouteValues(extraRouteValues, entryPoint)),
            QuestionFlowStep.HouseholdIncome => new PathByActionArguments(nameof(HouseholdIncome_Get), "Questionnaire", GetRouteValues(extraRouteValues, entryPoint)),
            QuestionFlowStep.CheckAnswers => new PathByActionArguments(nameof(CheckAnswers_Get), "Questionnaire", GetRouteValues(extraRouteValues)),
            QuestionFlowStep.Ineligible => new PathByActionArguments(nameof(Ineligible_Get), "Questionnaire", GetRouteValues(extraRouteValues)),
            QuestionFlowStep.Eligible => new PathByActionArguments(nameof(Eligible_Get), "Questionnaire", GetRouteValues(extraRouteValues)),
            QuestionFlowStep.Confirmation => new PathByActionArguments(nameof(Confirmation_Get), "Questionnaire", GetRouteValues(extraRouteValues)),
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
    
    private static string GetLocalAuthorityPendingMessagePartialViewPath(Questionnaire questionnaire)
    {
        var partialViewName = questionnaire.CustodianCode switch
        {
            "2610" => "Broadland",
            "2605" => "Broadland",
            "2620" => "Broadland",
            "2625" => "Broadland",
            "2630" => "Broadland",
            "2635" => "Broadland",
            _ => "Default"
        };
        return $"~/Views/Partials/LocalAuthorityMessages/Pending/{partialViewName}.cshtml";
    }
    
    private static string GetLocalAuthorityConfirmationMessagePartialViewPath(Questionnaire questionnaire)
    {
        var partialViewName = questionnaire.CustodianCode switch
        {
            _ => "Default"
        };
        return $"~/Views/Partials/LocalAuthorityMessages/Confirmation/{partialViewName}.cshtml";
    }
}
