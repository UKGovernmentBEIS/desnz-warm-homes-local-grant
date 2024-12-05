using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using HerPublicWebsite.Models.ReferralRequestFollowUp;
using Microsoft.AspNetCore.Mvc;
using HerPublicWebsite.BusinessLogic.Services.ReferralFollowUps;
using HerPublicWebsite.Models.Enums;
using HerPublicWebsite.BusinessLogic.Models;

namespace HerPublicWebsite.Controllers;

[Route("referral-follow-up")]
public class ReferralRequestFollowUpController : Controller
{
    private readonly IReferralFollowUpService referralFollowUpService;

    public ReferralRequestFollowUpController(
        IReferralFollowUpService referralFollowUpService
    )
    {
        this.referralFollowUpService = referralFollowUpService;
    }

    [HttpGet("already-responded")]
    public IActionResult AlreadyResponded()
    {
         return View("AlreadyResponded");
    }

    [HttpGet("")]
    public async Task<IActionResult> RespondPage_Get([Required] string token)
    {
        ReferralRequestFollowUp referralRequestFollowUp = await referralFollowUpService.GetReferralRequestFollowUpByToken(token);
        if (referralRequestFollowUp.WasFollowedUp is not null) {
            return RedirectToAction(nameof(AlreadyResponded), "ReferralRequestFollowUp");
        } else {  
            var viewModel = new ReferralRequestFollowUpResponsePageViewModel
            {
                Token = token,
                ReferralCode = referralRequestFollowUp.ReferralRequest.ReferralCode, 
                RequestDate = referralRequestFollowUp.ReferralRequest.RequestDate
            };
            return View("ResponsePage", viewModel);
        }
    }

    [HttpPost("")]
    public async Task<IActionResult> RespondPage_Post(ReferralRequestFollowUpResponsePageViewModel viewModel)
    {
        await referralFollowUpService.RecordFollowUpResponseForToken(viewModel.Token, viewModel.HasFollowedUp is YesOrNo.Yes);
        return RedirectToAction(nameof(ResponseRecorded), "ReferralRequestFollowUp");
    }

    [HttpGet("response-recorded")]
    public IActionResult ResponseRecorded()
    {
         return View("ResponseRecorded");
    }
}
