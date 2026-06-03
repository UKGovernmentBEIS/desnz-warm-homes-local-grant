using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using WhlgPublicWebsite.BusinessLogic.Models;
using WhlgPublicWebsite.Models.Questionnaire;

namespace WhlgPublicWebsite.Controllers;

public class LaWordingPreviewViewModel
{
    public string LaName { get; set; }
    public LocalAuthorityData.LocalAuthorityStatus SelectedStatus { get; set; }
    public string EligiblePartialPath { get; set; }
    public string NotParticipatingPartialPath { get; set; }
    public string ReferralsPausedPartialPath { get; set; }
    public string NoLongerParticipatingPartialPath { get; set; }
    public string ConfirmationPartialPath { get; set; }
    public EligibleViewModel EligiblePartialModel { get; set; }
    public NotParticipatingViewModel NotParticipatingPartialModel { get; set; }
    public ReferralsPausedViewModel ReferralsPausedPartialModel { get; set; }
    public NoLongerParticipatingViewModel NoLongerParticipatingPartialModel { get; set; }
    public ConfirmationViewModel ConfirmationPartialModel { get; set; }
}

public class DevController : Controller
{
    private readonly IConfiguration configuration;

    public DevController(IConfiguration configuration)
    {
        this.configuration = configuration;
    }

    [HttpGet("/dev/la-wording-preview/page")]
    public IActionResult LaWordingPreviewPage(string custodianCode, LocalAuthorityData.LocalAuthorityStatus status, string page)
    {
        if (!configuration.GetValue<bool>("EnableDevTools"))
            return NotFound();

        if (!LocalAuthorityData.LocalAuthorityDetailsByCustodianCode.ContainsKey(custodianCode))
            return NotFound();

        var laName = LocalAuthorityData.LocalAuthorityDetailsByCustodianCode[custodianCode].Name;
        var laWebsite = LocalAuthorityData.LocalAuthorityDetailsByCustodianCode[custodianCode].WebsiteUrl;
        var questionnaire = new Questionnaire { CustodianCode = custodianCode };
        var isPending = status == LocalAuthorityData.LocalAuthorityStatus.Pending;
        var isTakingFutureReferrals = status == LocalAuthorityData.LocalAuthorityStatus.TakingFutureReferrals;
        var isLive = status == LocalAuthorityData.LocalAuthorityStatus.Live;
        var backLink = Url.Action(nameof(LaWordingPreview), new { custodianCode, status });

        return page switch
        {
            "Eligible" => View("~/Views/Questionnaire/Eligible.cshtml", new EligibleViewModel
            {
                LocalAuthorityName = laName,
                LocalAuthorityIsPending = isPending,
                LocalAuthorityIsTakingFutureReferrals = isTakingFutureReferrals,
                LocalAuthorityIsLive = isLive,
                LocalAuthorityMessagePartialViewPath = QuestionnaireController.GetLocalAuthorityEligibleMessagePartialViewPath(questionnaire),
                BackLink = backLink,
            }),
            "Confirmation" => View("~/Views/Questionnaire/Confirmation.cshtml", new ConfirmationViewModel
            {
                LocalAuthorityName = laName,
                LocalAuthorityWebsite = laWebsite,
                LocalAuthorityIsLive = isLive,
                LocalAuthorityIsInBroadland = LocalAuthorityData.CustodianCodeIsInConsortium(custodianCode, ConsortiumNames.BroadlandDistrictCouncil),
                LocalAuthorityMessagePartialViewPath = GetConfirmationPartialPath(custodianCode, status),
                ReferenceCode = "DEV-PREVIEW",
                RequestEmailAddress = true,
                BackLink = backLink,
            }),
            "NotParticipating" => View("~/Views/Questionnaire/NotParticipating.cshtml", new NotParticipatingViewModel
            {
                LocalAuthorityName = laName,
                LocalAuthorityMessagePartialViewPath = QuestionnaireController.GetLocalAuthorityNotParticipatingMessagePartialViewPath(questionnaire),
                BackLink = backLink,
            }),
            "ReferralsPaused" => View("~/Views/Questionnaire/ReferralsPaused.cshtml", new ReferralsPausedViewModel
            {
                LocalAuthorityName = laName,
                LocalAuthorityMessagePartialViewPath = QuestionnaireController.GetLocalAuthorityReferralsPausedMessagePartialViewPath(questionnaire),
                BackLink = backLink,
            }),
            "NoLongerParticipating" => View("~/Views/Questionnaire/NoLongerParticipating.cshtml", new NoLongerParticipatingViewModel
            {
                LocalAuthorityName = laName,
                LocalAuthorityMessagePartialViewPath = QuestionnaireController.GetLocalAuthorityNoLongerParticipatingMessagePartialViewPath(questionnaire),
                BackLink = backLink,
            }),
            "NoFunding" => View("~/Views/Questionnaire/NoFunding.cshtml", new NoFundingViewModel
            {
                LocalAuthorityName = laName,
                BackLink = backLink,
            }),
            "Pending" => View("~/Views/Questionnaire/Pending.cshtml", new PendingViewModel
            {
                LocalAuthorityName = laName,
                BackLink = backLink,
            }),
            "TakingFutureReferrals" => View("~/Views/Questionnaire/TakingFutureReferrals.cshtml", new TakingFutureReferralsViewModel
            {
                LocalAuthorityName = laName,
                BackLink = backLink,
            }),
            "Ineligible" => View("~/Views/Questionnaire/Ineligible.cshtml", new IneligibleViewModel
            {
                LocalAuthorityName = laName,
                LocalAuthorityWebsite = laWebsite,
                BackLink = backLink,
            }),
            "NoConsent" => View("~/Views/Questionnaire/NoConsent.cshtml", new NoConsentViewModel
            {
                LocalAuthorityName = laName,
                LocalAuthorityWebsite = laWebsite,
                BackLink = backLink,
            }),
            "ConfirmLocalAuthority" => View("~/Views/Questionnaire/ConfirmLocalAuthority.cshtml", new ConfirmLocalAuthorityViewModel
            {
                LocalAuthorityName = laName,
                BackLink = backLink,
            }),
            _ => NotFound()
        };
    }

    [HttpGet("/dev/la-wording-preview")]
    public IActionResult LaWordingPreview(string custodianCode, LocalAuthorityData.LocalAuthorityStatus? status)
    {
        if (!configuration.GetValue<bool>("EnableDevTools"))
            return NotFound();

        var allLas = LocalAuthorityData.LocalAuthorityDetailsByCustodianCode
            .OrderBy(e => e.Value.Name)
            .Select(e => (e.Key, e.Value.Name))
            .ToList();

        var statuses = System.Enum.GetValues<LocalAuthorityData.LocalAuthorityStatus>()
            .ToList();

        LaWordingPreviewViewModel viewModel = null;

        if (custodianCode != null && status != null &&
            LocalAuthorityData.LocalAuthorityDetailsByCustodianCode.ContainsKey(custodianCode))
        {
            var questionnaire = new Questionnaire { CustodianCode = custodianCode };
            var laName = LocalAuthorityData.LocalAuthorityDetailsByCustodianCode[custodianCode].Name;
            var laWebsite = LocalAuthorityData.LocalAuthorityDetailsByCustodianCode[custodianCode].WebsiteUrl;
            var isPending = status.Value == LocalAuthorityData.LocalAuthorityStatus.Pending;
            var isTakingFutureReferrals = status.Value == LocalAuthorityData.LocalAuthorityStatus.TakingFutureReferrals;
            var isLive = status.Value == LocalAuthorityData.LocalAuthorityStatus.Live;

            viewModel = new LaWordingPreviewViewModel
            {
                LaName = laName,
                SelectedStatus = status.Value,
                EligiblePartialPath = QuestionnaireController.GetLocalAuthorityEligibleMessagePartialViewPath(questionnaire),
                NotParticipatingPartialPath = QuestionnaireController.GetLocalAuthorityNotParticipatingMessagePartialViewPath(questionnaire),
                ReferralsPausedPartialPath = QuestionnaireController.GetLocalAuthorityReferralsPausedMessagePartialViewPath(questionnaire),
                NoLongerParticipatingPartialPath = QuestionnaireController.GetLocalAuthorityNoLongerParticipatingMessagePartialViewPath(questionnaire),
                ConfirmationPartialPath = GetConfirmationPartialPath(custodianCode, status.Value),
                EligiblePartialModel = new EligibleViewModel
                {
                    LocalAuthorityName = laName,
                    LocalAuthorityIsPending = isPending,
                    LocalAuthorityIsTakingFutureReferrals = isTakingFutureReferrals,
                    LocalAuthorityIsLive = isLive,
                },
                NotParticipatingPartialModel = new NotParticipatingViewModel { LocalAuthorityName = laName },
                ReferralsPausedPartialModel = new ReferralsPausedViewModel { LocalAuthorityName = laName },
                NoLongerParticipatingPartialModel = new NoLongerParticipatingViewModel { LocalAuthorityName = laName },
                ConfirmationPartialModel = new ConfirmationViewModel
                {
                    LocalAuthorityName = laName,
                    LocalAuthorityWebsite = laWebsite,
                    LocalAuthorityIsLive = isLive,
                },
            };
        }

        ViewBag.AllLas = allLas;
        ViewBag.Statuses = statuses;
        ViewBag.SelectedCode = custodianCode;
        ViewBag.SelectedStatus = status;
        return View("LaWordingPreview", viewModel);
    }

    private static string GetConfirmationPartialPath(string custodianCode, LocalAuthorityData.LocalAuthorityStatus status)
    {
        // Mirrors QuestionnaireController.GetLocalAuthorityConfirmationMessagePartialViewPath but accepts
        // status as a parameter rather than deriving it from LA data, so the dev tool can preview any status.
        var partialViewName = status switch
        {
            LocalAuthorityData.LocalAuthorityStatus.Pending => "Pending",
            LocalAuthorityData.LocalAuthorityStatus.TakingFutureReferrals => "TakingFutureReferrals",
            LocalAuthorityData.LocalAuthorityStatus.Live when LocalAuthorityData.CustodianCodeIsInConsortium(
                custodianCode, ConsortiumNames.WestMidlandsCombinedAuthority) => "WestMidlandsCombinedAuthority",
            LocalAuthorityData.LocalAuthorityStatus.Live when LocalAuthorityData.CustodianCodeIsInConsortium(
                custodianCode, ConsortiumNames.BroadlandDistrictCouncil) => "BroadlandDistrictCouncil",
            LocalAuthorityData.LocalAuthorityStatus.Live when LocalAuthorityData.CustodianCodeIsInConsortium(
                custodianCode, ConsortiumNames.PortsmouthCityCouncil) => "PortsmouthCityCouncil",
            LocalAuthorityData.LocalAuthorityStatus.Live when LocalAuthorityData.CustodianCodeIsInConsortium(
                custodianCode, ConsortiumNames.GreaterLondonAuthority) => "GreaterLondonAuthority",
            _ => "Default"
        };

        return $"~/Views/Partials/LocalAuthorityMessages/Confirmation/{partialViewName}.cshtml";
    }
}
