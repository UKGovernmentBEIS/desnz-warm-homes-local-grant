using SeaPublicWebsite.BusinessLogic.ExternalServices.EpbEpc;
using SeaPublicWebsite.BusinessLogic.Models;
using SeaPublicWebsite.BusinessLogic.Models.Enums;
using SeaPublicWebsite.BusinessLogic.Services;

namespace SeaPublicWebsite.BusinessLogic;

public class PropertyDataUpdater
{
    private readonly IQuestionFlowService questionFlowService;
    private readonly IEpcApi epcApi;
    
    public PropertyDataUpdater(
        IQuestionFlowService questionFlowService,
        IEpcApi epcApi)
    {
        this.questionFlowService = questionFlowService;
        this.epcApi = epcApi;
    }
    
    public QuestionFlowStep UpdateOwnershipStatus(PropertyData propertyData, OwnershipStatus? ownershipStatus)
    {
        return UpdatePropertyData(
            p => { p.OwnershipStatus = ownershipStatus; },
            propertyData,
            QuestionFlowStep.OwnershipStatus);
    }
    
    public QuestionFlowStep UpdateCountry(PropertyData propertyData, Country? country)
    {
        return UpdatePropertyData(
            p => { p.Country = country; },
            propertyData,
            QuestionFlowStep.Country);
    }
    
    public QuestionFlowStep UpdateSearchForEpc(PropertyData propertyData, SearchForEpc? searchForEpc)
    {
        return UpdatePropertyData(
            p =>
            {
                p.SearchForEpc = searchForEpc;
                p.EpcDetailsConfirmed = null;
                p.Epc = null;
                p.PropertyType = null;
                p.YearBuilt = null;
            },
            propertyData,
            QuestionFlowStep.FindEpc);
    }
    
    public async Task<QuestionFlowStep> SetEpcAsync(PropertyData propertyData, string epcId)
    {
        var epc = epcId == null ? null : await epcApi.GetEpcForId(epcId);

        return UpdatePropertyData(
            p => { p.Epc = epc; },
            propertyData,
            QuestionFlowStep.ConfirmAddress);
    }

    public QuestionFlowStep ConfirmEpcDetails(PropertyData propertyData, EpcDetailsConfirmed? confirmed)
    {
        return UpdatePropertyData(
            p =>
            {
                p.EpcDetailsConfirmed = confirmed;
                Epc epc = p.Epc;
                if (confirmed == EpcDetailsConfirmed.Yes)
                {
                    p.PropertyType = epc.PropertyType;
                    p.HouseType = epc.HouseType;
                    p.BungalowType = epc.BungalowType;
                    p.FlatType = epc.FlatType;
                    p.YearBuilt = epc.ConstructionAgeBand switch
                    {
                        HomeAge.Pre1900 => YearBuilt.Pre1930,
                        HomeAge.From1900To1929 => YearBuilt.Pre1930,
                        HomeAge.From1930To1949 => YearBuilt.From1930To1966,
                        HomeAge.From1950To1966 => YearBuilt.From1930To1966,
                        HomeAge.From1967To1975 => YearBuilt.From1967To1982,
                        HomeAge.From1976To1982 => YearBuilt.From1967To1982,
                        HomeAge.From1983To1990 => YearBuilt.From1983To1995,
                        HomeAge.From1991To1995 => YearBuilt.From1983To1995,
                        HomeAge.From1996To2002 => YearBuilt.From1996To2011,
                        HomeAge.From2003To2006 => YearBuilt.From1996To2011,
                        HomeAge.From2007To2011 => YearBuilt.From1996To2011,
                        HomeAge.From2012ToPresent => YearBuilt.From2012ToPresent,
                        _ => throw new ArgumentOutOfRangeException()
                    };
                }
            },
            propertyData,
            QuestionFlowStep.ConfirmEpcDetails);
    }
    
    public QuestionFlowStep UpdatePropertyType(
        PropertyData propertyData,
        PropertyType? propertyType,
        QuestionFlowStep? entryPoint)
    {
        return UpdatePropertyData(
            p => { p.PropertyType = propertyType; },
            propertyData,
            QuestionFlowStep.PropertyType,
            entryPoint);
    }
    
    public QuestionFlowStep UpdateHouseType(
        PropertyData propertyData,
        HouseType? houseType,
        QuestionFlowStep? entryPoint)
    {
        return UpdatePropertyData(
            p => { p.HouseType = houseType; },
            propertyData,
            QuestionFlowStep.HouseType,
            entryPoint);
    }
    
    public QuestionFlowStep UpdateBungalowType(
        PropertyData propertyData,
        BungalowType? bungalowType,
        QuestionFlowStep? entryPoint)
    {
        return UpdatePropertyData(
            p => { p.BungalowType = bungalowType; },
            propertyData,
            QuestionFlowStep.BungalowType,
            entryPoint);
    }
    
    public QuestionFlowStep UpdateFlatType(
        PropertyData propertyData,
        FlatType? flatType,
        QuestionFlowStep? entryPoint)
    {
        return UpdatePropertyData(
            p => { p.FlatType = flatType; },
            propertyData,
            QuestionFlowStep.FlatType,
            entryPoint);
    }
    
    public QuestionFlowStep UpdateYearBuilt(
        PropertyData propertyData,
        YearBuilt? yearBuilt,
        QuestionFlowStep? entryPoint)
    {
        return UpdatePropertyData(
            p => { p.YearBuilt = yearBuilt; },
            propertyData,
            QuestionFlowStep.HomeAge,
            entryPoint);
    }
    
    public QuestionFlowStep UpdateWallConstruction(
        PropertyData propertyData,
        WallConstruction? wallConstruction,
        QuestionFlowStep? entryPoint)
    {
        return UpdatePropertyData(
            p => { p.WallConstruction = wallConstruction; },
            propertyData,
            QuestionFlowStep.WallConstruction,
            entryPoint);
    }
    
    public QuestionFlowStep UpdateCavityWallInsulation(
        PropertyData propertyData,
        CavityWallsInsulated? cavityWallsInsulated,
        QuestionFlowStep? entryPoint)
    {
        return UpdatePropertyData(
            p => { p.CavityWallsInsulated = cavityWallsInsulated; },
            propertyData,
            QuestionFlowStep.CavityWallsInsulated,
            entryPoint);
    }
    
    public QuestionFlowStep UpdateSolidWallInsulation(
        PropertyData propertyData,
        SolidWallsInsulated? solidWallsInsulated,
        QuestionFlowStep? entryPoint)
    {
        return UpdatePropertyData(
            p => { p.SolidWallsInsulated = solidWallsInsulated; },
            propertyData,
            QuestionFlowStep.SolidWallsInsulated,
            entryPoint);
    }
    
    public QuestionFlowStep UpdateFloorConstruction(
        PropertyData propertyData,
        FloorConstruction? floorConstruction,
        QuestionFlowStep? entryPoint)
    {
        return UpdatePropertyData(
            p => { p.FloorConstruction = floorConstruction; },
            propertyData,
            QuestionFlowStep.FloorConstruction,
            entryPoint);
    }
    
    public QuestionFlowStep UpdateFloorInsulated(
        PropertyData propertyData,
        FloorInsulated? floorInsulated,
        QuestionFlowStep? entryPoint)
    {
        return UpdatePropertyData(
            p => { p.FloorInsulated = floorInsulated; },
            propertyData,
            QuestionFlowStep.FloorInsulated,
            entryPoint);
    }
    
    public QuestionFlowStep UpdateRoofConstruction(
        PropertyData propertyData,
        RoofConstruction? roofConstruction,
        QuestionFlowStep? entryPoint)
    {
        return UpdatePropertyData(
            p => { p.RoofConstruction = roofConstruction; },
            propertyData,
            QuestionFlowStep.RoofConstruction,
            entryPoint);
    }
    
    public QuestionFlowStep UpdateLoftSpace(
        PropertyData propertyData,
        LoftSpace? loftSpace,
        QuestionFlowStep? entryPoint)
    {
        return UpdatePropertyData(
            p => { p.LoftSpace = loftSpace; },
            propertyData,
            QuestionFlowStep.LoftSpace,
            entryPoint);
    }
    
    public QuestionFlowStep UpdateLoftAccess(
        PropertyData propertyData,
        LoftAccess? loftAccess,
        QuestionFlowStep? entryPoint)
    {
        return UpdatePropertyData(
            p => { p.LoftAccess = loftAccess; },
            propertyData,
            QuestionFlowStep.LoftAccess,
            entryPoint);
    }
    
    public QuestionFlowStep UpdateRoofInsulated(
        PropertyData propertyData,
        RoofInsulated? roofInsulated,
        QuestionFlowStep? entryPoint)
    {
        return UpdatePropertyData(
            p => { p.RoofInsulated = roofInsulated; },
            propertyData,
            QuestionFlowStep.RoofInsulated,
            entryPoint);
    }
    
    public QuestionFlowStep UpdateGlazingType(
        PropertyData propertyData,
        GlazingType? glazingType,
        QuestionFlowStep? entryPoint)
    {
        return UpdatePropertyData(
            p => { p.GlazingType = glazingType; },
            propertyData,
            QuestionFlowStep.GlazingType,
            entryPoint);
    }
    
    public QuestionFlowStep UpdateHasOutdoorSpace(
        PropertyData propertyData,
        HasOutdoorSpace? hasOutdoorSpace,
        QuestionFlowStep? entryPoint)
    {
        return UpdatePropertyData(
            p => { p.HasOutdoorSpace = hasOutdoorSpace; },
            propertyData,
            QuestionFlowStep.OutdoorSpace,
            entryPoint);
    }
    
    public QuestionFlowStep UpdateHeatingType(
        PropertyData propertyData,
        HeatingType? heatingType,
        QuestionFlowStep? entryPoint)
    {
        return UpdatePropertyData(
            p => { p.HeatingType = heatingType; },
            propertyData,
            QuestionFlowStep.HeatingType,
            entryPoint);
    }
    
    public QuestionFlowStep UpdateOtherHeatingType(
        PropertyData propertyData,
        OtherHeatingType? otherHeatingType,
        QuestionFlowStep? entryPoint)
    {
        return UpdatePropertyData(
            p => { p.OtherHeatingType = otherHeatingType; },
            propertyData,
            QuestionFlowStep.OtherHeatingType,
            entryPoint);
    }
    
    public QuestionFlowStep UpdateHasHotWaterCylinder(
        PropertyData propertyData,
        HasHotWaterCylinder? hasHotWaterCylinder,
        QuestionFlowStep? entryPoint)
    {
        return UpdatePropertyData(
            p => { p.HasHotWaterCylinder = hasHotWaterCylinder; },
            propertyData,
            QuestionFlowStep.HotWaterCylinder,
            entryPoint);
    }
    
    public QuestionFlowStep UpdateNumberOfOccupants(
        PropertyData propertyData,
        int? numberOfOccupants,
        QuestionFlowStep? entryPoint)
    {
        return UpdatePropertyData(
            p => { p.NumberOfOccupants = numberOfOccupants; },
            propertyData,
            QuestionFlowStep.NumberOfOccupants,
            entryPoint);
    }
    
    public QuestionFlowStep UpdateHeatingPattern(
        PropertyData propertyData,
        HeatingPattern? heatingPattern,
        int? hoursOfHeatingMorning,
        int? hoursOfHeatingEvening,
        QuestionFlowStep? entryPoint)
    {
        return UpdatePropertyData(
            p =>
            {
                p.HeatingPattern = heatingPattern;
                p.HoursOfHeatingMorning = hoursOfHeatingMorning;
                p.HoursOfHeatingEvening = hoursOfHeatingEvening;
            },
            propertyData,
            QuestionFlowStep.HeatingPattern,
            entryPoint);
    }
    
    public QuestionFlowStep UpdateTemperature(
        PropertyData propertyData,
        decimal? temperature,
        QuestionFlowStep? entryPoint)
    {
        return UpdatePropertyData(
            p => { p.Temperature = temperature; },
            propertyData,
            QuestionFlowStep.Temperature,
            entryPoint);
    }
    
    private QuestionFlowStep UpdatePropertyData(
        Action<PropertyData> update,
        PropertyData propertyData,
        QuestionFlowStep currentPage,
        QuestionFlowStep? entryPoint = null)
    {
        // If entryPoint is set then the user is editing their answers (and if HasSeenRecommendations then they have
        // already generated recommendations that may now need to change), so we need to take a copy of the current
        // answers
        if ((entryPoint is not null || propertyData.HasSeenRecommendations) && propertyData.UneditedData is null)
        {
            propertyData.CreateUneditedData();
        }
        
        update(propertyData);
        propertyData.ResetUnusedFields();
        
        var nextStep = questionFlowService.NextStep(currentPage, propertyData, entryPoint);
            
        // If the user is going back to the answer summary page or the check your unchangeable answers page then they
        // finished editing and we can get rid of the old answers
        if ((entryPoint is not null || propertyData.HasSeenRecommendations) &&
            (nextStep == QuestionFlowStep.AnswerSummary ||
             nextStep == QuestionFlowStep.CheckYourUnchangeableAnswers))
        {
            propertyData.CommitEdits();
        }
        
        return nextStep;
    }
}
