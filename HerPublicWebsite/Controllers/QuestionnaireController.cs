using HerPublicWebsite.BusinessLogic.ExternalServices.EpbEpc;
using HerPublicWebsite.BusinessLogic.Services;
using HerPublicWebsite.DataStores;
using HerPublicWebsite.ExternalServices.GoogleAnalytics;
using HerPublicWebsite.Services.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace HerPublicWebsite.Controllers;

[Route("questionnaire")]
public class QuestionnaireController : Controller
{
    private readonly PropertyDataStore propertyDataStore;
    private readonly IQuestionFlowService questionFlowService;
    private readonly IEpcApi epcApi;
    private readonly CookieService cookieService;
    private readonly GoogleAnalyticsService googleAnalyticsService;

    public QuestionnaireController(
        PropertyDataStore propertyDataStore,
        IQuestionFlowService questionFlowService, 
        IEpcApi epcApi,
        CookieService cookieService,
        GoogleAnalyticsService googleAnalyticsService)
    {
        this.propertyDataStore = propertyDataStore;
        this.questionFlowService = questionFlowService;
        this.epcApi = epcApi;
        this.cookieService = cookieService;
        this.googleAnalyticsService = googleAnalyticsService;
    }
    
    [HttpGet("")]
    public IActionResult Index()
    {
        return RedirectToAction(nameof(StaticPagesController.Index), "StaticPages");
    }
}