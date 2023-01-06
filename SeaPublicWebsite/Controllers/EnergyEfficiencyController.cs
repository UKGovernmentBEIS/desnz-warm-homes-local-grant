using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using SeaPublicWebsite.BusinessLogic.ExternalServices.Bre;
using SeaPublicWebsite.BusinessLogic.ExternalServices.EpbEpc;
using SeaPublicWebsite.BusinessLogic.Models;
using SeaPublicWebsite.BusinessLogic.Models.Enums;
using SeaPublicWebsite.BusinessLogic.Services;
using SeaPublicWebsite.DataStores;
using SeaPublicWebsite.ExternalServices.EmailSending;
using SeaPublicWebsite.ExternalServices.GoogleAnalytics;
using SeaPublicWebsite.ExternalServices.PostcodesIo;
using SeaPublicWebsite.Models.EnergyEfficiency;
using SeaPublicWebsite.Services;
using SeaPublicWebsite.Services.Cookies;

namespace SeaPublicWebsite.Controllers
{
    [Route("energy-efficiency")]
    public class EnergyEfficiencyController : Controller
    {
        private readonly PropertyDataStore propertyDataStore;
        private readonly IQuestionFlowService questionFlowService;
        private readonly IEpcApi epcApi;
        private readonly IEmailSender emailApi;
        private readonly RecommendationService recommendationService;
        private readonly CookieService cookieService;
        private readonly GoogleAnalyticsService googleAnalyticsService;
        private readonly PostcodesIoApi postcodesIoApi;
        private readonly AnswerService answerService;

        public EnergyEfficiencyController(
            PropertyDataStore propertyDataStore,
            IQuestionFlowService questionFlowService, 
            IEpcApi epcApi,
            IEmailSender emailApi,
            RecommendationService recommendationService,
            CookieService cookieService,
            GoogleAnalyticsService googleAnalyticsService,
            PostcodesIoApi postcodesIoApi,
            AnswerService answerService)
        {
            this.propertyDataStore = propertyDataStore;
            this.questionFlowService = questionFlowService;
            this.emailApi = emailApi;
            this.epcApi = epcApi;
            this.recommendationService = recommendationService;
            this.cookieService = cookieService;
            this.googleAnalyticsService = googleAnalyticsService;
            this.postcodesIoApi = postcodesIoApi;
            this.answerService = answerService;
        }
        
        [HttpGet("")]
        public IActionResult Index()
        {
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }
        
        
        [HttpGet("new-or-returning-user")]
        public IActionResult NewOrReturningUser_Get()
        {
            var viewModel = new NewOrReturningUserViewModel
            {
                BackLink = GetBackUrl(QuestionFlowStep.NewOrReturningUser)
            };
            return View("NewOrReturningUser", viewModel);
        }

        [HttpPost("new-or-returning-user")]
        public async Task<IActionResult> NewOrReturningUser_Post(NewOrReturningUserViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return NewOrReturningUser_Get();
            }

            if (viewModel.NewOrReturningUser is NewOrReturningUser.ReturningUser)
            {
                if (!await propertyDataStore.IsReferenceValidAsync(viewModel.Reference))
                {
                    ModelState.AddModelError(nameof(NewOrReturningUserViewModel.Reference), "Check you have typed the reference correctly. Reference must be 8 characters.");
                    return NewOrReturningUser_Get();
                }

                return await ReturningUser_Get(viewModel.Reference, fromMagicLink: false);
            }

            var propertyData = await propertyDataStore.CreateNewPropertyDataAsync();

            var nextStep = questionFlowService.NextStep(QuestionFlowStep.NewOrReturningUser, propertyData);
            return RedirectToNextStep(nextStep, propertyData.Reference);
        }

        
        [HttpGet("ownership-status/{reference}")]
        public async Task<IActionResult> OwnershipStatus_Get(string reference)
        {
            var propertyData = await propertyDataStore.LoadPropertyDataAsync(reference);

            var viewModel = new OwnershipStatusViewModel
            {
                OwnershipStatus = propertyData.OwnershipStatus,
                Reference = propertyData.Reference,
                BackLink = GetBackUrl(QuestionFlowStep.OwnershipStatus, propertyData)
            };

            return View("OwnershipStatus", viewModel);
        }

        [HttpPost("ownership-status/{reference}")]
        public async Task<IActionResult> OwnershipStatus_Post(OwnershipStatusViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return await OwnershipStatus_Get(viewModel.Reference);
            }
            
            var nextStep = await answerService.UpdateOwnershipStatus(viewModel.Reference, viewModel.OwnershipStatus);
            
            return RedirectToNextStep(nextStep, viewModel.Reference);
        }

        [HttpGet("country/{reference}")]
        public async Task<IActionResult> Country_Get(string reference)
        {
            var propertyData = await propertyDataStore.LoadPropertyDataAsync(reference);

            var viewModel = new CountryViewModel
            {
                Country = propertyData.Country,
                Reference = propertyData.Reference,
                BackLink = GetBackUrl(QuestionFlowStep.Country, propertyData)
            };

            return View("Country", viewModel);
        }

        [HttpPost("country/{reference}")]
        public async Task<IActionResult> Country_Post(CountryViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return await Country_Get(viewModel.Reference);
            }
            
            var nextStep = await answerService.UpdateCountry(viewModel.Reference, viewModel.Country);
            
            return RedirectToNextStep(nextStep, viewModel.Reference);
        }
        
        [HttpGet("service-unsuitable/{reference}")]
        public async Task<IActionResult> ServiceUnsuitable(string reference)
        {
            var propertyData = await propertyDataStore.LoadPropertyDataAsync(reference);
            var viewModel = new ServiceUnsuitableViewModel
            {
                Reference = propertyData.Reference,
                Country = propertyData.Country,
                BackLink = GetBackUrl(QuestionFlowStep.ServiceUnsuitable, propertyData)
            };
            
            return View("ServiceUnsuitable", viewModel);
        }

        [HttpGet("find-epc/{reference}")]
        public async Task<IActionResult> FindEpc_Get(string reference)
        {
            var propertyData = await propertyDataStore.LoadPropertyDataAsync(reference);

            var viewModel = new FindEpcViewModel
            {
                Reference = propertyData.Reference,
                FindEpc = propertyData.SearchForEpc,
                BackLink = GetBackUrl(QuestionFlowStep.FindEpc, propertyData)
            };
            
            return View("FindEpc", viewModel);
        }

        [HttpPost("find-epc/{reference}")]
        public async Task<IActionResult> FindEpc_Post(FindEpcViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return await FindEpc_Get(viewModel.Reference);
            }

            var nextStep = await answerService.UpdateSearchForEpc(viewModel.Reference, viewModel.FindEpc);
            
            return RedirectToNextStep(nextStep, viewModel.Reference);
        }

        [HttpGet("postcode/{reference}")]
        public async Task<IActionResult> AskForPostcode_Get(string reference)
        {
            var propertyData = await propertyDataStore.LoadPropertyDataAsync(reference);
            
            var viewModel = new AskForPostcodeViewModel
            {
                BackLink = GetBackUrl(QuestionFlowStep.AskForPostcode, propertyData),
                SkipLink = GetSkipUrl(QuestionFlowStep.AskForPostcode, propertyData)
            };

            return View("AskForPostcode", viewModel);
        }

        [HttpPost("postcode/{reference}")]
        public async Task<IActionResult> AskForPostcode_Post(AskForPostcodeViewModel viewModel, bool cancel = false)
        {
            var propertyData = await propertyDataStore.LoadPropertyDataAsync(viewModel.Reference);
            
            if (cancel)
            {
                // Behave as if the user had selected "No" on the page before
                var nextStepCancel = await answerService.UpdateSearchForEpc(viewModel.Reference, SearchForEpc.No);
                return RedirectToNextStep(nextStepCancel, viewModel.Reference);
            }
            
            if (viewModel.Postcode is not null && !(await postcodesIoApi.IsValidPostcode(viewModel.Postcode)))
            {
                ModelState.AddModelError(nameof(AskForPostcodeViewModel.Postcode), "Enter a valid UK postcode");
            }
            
            if (!ModelState.IsValid)
            {
                return await AskForPostcode_Get(viewModel.Reference);
            }

            var nextStep = questionFlowService.NextStep(QuestionFlowStep.AskForPostcode, propertyData);
            var forwardArgs = GetActionArgumentsForQuestion(
                nextStep,
                viewModel.Reference,
                extraRouteValues: new Dictionary<string, object> { { "postcode", viewModel.Postcode}, { "number", viewModel.HouseNameOrNumber } });

            return RedirectToAction(forwardArgs.Action, forwardArgs.Controller, forwardArgs.Values);
        }

        
        [HttpGet("address/{reference}/{postcode}/{number}")]
        public async Task<ViewResult> ConfirmAddress_Get(string reference, string postcode, string number)
        {
            List<EpcSearchResult> epcSearchResults = await epcApi.GetEpcsInformationForPostcodeAndBuildingNameOrNumber(postcode, number);
            
            var filteredEpcSearchResults = epcSearchResults.Where(e =>
                e.Address1.Contains(number, StringComparison.OrdinalIgnoreCase) ||
                e.Address2.Contains(number, StringComparison.OrdinalIgnoreCase)).ToList();

            epcSearchResults = filteredEpcSearchResults.Any() ? filteredEpcSearchResults : epcSearchResults;

            var propertyData = await propertyDataStore.LoadPropertyDataAsync(reference);
            var backLink = GetBackUrl(QuestionFlowStep.ConfirmAddress, propertyData);

            if (epcSearchResults.Count == 0)
            {
                var viewModel = new NoEpcFoundViewModel
                {
                    Reference = reference,
                    BackLink = backLink,
                    ForwardLink = GetForwardUrl(QuestionFlowStep.NoEpcFound, propertyData)
                };
                return View("NoEpcFound", viewModel);
            }
            else if (epcSearchResults.Count == 1)
            {
                var viewModel = new ConfirmSingleAddressViewModel
                {
                    Reference = reference,
                    BackLink = backLink,
                    EpcSearchResult = epcSearchResults[0],
                    Number = number,
                    Postcode = postcode,
                    EpcId = epcSearchResults[0].EpcId
                };
                return View("ConfirmSingleAddress", viewModel);
            }
            else
            {
                var viewModel = new ConfirmAddressViewModel
                {
                    Reference = reference,
                    EpcSearchResults = epcSearchResults,
                    Postcode = postcode,
                    Number = number,
                    BackLink = backLink
                };

                    return View("ConfirmAddress", viewModel);
            }
        }

        [HttpPost("address/{reference}/{postcode}/{number}")]
        public async Task<IActionResult> ConfirmAddress_Post(ConfirmAddressViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return await ConfirmAddress_Get(viewModel.Reference, viewModel.Postcode, viewModel.Number);
            }

            var epcId = viewModel.SelectedEpcId == "unlisted" ? null : viewModel.SelectedEpcId;
            var nextStep = await answerService.SetEpc(viewModel.Reference, epcId);
            
            return RedirectToNextStep(nextStep, viewModel.Reference);
        }
        
        [HttpPost("single-address/{reference}/{postcode}/{number}")]
        public async Task<IActionResult> ConfirmSingleAddress_Post(ConfirmSingleAddressViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return await ConfirmAddress_Get(viewModel.Reference, viewModel.Postcode, viewModel.Number);
            }
            
            var epcId = viewModel.EpcAddressConfirmed == EpcAddressConfirmed.Yes ? viewModel.EpcId : null;
            var nextStep = await answerService.SetEpc(viewModel.Reference, epcId);

            return RedirectToNextStep(nextStep, viewModel.Reference);
        }

        [HttpGet("confirm-epc-details/{reference}")]
        public async Task<IActionResult> ConfirmEpcDetails_Get(string reference)
        {
            var propertyData = await propertyDataStore.LoadPropertyDataAsync(reference);

            var viewModel = new ConfirmEpcDetailsViewModel
            {
                Reference = propertyData.Reference,
                Epc = propertyData.Epc,
                BackLink = GetBackUrl(QuestionFlowStep.ConfirmEpcDetails, propertyData)
            };
            
            return View("ConfirmEpcDetails", viewModel);
        }

        [HttpPost("confirm-epc-details/{reference}")]
        public async Task<IActionResult> ConfirmEpcDetails_Post(ConfirmEpcDetailsViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return await ConfirmEpcDetails_Get(viewModel.Reference);
            }
            
            var nextStep = await answerService.ConfirmEpcDetails(viewModel.Reference, viewModel.EpcDetailsConfirmed);
            
            return RedirectToNextStep(nextStep, viewModel.Reference);
        }

        [HttpGet("no-epc-found/{reference}")]
        public async Task<IActionResult> NoEpcFound_Get(string reference)
        {
            var propertyData = await propertyDataStore.LoadPropertyDataAsync(reference);

            var viewModel = new NoEpcFoundViewModel
            {
                Reference = propertyData.Reference,
                BackLink = GetBackUrl(QuestionFlowStep.NoEpcFound, propertyData),
                ForwardLink = GetForwardUrl(QuestionFlowStep.NoEpcFound, propertyData)
            };
            
            return View("NoEpcFound", viewModel);
        }

        [HttpGet("property-type/{reference}")]
        public async Task<IActionResult> PropertyType_Get(string reference, QuestionFlowStep? entryPoint = null)
        {
            var propertyData = await propertyDataStore.LoadPropertyDataAsync(reference);
            
            var viewModel = new PropertyTypeViewModel
            {
                PropertyType = propertyData.PropertyType,
                Reference = reference,
                EntryPoint = entryPoint,
                BackLink = GetBackUrl(QuestionFlowStep.PropertyType, propertyData, entryPoint)
            };

            return View("PropertyType", viewModel);
        }

        [HttpPost("property-type/{reference}")]
        public async Task<IActionResult> PropertyType_Post(PropertyTypeViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return await PropertyType_Get(viewModel.Reference, viewModel.EntryPoint);
            }
            
            var nextStep = await answerService.UpdatePropertyType(
                viewModel.Reference,
                viewModel.PropertyType,
                viewModel.EntryPoint);
            
            return RedirectToNextStep(nextStep, viewModel.Reference, viewModel.EntryPoint);
        }

        [HttpGet("house-type/{reference}")]
        public async Task<IActionResult> HouseType_Get(string reference, QuestionFlowStep? entryPoint = null)
        {
            var propertyData = await propertyDataStore.LoadPropertyDataAsync(reference);
            
            var viewModel = new HouseTypeViewModel
            {
                HouseType = propertyData.HouseType,
                Reference = propertyData.Reference,
                EntryPoint = entryPoint,
                BackLink = GetBackUrl(QuestionFlowStep.HouseType, propertyData, entryPoint)
            };

            return View("HouseType", viewModel);
        }

        [HttpPost("house-type/{reference}")]
        public async Task<IActionResult> HouseType_Post(HouseTypeViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return await HouseType_Get(viewModel.Reference, viewModel.EntryPoint);
            }
            
            var nextStep = await answerService.UpdateHouseType(
                viewModel.Reference,
                viewModel.HouseType,
                viewModel.EntryPoint);
            
            return RedirectToNextStep(nextStep, viewModel.Reference, viewModel.EntryPoint);
        }

        
        [HttpGet("bungalow-type/{reference}")]
        public async Task<IActionResult> BungalowType_Get(string reference, QuestionFlowStep? entryPoint = null)
        {
            var propertyData = await propertyDataStore.LoadPropertyDataAsync(reference);
            
            var viewModel = new BungalowTypeViewModel
            {
                BungalowType = propertyData.BungalowType,
                Reference = reference,
                EntryPoint = entryPoint,
                BackLink = GetBackUrl(QuestionFlowStep.BungalowType, propertyData, entryPoint)
            };

            return View("BungalowType", viewModel);
        }

        [HttpPost("bungalow-type/{reference}")]
        public async Task<IActionResult> BungalowType_Post(BungalowTypeViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return await BungalowType_Get(viewModel.Reference, viewModel.EntryPoint);
            }
            
            var nextStep = await answerService.UpdateBungalowType(
                viewModel.Reference,
                viewModel.BungalowType,
                viewModel.EntryPoint);
            
            return RedirectToNextStep(nextStep, viewModel.Reference, viewModel.EntryPoint);
        }

        
        [HttpGet("flat-type/{reference}")]
        public async Task<IActionResult> FlatType_Get(string reference, QuestionFlowStep? entryPoint = null)
        {
            var propertyData = await propertyDataStore.LoadPropertyDataAsync(reference);
            
            var viewModel = new FlatTypeViewModel
            {
                FlatType = propertyData.FlatType,
                Reference = propertyData.Reference,
                EntryPoint = entryPoint,
                BackLink = GetBackUrl(QuestionFlowStep.FlatType, propertyData, entryPoint)
            };

            return View("FlatType", viewModel);
        }

        [HttpPost("flat-type/{reference}")]
        public async Task<IActionResult> FlatType_Post(FlatTypeViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return await FlatType_Get(viewModel.Reference, viewModel.EntryPoint);
            }
            
            var nextStep = await answerService.UpdateFlatType(
                viewModel.Reference,
                viewModel.FlatType,
                viewModel.EntryPoint);
            
            return RedirectToNextStep(nextStep, viewModel.Reference, viewModel.EntryPoint);
        }

        
        [HttpGet("home-age/{reference}")]
        public async Task<IActionResult> HomeAge_Get(string reference, QuestionFlowStep? entryPoint = null)
        {
            var propertyData = await propertyDataStore.LoadPropertyDataAsync(reference);
            
            var viewModel = new HomeAgeViewModel
            {
                PropertyType = propertyData.PropertyType,
                YearBuilt = propertyData.YearBuilt,
                Reference = propertyData.Reference,
                Epc = propertyData.Epc,
                EntryPoint = entryPoint,
                BackLink = GetBackUrl(QuestionFlowStep.HomeAge, propertyData, entryPoint)
            };

            return View("HomeAge", viewModel);
        }

        [HttpPost("home-age/{reference}")]
        public async Task<IActionResult> HomeAge_Post(HomeAgeViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return await HomeAge_Get(viewModel.Reference, viewModel.EntryPoint);
            }
            
            var nextStep = await answerService.UpdateYearBuilt(
                viewModel.Reference,
                viewModel.YearBuilt,
                viewModel.EntryPoint);
            
            return RedirectToNextStep(nextStep, viewModel.Reference, viewModel.EntryPoint);
        }

        [HttpGet("check-your-unchangeable-answers/{reference}")]
        public async Task<IActionResult> CheckYourUnchangeableAnswers_Get(string reference, QuestionFlowStep? entryPoint = null)
        {
            var propertyData = await propertyDataStore.LoadPropertyDataAsync(reference);
            
            // If the user comes back to this page and their changes haven't been saved,
            // we need to revert to their old answers.
            if (propertyData.UneditedData is not null)
            {
                propertyData.RevertToUneditedData();
                await propertyDataStore.SavePropertyDataAsync(propertyData);
            }
            
            var viewModel = new CheckYourUnchangeableAnswersViewModel()
            {
                Reference = reference,
                PropertyData = propertyData,
                BackLink = GetBackUrl(QuestionFlowStep.CheckYourUnchangeableAnswers, propertyData, entryPoint),
                ForwardLink = GetForwardUrl(QuestionFlowStep.CheckYourUnchangeableAnswers, propertyData, entryPoint)
            };
            
            return View("CheckYourUnchangeableAnswers", viewModel);
        }

        [HttpGet("wall-construction/{reference}")]
        public async Task<IActionResult> WallConstruction_Get(string reference, QuestionFlowStep? entryPoint = null)
        {
            var propertyData = await propertyDataStore.LoadPropertyDataAsync(reference);
            
            var viewModel = new WallConstructionViewModel
            {
                WallConstruction = propertyData.WallConstruction,
                HintSolidWalls = propertyData.HintSolidWalls,
                Reference = propertyData.Reference,
                EntryPoint = entryPoint,
                Epc = propertyData.Epc,
                BackLink = GetBackUrl(QuestionFlowStep.WallConstruction, propertyData, entryPoint)
            };

            return View("WallConstruction", viewModel);
        }

        [HttpPost("wall-construction/{reference}")]
        public async Task<IActionResult> WallConstruction_Post(WallConstructionViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return await WallConstruction_Get(viewModel.Reference, viewModel.EntryPoint);
            }
            
            var nextStep = await answerService.UpdateWallConstruction(
                viewModel.Reference,
                viewModel.WallConstruction,
                viewModel.EntryPoint);
            
            return RedirectToNextStep(nextStep, viewModel.Reference, viewModel.EntryPoint);
        }

        
        [HttpGet("cavity-walls-insulated/{reference}")]
        public async Task<IActionResult> CavityWallsInsulated_Get(string reference, QuestionFlowStep? entryPoint = null)
        {
            var propertyData = await propertyDataStore.LoadPropertyDataAsync(reference);

            var viewModel = new CavityWallsInsulatedViewModel
            {
                CavityWallsInsulated = propertyData.CavityWallsInsulated,
                YearBuilt = propertyData.YearBuilt,
                HintUninsulatedCavityWalls = propertyData.HintUninsulatedCavityWalls,
                Reference = propertyData.Reference,
                EntryPoint = entryPoint,
                Epc = propertyData.Epc,
                BackLink = GetBackUrl(QuestionFlowStep.CavityWallsInsulated, propertyData, entryPoint)
            };

            return View("CavityWallsInsulated", viewModel);
        }

        [HttpPost("cavity-walls-insulated/{reference}")]
        public async Task<IActionResult> CavityWallsInsulated_Post(CavityWallsInsulatedViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return await CavityWallsInsulated_Get(viewModel.Reference, viewModel.EntryPoint);
            }
            
            var nextStep = await answerService.UpdateCavityWallInsulation(
                viewModel.Reference,
                viewModel.CavityWallsInsulated,
                viewModel.EntryPoint);
            
            return RedirectToNextStep(nextStep, viewModel.Reference, viewModel.EntryPoint);
        }


        [HttpGet("solid-walls-insulated/{reference}")]
        public async Task<IActionResult> SolidWallsInsulated_Get(string reference, QuestionFlowStep? entryPoint = null)
        {
            var propertyData = await propertyDataStore.LoadPropertyDataAsync(reference);

            var viewModel = new SolidWallsInsulatedViewModel
            {
                SolidWallsInsulated = propertyData.SolidWallsInsulated,
                Reference = propertyData.Reference,
                EntryPoint = entryPoint,
                Epc = propertyData.Epc,
                BackLink = GetBackUrl(QuestionFlowStep.SolidWallsInsulated, propertyData, entryPoint)
            };

            return View("SolidWallsInsulated", viewModel);
        }

        [HttpPost("solid-walls-insulated/{reference}")]
        public async Task<IActionResult> SolidWallsInsulated_Post(SolidWallsInsulatedViewModel viewModel)
        {            
            if (!ModelState.IsValid)
            {
                return await SolidWallsInsulated_Get(viewModel.Reference, viewModel.EntryPoint);
            }
            
            var nextStep = await answerService.UpdateSolidWallInsulation(
                viewModel.Reference,
                viewModel.SolidWallsInsulated,
                viewModel.EntryPoint);
            
            return RedirectToNextStep(nextStep, viewModel.Reference, viewModel.EntryPoint);
        }


        [HttpGet("floor-construction/{reference}")]
        public async Task<IActionResult> FloorConstruction_Get(string reference, QuestionFlowStep? entryPoint = null)
        {
            var propertyData = await propertyDataStore.LoadPropertyDataAsync(reference);
            
            var viewModel = new FloorConstructionViewModel
            {
                FloorConstruction = propertyData.FloorConstruction,
                HintSuspendedTimber = propertyData.HintSuspendedTimber,
                Reference = propertyData.Reference,
                EntryPoint = entryPoint,
                Epc = propertyData.Epc,
                BackLink = GetBackUrl(QuestionFlowStep.FloorConstruction, propertyData, entryPoint)
            };

            return View("FloorConstruction", viewModel);
        }

        [HttpPost("floor-construction/{reference}")]
        public async Task<IActionResult> FloorConstruction_Post(FloorConstructionViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return await FloorConstruction_Get(viewModel.Reference, viewModel.EntryPoint);
            }
            
            var nextStep = await answerService.UpdateFloorConstruction(
                viewModel.Reference,
                viewModel.FloorConstruction,
                viewModel.EntryPoint);
            
            return RedirectToNextStep(nextStep, viewModel.Reference, viewModel.EntryPoint);
        }
        
        [HttpGet("floor-insulated/{reference}")]
        public async Task<IActionResult> FloorInsulated_Get(string reference, QuestionFlowStep? entryPoint = null)
        {
            var propertyData = await propertyDataStore.LoadPropertyDataAsync(reference);
            
            var viewModel = new FloorInsulatedViewModel
            {
                FloorInsulated = propertyData.FloorInsulated,
                HintUninsulatedFloor = propertyData.HintUninsulatedFloor,
                Reference = propertyData.Reference,
                EntryPoint = entryPoint,
                Epc = propertyData.Epc,
                BackLink = GetBackUrl(QuestionFlowStep.FloorInsulated, propertyData, entryPoint)
            };

            return View("FloorInsulated", viewModel);
        }

        [HttpPost("floor-insulated/{reference}")]
        public async Task<IActionResult> FloorInsulated_Post(FloorInsulatedViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return await FloorInsulated_Get(viewModel.Reference, viewModel.EntryPoint);
            }
            
            var nextStep = await answerService.UpdateFloorInsulated(
                viewModel.Reference,
                viewModel.FloorInsulated,
                viewModel.EntryPoint);
            
            return RedirectToNextStep(nextStep, viewModel.Reference, viewModel.EntryPoint);
        }

        [HttpGet("roof-construction/{reference}")]
        public async Task<IActionResult> RoofConstruction_Get(string reference, QuestionFlowStep? entryPoint = null)
        {
            var propertyData = await propertyDataStore.LoadPropertyDataAsync(reference);
            
            var viewModel = new RoofConstructionViewModel
            {
                RoofConstruction = propertyData.RoofConstruction,
                Reference = propertyData.Reference,
                Epc = propertyData.Epc,
                EntryPoint = entryPoint,
                BackLink = GetBackUrl(QuestionFlowStep.RoofConstruction, propertyData, entryPoint)
            };

            return View("RoofConstruction", viewModel);
        }

        [HttpPost("roof-construction/{reference}")]
        public async Task<IActionResult> RoofConstruction_Post(RoofConstructionViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return await RoofConstruction_Get(viewModel.Reference, viewModel.EntryPoint);
            }
            
            var nextStep = await answerService.UpdateRoofConstruction(
                viewModel.Reference,
                viewModel.RoofConstruction,
                viewModel.EntryPoint);
            
            return RedirectToNextStep(nextStep, viewModel.Reference, viewModel.EntryPoint);
        }

        [HttpGet("loft-space/{reference}")]
        public async Task<IActionResult> LoftSpace_Get(string reference, QuestionFlowStep? entryPoint = null)
        {
            var propertyData = await propertyDataStore.LoadPropertyDataAsync(reference);

            var viewModel = new LoftSpaceViewModel
            {
                LoftSpace = propertyData.LoftSpace,
                HintHaveLoftAndAccess = propertyData.HintHaveLoftAndAccess,
                Reference = propertyData.Reference,
                EntryPoint = entryPoint,
                BackLink = GetBackUrl(QuestionFlowStep.LoftSpace, propertyData, entryPoint)
            };

            return View("LoftSpace", viewModel);
        }

        [HttpPost("loft-space/{reference}")]
        public async Task<IActionResult> LoftSpace_Post(LoftSpaceViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return await LoftSpace_Get(viewModel.Reference, viewModel.EntryPoint);
            }
            
            var nextStep = await answerService.UpdateLoftSpace(
                viewModel.Reference,
                viewModel.LoftSpace,
                viewModel.EntryPoint);
            
            return RedirectToNextStep(nextStep, viewModel.Reference, viewModel.EntryPoint);
        }

        [HttpGet("loft-access/{reference}")]
        public async Task<IActionResult> LoftAccess_Get(string reference, QuestionFlowStep? entryPoint = null)
        {
            var propertyData = await propertyDataStore.LoadPropertyDataAsync(reference);

            var viewModel = new LoftAccessViewModel
            {
                LoftAccess = propertyData.LoftAccess,
                HintHaveLoftAndAccess = propertyData.HintHaveLoftAndAccess,
                Reference = propertyData.Reference,
                EntryPoint = entryPoint,
                BackLink = GetBackUrl(QuestionFlowStep.LoftAccess, propertyData, entryPoint)
            };

            return View("LoftAccess", viewModel);
        }
        
        [HttpPost("loft-access/{reference}")]
        public async Task<IActionResult> LoftAccess_Post(LoftAccessViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return await LoftAccess_Get(viewModel.Reference, viewModel.EntryPoint);
            }

            var nextStep = await answerService.UpdateLoftAccess(
                viewModel.Reference,
                viewModel.LoftAccess,
                viewModel.EntryPoint);
            
            return RedirectToNextStep(nextStep, viewModel.Reference, viewModel.EntryPoint);
        }

        [HttpGet("roof-insulated/{reference}")]
        public async Task<IActionResult> RoofInsulated_Get(string reference, QuestionFlowStep? entryPoint = null)
        {
            var propertyData = await propertyDataStore.LoadPropertyDataAsync(reference);
            
            var viewModel = new RoofInsulatedViewModel
            {
                RoofInsulated = propertyData.RoofInsulated,
                HintUninsulatedRoof = propertyData.HintUninsulatedRoof,
                Reference = propertyData.Reference,
                EntryPoint = entryPoint,
                Epc = propertyData.Epc,
                BackLink = GetBackUrl(QuestionFlowStep.RoofInsulated, propertyData, entryPoint)
            };

            return View("RoofInsulated", viewModel);
        }

        [HttpPost("roof-insulated/{reference}")]
        public async Task<IActionResult> RoofInsulated_Post(RoofInsulatedViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return await RoofInsulated_Get(viewModel.Reference, viewModel.EntryPoint);
            }
            
            var nextStep = await answerService.UpdateRoofInsulated(
                viewModel.Reference,
                viewModel.RoofInsulated,
                viewModel.EntryPoint);

            return RedirectToNextStep(nextStep, viewModel.Reference, viewModel.EntryPoint);
        }

        [HttpGet("glazing-type/{reference}")]
        public async Task<IActionResult> GlazingType_Get(string reference, QuestionFlowStep? entryPoint = null)
        {
            var propertyData = await propertyDataStore.LoadPropertyDataAsync(reference);
            
            var viewModel = new GlazingTypeViewModel
            {
                GlazingType = propertyData.GlazingType,
                HintSingleGlazing = propertyData.HintSingleGlazing,
                Reference = propertyData.Reference,
                EntryPoint = entryPoint,
                Epc = propertyData.Epc,
                BackLink = GetBackUrl(QuestionFlowStep.GlazingType, propertyData, entryPoint)
            };

            return View("GlazingType", viewModel);
        }

        [HttpPost("glazing-type/{reference}")]
        public async Task<IActionResult> GlazingType_Post(GlazingTypeViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return await GlazingType_Get(viewModel.Reference, viewModel.EntryPoint);
            }
            
            var nextStep = await answerService.UpdateGlazingType(
                viewModel.Reference,
                viewModel.GlazingType,
                viewModel.EntryPoint);
            
            return RedirectToNextStep(nextStep, viewModel.Reference, viewModel.EntryPoint);
        }

        [HttpGet("outdoor-space/{reference}")]
        public async Task<IActionResult> OutdoorSpace_Get(string reference, QuestionFlowStep? entryPoint = null)
        {
            var propertyData = await propertyDataStore.LoadPropertyDataAsync(reference);
            
            var viewModel = new OutdoorSpaceViewModel
            {
                HasOutdoorSpace = propertyData.HasOutdoorSpace,
                HintHasOutdoorSpace = propertyData.HintHasOutdoorSpace,
                Reference = propertyData.Reference,
                EntryPoint = entryPoint,
                BackLink = GetBackUrl(QuestionFlowStep.OutdoorSpace, propertyData, entryPoint)
            };

            return View("OutdoorSpace", viewModel);
        }

        [HttpPost("outdoor-space/{reference}")]
        public async Task<IActionResult> OutdoorSpace_Post(OutdoorSpaceViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return await OutdoorSpace_Get(viewModel.Reference, viewModel.EntryPoint);
            }
            
            var nextStep = await answerService.UpdateHasOutdoorSpace(
                viewModel.Reference,
                viewModel.HasOutdoorSpace,
                viewModel.EntryPoint);
            
            return RedirectToNextStep(nextStep, viewModel.Reference, viewModel.EntryPoint);
        }
        
        [HttpGet("heating-type/{reference}")]
        public async Task<IActionResult> HeatingType_Get(string reference, QuestionFlowStep? entryPoint = null)
        {
            var propertyData = await propertyDataStore.LoadPropertyDataAsync(reference);
            
            var viewModel = new HeatingTypeViewModel
            {
                HeatingType = propertyData.HeatingType,
                Reference = propertyData.Reference,
                EntryPoint = entryPoint,
                Epc = propertyData.Epc,
                BackLink = GetBackUrl(QuestionFlowStep.HeatingType, propertyData, entryPoint)
            };

            return View("HeatingType", viewModel);
        }

        [HttpPost("heating-type/{reference}")]
        public async Task<IActionResult> HeatingType_Post(HeatingTypeViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return await HeatingType_Get(viewModel.Reference, viewModel.EntryPoint);
            }
            
            var nextStep = await answerService.UpdateHeatingType(
                viewModel.Reference,
                viewModel.HeatingType,
                viewModel.EntryPoint);
            
            return RedirectToNextStep(nextStep, viewModel.Reference, viewModel.EntryPoint);
        }

        [HttpGet("other-heating-type/{reference}")]
        public async Task<IActionResult> OtherHeatingType_Get(string reference, QuestionFlowStep? entryPoint = null)
        {
            var propertyData = await propertyDataStore.LoadPropertyDataAsync(reference);

            var viewModel = new OtherHeatingTypeViewModel
            {
                OtherHeatingType = propertyData.OtherHeatingType,
                Reference = propertyData.Reference,
                EntryPoint = entryPoint,
                Epc = propertyData.Epc,
                BackLink = GetBackUrl(QuestionFlowStep.OtherHeatingType, propertyData, entryPoint)
            };

            return View("OtherHeatingType", viewModel);
        }

        [HttpPost("other-heating-type/{reference}")]
        public async Task<IActionResult> OtherHeatingType_Post(OtherHeatingTypeViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return await OtherHeatingType_Get(viewModel.Reference, viewModel.EntryPoint);
            }
            
            var nextStep = await answerService.UpdateOtherHeatingType(
                viewModel.Reference,
                viewModel.OtherHeatingType,
                viewModel.EntryPoint);
            
            return RedirectToNextStep(nextStep, viewModel.Reference, viewModel.EntryPoint);
        }


        [HttpGet("hot-water-cylinder/{reference}")]
        public async Task<IActionResult> HotWaterCylinder_Get(string reference, QuestionFlowStep? entryPoint = null)
        {
            var propertyData = await propertyDataStore.LoadPropertyDataAsync(reference);

            var viewModel = new HotWaterCylinderViewModel
            {
                HasHotWaterCylinder = propertyData.HasHotWaterCylinder,
                Reference = propertyData.Reference,
                Epc = propertyData.Epc,
                EntryPoint = entryPoint,
                BackLink = GetBackUrl(QuestionFlowStep.HotWaterCylinder, propertyData, entryPoint)
            };

            return View("HotWaterCylinder", viewModel);
        }

        [HttpPost("hot-water-cylinder/{reference}")]
        public async Task<IActionResult> HotWaterCylinder_Post(HotWaterCylinderViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return await HotWaterCylinder_Get(viewModel.Reference, viewModel.EntryPoint);
            }
            
            var nextStep = await answerService.UpdateHasHotWaterCylinder(
                viewModel.Reference,
                viewModel.HasHotWaterCylinder,
                viewModel.EntryPoint);
            
            return RedirectToNextStep(nextStep, viewModel.Reference, viewModel.EntryPoint);
        }
        
        [HttpGet("number-of-occupants/{reference}")]
        public async Task<IActionResult> NumberOfOccupants_Get(string reference, QuestionFlowStep? entryPoint = null)
        {
            var propertyData = await propertyDataStore.LoadPropertyDataAsync(reference);

            var viewModel = new NumberOfOccupantsViewModel
            {
                NumberOfOccupants = propertyData.NumberOfOccupants,
                Reference = propertyData.Reference,
                EntryPoint = entryPoint,
                BackLink = GetBackUrl(QuestionFlowStep.NumberOfOccupants, propertyData, entryPoint)
            };

            return View("NumberOfOccupants", viewModel);
        }

        [HttpPost("number-of-occupants/{reference}")]
        public async Task<IActionResult> NumberOfOccupants_Post(NumberOfOccupantsViewModel viewModel, bool? skip = null)
        {
            if (!ModelState.IsValid && skip != true)
            {
                return await NumberOfOccupants_Get(viewModel.Reference, viewModel.EntryPoint);
            }
            
            var numberOfOccupants = skip == true ? null : viewModel.NumberOfOccupants;
            var nextStep = await answerService.UpdateNumberOfOccupants(
                viewModel.Reference,
                numberOfOccupants,
                viewModel.EntryPoint);
            
            return RedirectToNextStep(nextStep, viewModel.Reference, viewModel.EntryPoint);
        }
        
        [HttpGet("heating-pattern/{reference}")]
        public async Task<IActionResult> HeatingPattern_Get(string reference, QuestionFlowStep? entryPoint = null)
        {
            var propertyData = await propertyDataStore.LoadPropertyDataAsync(reference);
            
            var viewModel = new HeatingPatternViewModel
            {
                HeatingPattern = propertyData.HeatingPattern,
                HoursOfHeatingMorning = propertyData.HoursOfHeatingMorning,
                HoursOfHeatingEvening = propertyData.HoursOfHeatingEvening,
                Reference = propertyData.Reference,
                EntryPoint = entryPoint,
                BackLink = GetBackUrl(QuestionFlowStep.HeatingPattern, propertyData, entryPoint)
            };

            return View("HeatingPattern", viewModel);
        }

        [HttpPost("heating-pattern/{reference}")]
        public async Task<IActionResult> HeatingPattern_Post(HeatingPatternViewModel viewModel)
        {
            if (viewModel.HeatingPattern != HeatingPattern.Other)
            {
                ModelState.Remove("HoursOfHeatingMorning");
                ModelState.Remove("HoursOfHeatingEvening");
            }
            
            if (!ModelState.IsValid)
            {
                return await HeatingPattern_Get(viewModel.Reference, viewModel.EntryPoint);
            }
            
            var nextStep = await answerService.UpdateHeatingPattern(
                viewModel.Reference,
                viewModel.HeatingPattern,
                viewModel.HoursOfHeatingMorning,
                viewModel.HoursOfHeatingEvening,
                viewModel.EntryPoint);
            
            return RedirectToNextStep(nextStep, viewModel.Reference, viewModel.EntryPoint);
        }
        
        [HttpGet("temperature/{reference}")]
        public async Task<IActionResult> Temperature_Get(string reference, QuestionFlowStep? entryPoint = null)
        {
            var propertyData = await propertyDataStore.LoadPropertyDataAsync(reference);
            
            var viewModel = new TemperatureViewModel
            {
                Temperature = propertyData.Temperature,
                Reference = propertyData.Reference,
                EntryPoint = entryPoint,
                BackLink = GetBackUrl(QuestionFlowStep.Temperature, propertyData, entryPoint)
            };

            return View("Temperature", viewModel);
        }

        [HttpPost("temperature/{reference}")]
        public async Task<IActionResult> Temperature_Post(TemperatureViewModel viewModel, bool? skip = null)
        {
            if (!ModelState.IsValid && skip != true)
            {
                return await Temperature_Get(viewModel.Reference, viewModel.EntryPoint);
            }

            var temperature = skip == true ? null : viewModel.Temperature;
            var nextStep = await answerService.UpdateTemperature(
                viewModel.Reference,
                temperature,
                viewModel.EntryPoint);
            
            return RedirectToNextStep(nextStep, viewModel.Reference, viewModel.EntryPoint);
        }

        [HttpGet("answer-summary/{reference}")]
        public async Task<IActionResult> AnswerSummary_Get(string reference)
        {
            var propertyData = await propertyDataStore.LoadPropertyDataAsync(reference);
            
            // If the user comes back to this page and their changes haven't been saved,
            // we need to revert to their old answers.
            if (propertyData.UneditedData is not null)
            {
                propertyData.RevertToUneditedData();
                await propertyDataStore.SavePropertyDataAsync(propertyData);
            }

            var viewModel = new AnswerSummaryViewModel
            {
                PropertyData = propertyData,
                BackLink = GetBackUrl(QuestionFlowStep.AnswerSummary, propertyData)
            };
            
            return View("AnswerSummary", viewModel);
        }

        [HttpPost("answer-summary/{reference}")]
        public async Task<IActionResult> AnswerSummary_Post(string reference)
        {
            if (!ModelState.IsValid)
            {
                return await AnswerSummary_Get(reference);
            }
            
            var propertyData = await propertyDataStore.LoadPropertyDataAsync(reference);
            if (propertyData.PropertyRecommendations is null || propertyData.PropertyRecommendations.Count == 0)
            {
                var recommendationsForPropertyAsync = await recommendationService.GetRecommendationsForPropertyAsync(propertyData);
                propertyData.PropertyRecommendations = recommendationsForPropertyAsync.Select(r => 
                    new PropertyRecommendation()
                    {
                        Key = r.Key,
                        Title = r.Title,
                        MinInstallCost = r.MinInstallCost,
                        MaxInstallCost = r.MaxInstallCost,
                        Saving = r.Saving,
                        LifetimeSaving = r.LifetimeSaving,
                        Lifetime = r.Lifetime,
                        Summary = r.Summary
                    }
                ).ToList();
            }

            propertyData.HasSeenRecommendations = true;
            await propertyDataStore.SavePropertyDataAsync(propertyData);

            var nextStep = questionFlowService.NextStep(QuestionFlowStep.AnswerSummary, propertyData);
            var forwardArgs = GetActionArgumentsForQuestion(nextStep, reference);
            return RedirectToAction(forwardArgs.Action, forwardArgs.Controller, forwardArgs.Values);
        }

        [HttpGet("returning-user/{reference}")]
        public async Task<IActionResult> ReturningUser_Get(string reference, bool fromMagicLink = true)
        {
            if (cookieService.HasAcceptedGoogleAnalytics(Request))
            {
                await googleAnalyticsService.SendEvent(new GaRequestBody
                {
                    ClientId = googleAnalyticsService.GetClientId(Request),
                    GaEvents = new List<GaEvent>
                    {
                        new()
                        {
                            Name = "user_returned",
                            Parameters = new Dictionary<string, object>
                            {
                                {"value", fromMagicLink ? "link" : "code"}
                            }
                        }
                    }
                });
            }
            
            var propertyData = await propertyDataStore.LoadPropertyDataAsync(reference);
            
            // Arriving here means the user used the reference code or the magic link,
            // so we mark the user as a returning user
            propertyData.ReturningUser = true;
            await propertyDataStore.SavePropertyDataAsync(propertyData);
            
            var recommendations = propertyData.PropertyRecommendations;
            if (!recommendations.Any())
            {
                return RedirectToAction(nameof(NoRecommendations_Get), "EnergyEfficiency", new { reference });
            }

            var firstNotActionedRecommendation = recommendations.Find(recommendation => recommendation.RecommendationAction is null);
            if (firstNotActionedRecommendation is not null)
            {
                return RedirectToAction(nameof(Recommendation_Get), "EnergyEfficiency",
                    new { id = (int)firstNotActionedRecommendation.Key, reference = propertyData.Reference });
            }
            
            return RedirectToAction("ActionPlan_Get", new {reference = propertyData.Reference});
        }

        [HttpGet("no-recommendations/{reference}")]
        public async Task<IActionResult> NoRecommendations_Get(string reference, string emailAddress = null, bool emailSent = false)
        {
            var propertyData = await propertyDataStore.LoadPropertyDataAsync(reference);
            var viewModel = new NoRecommendationsViewModel
            {
                Reference = propertyData.Reference,
                EmailAddress = emailAddress,
                EmailSent = emailSent,
                HasOutdoorSpace = propertyData.HasOutdoorSpace,
                BackLink = GetBackUrl(QuestionFlowStep.NoRecommendations, propertyData)
            };
            
            return View("NoRecommendations", viewModel);
        }

        [HttpPost("no-recommendations/{reference}")]
        public async Task<IActionResult> NoRecommendations_Post(NoRecommendationsViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return await NoRecommendations_Get(viewModel.Reference, emailAddress: viewModel.EmailAddress);
            }

            TrySendReferenceNumberEmail(viewModel.EmailAddress, viewModel.Reference);
            
            if (!ModelState.IsValid)
            {
                return await NoRecommendations_Get(viewModel.Reference, emailAddress: viewModel.EmailAddress);
            }

            return RedirectToAction(nameof(NoRecommendations_Get), "EnergyEfficiency",
                new
                {
                    reference = viewModel.Reference, 
                    emailAddress = viewModel.EmailAddress,
                    emailSent = true
                }, "email-sent");
        }

        [HttpGet("your-recommendations/{reference}")]
        public async Task<IActionResult> YourRecommendations_Get(string reference)
        {
            var propertyData = await propertyDataStore.LoadPropertyDataAsync(reference);
            var viewModel = new YourRecommendationsViewModel
            {
                Reference = reference,
                NumberOfPropertyRecommendations = propertyData.PropertyRecommendations.Count,
                HasEmailAddress = false,
                BackLink = GetBackUrl(QuestionFlowStep.YourRecommendations, propertyData)
            };
            
            return View("YourRecommendations", viewModel);
        }

        [HttpPost("your-recommendations/{reference}")]
        public async Task<IActionResult> YourRecommendations_Post(YourRecommendationsViewModel viewModel)
        {
            if (ModelState.IsValid && viewModel.HasEmailAddress)
            {
                TrySendReferenceNumberEmail(viewModel.EmailAddress, viewModel.Reference);
            }
            
            if (!ModelState.IsValid)
            {
                return await YourRecommendations_Get(viewModel.Reference);
            }
            
            var propertyData = await propertyDataStore.LoadPropertyDataAsync(viewModel.Reference);
            return RedirectToAction("Recommendation_Get", new { id = (int)propertyData.GetFirstRecommendationKey(), reference = viewModel.Reference });
        }

        [HttpGet("your-recommendations/{id}/{reference}")]
        public async Task<IActionResult> Recommendation_Get(int id, string reference, bool fromActionPlan = false)
        {
            var recommendationKey = (RecommendationKey)id;
            var propertyData = await propertyDataStore.LoadPropertyDataAsync(reference);
            var recommendationIndex = propertyData.GetRecommendationIndex(recommendationKey);
            string backLink;

            if (fromActionPlan)
            {
                backLink = Url.Action(nameof(ActionPlan_Get), new { reference });
            }
            else if (recommendationIndex == 0)
            {
                backLink = Url.Action(nameof(YourRecommendations_Get), "EnergyEfficiency", new { reference });
            }
            else
            {
                try
                {
                    backLink = Url.Action(nameof(Recommendation_Get), "EnergyEfficiency",
                        new { id = (int) propertyData.GetPreviousRecommendationKey(recommendationKey), reference });
                }
                catch
                {
                    // In case user manually typed in an address with an incorrect recommendation id 
                    return View("../Error/PageNotFound");
                }
            }
            
            var viewModel = new RecommendationViewModel
            {
                RecommendationIndex = recommendationIndex,
                PropertyRecommendations = propertyData.PropertyRecommendations,
                RecommendationAction = propertyData.PropertyRecommendations[recommendationIndex].RecommendationAction,
                FromActionPlan = fromActionPlan,
                BackLink = backLink
            };

            return View("recommendations/" + Enum.GetName(recommendationKey), viewModel);
        }

        [HttpPost("your-recommendations/{id}/{reference}")]
        public async Task<IActionResult> Recommendation_Post(RecommendationViewModel viewModel, string command, int id, string reference)
        {
            var recommendationKey = (RecommendationKey)id;
            var propertyData = await propertyDataStore.LoadPropertyDataAsync(reference);
            if (command == "goBackwards")
            {
                return RedirectToAction(nameof(Recommendation_Get),
                    new {id = (int)propertyData.GetPreviousRecommendationKey(recommendationKey), reference = propertyData.Reference});
            }
            
            if (!ModelState.IsValid)
            {
                return await Recommendation_Get(id, reference, viewModel.FromActionPlan);
            }
            
            propertyData.PropertyRecommendations.Single(r => r.Key == recommendationKey).RecommendationAction =
                viewModel.RecommendationAction;
            await propertyDataStore.SavePropertyDataAsync(propertyData);

            if (command == "goForwards")
            {
                if (propertyData.GetLastRecommendationKey() == recommendationKey || viewModel.FromActionPlan)
                {
                    return RedirectToAction(nameof(ActionPlan_Get), new {reference = propertyData.Reference});
                }

                return RedirectToAction(nameof(Recommendation_Get),
                    new {id = (int)propertyData.GetNextRecommendationKey(recommendationKey), reference = propertyData.Reference});
            }
            
            throw new ArgumentOutOfRangeException();
        }

        [HttpGet("action-plan/{reference}")]
        public async Task<IActionResult> ActionPlan_Get(string reference, string emailAddress = null, bool emailSent = false)
        {
            var propertyData = await propertyDataStore.LoadPropertyDataAsync(reference);
            
            var viewModel = new ActionPlanViewModel
            {
                BackLink = Url.Action(nameof(Recommendation_Get), new { id = (int)propertyData.GetLastRecommendationKey(), reference }),
                PropertyData = propertyData,
                EmailAddress = emailAddress,
                EmailSent = emailSent
            };

            if (viewModel.GetSavedRecommendations().Any())
            {
                return View("ActionPlan/ActionPlanWithSavedRecommendations", viewModel);
            }

            if (viewModel.GetDecideLaterRecommendations().Any())
            {
                return View("ActionPlan/ActionPlanWithMaybeRecommendations", viewModel);
            }

            return View("ActionPlan/ActionPlanWithDiscardedRecommendations", viewModel);
        }

        [HttpPost("action-plan/{reference}")]
        public async Task<IActionResult> ActionPlan_Post(YourSavedRecommendationsEmailViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return await ActionPlan_Get(viewModel.Reference, emailAddress: viewModel.EmailAddress);
            }

            TrySendReferenceNumberEmail(viewModel.EmailAddress, viewModel.Reference);
            
            if (!ModelState.IsValid)
            {
                return await ActionPlan_Get(viewModel.Reference, emailAddress: viewModel.EmailAddress);
            }

            return RedirectToAction(nameof(ActionPlan_Get), "EnergyEfficiency",
                new
                {
                    reference = viewModel.Reference, emailAddress = viewModel.EmailAddress,
                    emailSent = true
                }, "email-sent");
        }

        private void TrySendReferenceNumberEmail(string emailAddress, string reference)
        {
            try
            {
                emailApi.SendReferenceNumberEmail(emailAddress, reference);
            }
            catch (EmailSenderException e)
            {
                switch (e.Type)
                {
                    case EmailSenderExceptionType.InvalidEmailAddress:
                        ModelState.AddModelError(nameof(emailAddress), "Enter a valid email address");
                        return;
                    case EmailSenderExceptionType.Other:
                        ModelState.AddModelError(nameof(emailAddress), "Unable to send email due to unexpected error. Please make a note of your reference code.");
                        return;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private string GetBackUrl(
            QuestionFlowStep currentStep,
            PropertyData propertyData = null,
            QuestionFlowStep? entryPoint = null)
        {
            var backStep = questionFlowService.PreviousStep(currentStep, propertyData, entryPoint);
            var args = GetActionArgumentsForQuestion(backStep, propertyData?.Reference, entryPoint);
            return Url.Action(args.Action, args.Controller, args.Values);
        }
        
        private string GetSkipUrl(
            QuestionFlowStep currentStep,
            PropertyData propertyData)
        {
            var skipDestination = questionFlowService.SkipDestination(currentStep, propertyData);
            var args = GetActionArgumentsForQuestion(skipDestination, propertyData?.Reference);
            return Url.Action(args.Action, args.Controller, args.Values);
        }

        private string GetForwardUrl(
            QuestionFlowStep currentStep,
            PropertyData propertyData,
            QuestionFlowStep? entryPoint = null)
        {
            var nextStep = questionFlowService.NextStep(currentStep, propertyData, entryPoint);
            var args = GetActionArgumentsForQuestion(nextStep, propertyData?.Reference, entryPoint);
            return Url.Action(args.Action, args.Controller, args.Values);
        }
        
        private RedirectToActionResult RedirectToNextStep(QuestionFlowStep nextStep, string reference, QuestionFlowStep? entryPoint = null)
        {
            var forwardArgs = GetActionArgumentsForQuestion(nextStep, reference, entryPoint);
            return RedirectToAction(forwardArgs.Action, forwardArgs.Controller, forwardArgs.Values);
        }

        private PathByActionArguments GetActionArgumentsForQuestion(
            QuestionFlowStep question,
            string reference = null,
            QuestionFlowStep? entryPoint = null,
            IDictionary<string, object> extraRouteValues = null)
        {
            return question switch
            {
                QuestionFlowStep.Start => new PathByActionArguments(nameof(Index), "EnergyEfficiency"),
                QuestionFlowStep.NewOrReturningUser => new PathByActionArguments(nameof(NewOrReturningUser_Get), "EnergyEfficiency"),
                QuestionFlowStep.OwnershipStatus => new PathByActionArguments(nameof(OwnershipStatus_Get), "EnergyEfficiency", GetRouteValues(reference, extraRouteValues)),
                QuestionFlowStep.Country => new PathByActionArguments(nameof(Country_Get), "EnergyEfficiency", GetRouteValues(reference, extraRouteValues)),
                QuestionFlowStep.FindEpc => new PathByActionArguments(nameof(FindEpc_Get), "EnergyEfficiency", GetRouteValues(reference, extraRouteValues)),
                QuestionFlowStep.ServiceUnsuitable => new PathByActionArguments(nameof(ServiceUnsuitable), "EnergyEfficiency", GetRouteValues(reference, extraRouteValues)),
                QuestionFlowStep.AskForPostcode => new PathByActionArguments(nameof(AskForPostcode_Get), "EnergyEfficiency", GetRouteValues(reference, extraRouteValues)),
                QuestionFlowStep.ConfirmAddress => new PathByActionArguments(nameof(ConfirmAddress_Get), "EnergyEfficiency", GetRouteValues(reference, extraRouteValues)),
                QuestionFlowStep.ConfirmEpcDetails => new PathByActionArguments(nameof(ConfirmEpcDetails_Get), "EnergyEfficiency", GetRouteValues(reference, extraRouteValues)),
                QuestionFlowStep.NoEpcFound => new PathByActionArguments(nameof(NoEpcFound_Get), "EnergyEfficiency", GetRouteValues(reference, extraRouteValues)),
                QuestionFlowStep.PropertyType => new PathByActionArguments(nameof(PropertyType_Get), "EnergyEfficiency", GetRouteValues(reference, extraRouteValues, entryPoint)),
                QuestionFlowStep.HouseType => new PathByActionArguments(nameof(HouseType_Get), "EnergyEfficiency", GetRouteValues(reference, extraRouteValues, entryPoint)),
                QuestionFlowStep.BungalowType => new PathByActionArguments(nameof(BungalowType_Get), "EnergyEfficiency", GetRouteValues(reference, extraRouteValues, entryPoint)),
                QuestionFlowStep.FlatType => new PathByActionArguments(nameof(FlatType_Get), "EnergyEfficiency", GetRouteValues(reference, extraRouteValues, entryPoint)),
                QuestionFlowStep.HomeAge => new PathByActionArguments(nameof(HomeAge_Get), "EnergyEfficiency", GetRouteValues(reference, extraRouteValues, entryPoint)),
                QuestionFlowStep.CheckYourUnchangeableAnswers => new PathByActionArguments(nameof(CheckYourUnchangeableAnswers_Get), "EnergyEfficiency", GetRouteValues(reference, extraRouteValues)),
                QuestionFlowStep.WallConstruction => new PathByActionArguments(nameof(WallConstruction_Get), "EnergyEfficiency", GetRouteValues(reference, extraRouteValues, entryPoint)),
                QuestionFlowStep.CavityWallsInsulated => new PathByActionArguments(nameof(CavityWallsInsulated_Get), "EnergyEfficiency", GetRouteValues(reference, extraRouteValues, entryPoint)),
                QuestionFlowStep.SolidWallsInsulated => new PathByActionArguments(nameof(SolidWallsInsulated_Get), "EnergyEfficiency", GetRouteValues(reference, extraRouteValues, entryPoint)),
                QuestionFlowStep.FloorConstruction => new PathByActionArguments(nameof(FloorConstruction_Get), "EnergyEfficiency", GetRouteValues(reference, extraRouteValues, entryPoint)),
                QuestionFlowStep.FloorInsulated => new PathByActionArguments(nameof(FloorInsulated_Get), "EnergyEfficiency", GetRouteValues(reference, extraRouteValues, entryPoint)),
                QuestionFlowStep.RoofConstruction => new PathByActionArguments(nameof(RoofConstruction_Get), "EnergyEfficiency", GetRouteValues(reference, extraRouteValues, entryPoint)),
                QuestionFlowStep.LoftSpace => new PathByActionArguments(nameof(LoftSpace_Get), "EnergyEfficiency", GetRouteValues(reference, extraRouteValues, entryPoint)),
                QuestionFlowStep.LoftAccess => new PathByActionArguments(nameof(LoftAccess_Get), "EnergyEfficiency", GetRouteValues(reference, extraRouteValues, entryPoint)),
                QuestionFlowStep.RoofInsulated => new PathByActionArguments(nameof(RoofInsulated_Get), "EnergyEfficiency", GetRouteValues(reference, extraRouteValues, entryPoint)),
                QuestionFlowStep.OutdoorSpace => new PathByActionArguments(nameof(OutdoorSpace_Get), "EnergyEfficiency", GetRouteValues(reference, extraRouteValues, entryPoint)),
                QuestionFlowStep.GlazingType => new PathByActionArguments(nameof(GlazingType_Get), "EnergyEfficiency", GetRouteValues(reference, extraRouteValues, entryPoint)),
                QuestionFlowStep.HeatingType => new PathByActionArguments(nameof(HeatingType_Get), "EnergyEfficiency", GetRouteValues(reference, extraRouteValues, entryPoint)),
                QuestionFlowStep.OtherHeatingType => new PathByActionArguments(nameof(OtherHeatingType_Get), "EnergyEfficiency", GetRouteValues(reference, extraRouteValues, entryPoint)),
                QuestionFlowStep.HotWaterCylinder => new PathByActionArguments(nameof(HotWaterCylinder_Get), "EnergyEfficiency", GetRouteValues(reference, extraRouteValues, entryPoint)),
                QuestionFlowStep.NumberOfOccupants => new PathByActionArguments(nameof(NumberOfOccupants_Get), "EnergyEfficiency", GetRouteValues(reference, extraRouteValues, entryPoint)),
                QuestionFlowStep.HeatingPattern => new PathByActionArguments(nameof(HeatingPattern_Get), "EnergyEfficiency", GetRouteValues(reference, extraRouteValues, entryPoint)),
                QuestionFlowStep.Temperature => new PathByActionArguments(nameof(Temperature_Get), "EnergyEfficiency", GetRouteValues(reference, extraRouteValues, entryPoint)),
                QuestionFlowStep.AnswerSummary => new PathByActionArguments(nameof(AnswerSummary_Get), "EnergyEfficiency", GetRouteValues(reference, extraRouteValues)),
                QuestionFlowStep.NoRecommendations => new PathByActionArguments(nameof(NoRecommendations_Get), "EnergyEfficiency", GetRouteValues(reference, extraRouteValues)),
                QuestionFlowStep.YourRecommendations => new PathByActionArguments(nameof(YourRecommendations_Get), "EnergyEfficiency", GetRouteValues(reference, extraRouteValues)),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private RouteValueDictionary GetRouteValues(
            string reference,
            IDictionary<string, object> extraRouteValues,
            QuestionFlowStep? entryPoint = null)
        {
            if (reference == null)
            {
                throw new ArgumentException("Reference must be provided");
            }

            // If entryPoint is null then it won't appear in the URL
            var ret = new RouteValueDictionary { { "reference", reference }, { "entryPoint", entryPoint } };

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
}
