using HerPublicWebsite.BusinessLogic.ExternalServices.EpbEpc;
using HerPublicWebsite.BusinessLogic.Models;
using HerPublicWebsite.BusinessLogic.Models.Enums;
using HerPublicWebsite.BusinessLogic.Services;

namespace HerPublicWebsite.BusinessLogic;

public class PropertyDataUpdater
{
    private readonly IQuestionFlowServiceOld questionFlowServiceOld;
    private readonly IEpcApi epcApi;
    
    public PropertyDataUpdater(
        IQuestionFlowServiceOld questionFlowServiceOld,
        IEpcApi epcApi)
    {
        this.questionFlowServiceOld = questionFlowServiceOld;
        this.epcApi = epcApi;
    }
    
    public QuestionFlowStepOld UpdateOwnershipStatus(PropertyData propertyData, OwnershipStatus? ownershipStatus)
    {
        return UpdatePropertyData(
            p => { p.OwnershipStatus = ownershipStatus; },
            propertyData,
            QuestionFlowStepOld.OwnershipStatus);
    }
    
    public QuestionFlowStepOld UpdateCountry(PropertyData propertyData, Country? country)
    {
        return UpdatePropertyData(
            p => { p.Country = country; },
            propertyData,
            QuestionFlowStepOld.Country);
    }
    
    public QuestionFlowStepOld UpdateSearchForEpc(PropertyData propertyData, SearchForEpc? searchForEpc)
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
            QuestionFlowStepOld.FindEpc);
    }
    
    public async Task<QuestionFlowStepOld> SetEpcAsync(PropertyData propertyData, string epcId)
    {
        var epc = epcId == null ? null : await epcApi.GetEpcForId(epcId);

        return UpdatePropertyData(
            p => { p.Epc = epc; },
            propertyData,
            QuestionFlowStepOld.ConfirmAddress);
    }

    public QuestionFlowStepOld ConfirmEpcDetails(PropertyData propertyData, EpcDetailsConfirmed? confirmed)
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
            QuestionFlowStepOld.ConfirmEpcDetails);
    }
    
    public QuestionFlowStepOld UpdatePropertyType(
        PropertyData propertyData,
        PropertyType? propertyType,
        QuestionFlowStepOld? entryPoint)
    {
        return UpdatePropertyData(
            p => { p.PropertyType = propertyType; },
            propertyData,
            QuestionFlowStepOld.PropertyType,
            entryPoint);
    }
    
    public QuestionFlowStepOld UpdateHouseType(
        PropertyData propertyData,
        HouseType? houseType,
        QuestionFlowStepOld? entryPoint)
    {
        return UpdatePropertyData(
            p => { p.HouseType = houseType; },
            propertyData,
            QuestionFlowStepOld.HouseType,
            entryPoint);
    }
    
    public QuestionFlowStepOld UpdateBungalowType(
        PropertyData propertyData,
        BungalowType? bungalowType,
        QuestionFlowStepOld? entryPoint)
    {
        return UpdatePropertyData(
            p => { p.BungalowType = bungalowType; },
            propertyData,
            QuestionFlowStepOld.BungalowType,
            entryPoint);
    }
    
    public QuestionFlowStepOld UpdateFlatType(
        PropertyData propertyData,
        FlatType? flatType,
        QuestionFlowStepOld? entryPoint)
    {
        return UpdatePropertyData(
            p => { p.FlatType = flatType; },
            propertyData,
            QuestionFlowStepOld.FlatType,
            entryPoint);
    }
    
    public QuestionFlowStepOld UpdateYearBuilt(
        PropertyData propertyData,
        YearBuilt? yearBuilt,
        QuestionFlowStepOld? entryPoint)
    {
        return UpdatePropertyData(
            p => { p.YearBuilt = yearBuilt; },
            propertyData,
            QuestionFlowStepOld.HomeAge,
            entryPoint);
    }
    
    public QuestionFlowStepOld UpdateWallConstruction(
        PropertyData propertyData,
        WallConstruction? wallConstruction,
        QuestionFlowStepOld? entryPoint)
    {
        return UpdatePropertyData(
            p => { p.WallConstruction = wallConstruction; },
            propertyData,
            QuestionFlowStepOld.WallConstruction,
            entryPoint);
    }
    
    public QuestionFlowStepOld UpdateCavityWallInsulation(
        PropertyData propertyData,
        CavityWallsInsulated? cavityWallsInsulated,
        QuestionFlowStepOld? entryPoint)
    {
        return UpdatePropertyData(
            p => { p.CavityWallsInsulated = cavityWallsInsulated; },
            propertyData,
            QuestionFlowStepOld.CavityWallsInsulated,
            entryPoint);
    }
    
    public QuestionFlowStepOld UpdateSolidWallInsulation(
        PropertyData propertyData,
        SolidWallsInsulated? solidWallsInsulated,
        QuestionFlowStepOld? entryPoint)
    {
        return UpdatePropertyData(
            p => { p.SolidWallsInsulated = solidWallsInsulated; },
            propertyData,
            QuestionFlowStepOld.SolidWallsInsulated,
            entryPoint);
    }
    
    public QuestionFlowStepOld UpdateFloorConstruction(
        PropertyData propertyData,
        FloorConstruction? floorConstruction,
        QuestionFlowStepOld? entryPoint)
    {
        return UpdatePropertyData(
            p => { p.FloorConstruction = floorConstruction; },
            propertyData,
            QuestionFlowStepOld.FloorConstruction,
            entryPoint);
    }
    
    public QuestionFlowStepOld UpdateFloorInsulated(
        PropertyData propertyData,
        FloorInsulated? floorInsulated,
        QuestionFlowStepOld? entryPoint)
    {
        return UpdatePropertyData(
            p => { p.FloorInsulated = floorInsulated; },
            propertyData,
            QuestionFlowStepOld.FloorInsulated,
            entryPoint);
    }
    
    public QuestionFlowStepOld UpdateRoofConstruction(
        PropertyData propertyData,
        RoofConstruction? roofConstruction,
        QuestionFlowStepOld? entryPoint)
    {
        return UpdatePropertyData(
            p => { p.RoofConstruction = roofConstruction; },
            propertyData,
            QuestionFlowStepOld.RoofConstruction,
            entryPoint);
    }
    
    public QuestionFlowStepOld UpdateLoftSpace(
        PropertyData propertyData,
        LoftSpace? loftSpace,
        QuestionFlowStepOld? entryPoint)
    {
        return UpdatePropertyData(
            p => { p.LoftSpace = loftSpace; },
            propertyData,
            QuestionFlowStepOld.LoftSpace,
            entryPoint);
    }
    
    public QuestionFlowStepOld UpdateLoftAccess(
        PropertyData propertyData,
        LoftAccess? loftAccess,
        QuestionFlowStepOld? entryPoint)
    {
        return UpdatePropertyData(
            p => { p.LoftAccess = loftAccess; },
            propertyData,
            QuestionFlowStepOld.LoftAccess,
            entryPoint);
    }
    
    public QuestionFlowStepOld UpdateRoofInsulated(
        PropertyData propertyData,
        RoofInsulated? roofInsulated,
        QuestionFlowStepOld? entryPoint)
    {
        return UpdatePropertyData(
            p => { p.RoofInsulated = roofInsulated; },
            propertyData,
            QuestionFlowStepOld.RoofInsulated,
            entryPoint);
    }
    
    public QuestionFlowStepOld UpdateGlazingType(
        PropertyData propertyData,
        GlazingType? glazingType,
        QuestionFlowStepOld? entryPoint)
    {
        return UpdatePropertyData(
            p => { p.GlazingType = glazingType; },
            propertyData,
            QuestionFlowStepOld.GlazingType,
            entryPoint);
    }
    
    public QuestionFlowStepOld UpdateHasOutdoorSpace(
        PropertyData propertyData,
        HasOutdoorSpace? hasOutdoorSpace,
        QuestionFlowStepOld? entryPoint)
    {
        return UpdatePropertyData(
            p => { p.HasOutdoorSpace = hasOutdoorSpace; },
            propertyData,
            QuestionFlowStepOld.OutdoorSpace,
            entryPoint);
    }
    
    public QuestionFlowStepOld UpdateHeatingType(
        PropertyData propertyData,
        HeatingType? heatingType,
        QuestionFlowStepOld? entryPoint)
    {
        return UpdatePropertyData(
            p => { p.HeatingType = heatingType; },
            propertyData,
            QuestionFlowStepOld.HeatingType,
            entryPoint);
    }
    
    public QuestionFlowStepOld UpdateOtherHeatingType(
        PropertyData propertyData,
        OtherHeatingType? otherHeatingType,
        QuestionFlowStepOld? entryPoint)
    {
        return UpdatePropertyData(
            p => { p.OtherHeatingType = otherHeatingType; },
            propertyData,
            QuestionFlowStepOld.OtherHeatingType,
            entryPoint);
    }
    
    public QuestionFlowStepOld UpdateHasHotWaterCylinder(
        PropertyData propertyData,
        HasHotWaterCylinder? hasHotWaterCylinder,
        QuestionFlowStepOld? entryPoint)
    {
        return UpdatePropertyData(
            p => { p.HasHotWaterCylinder = hasHotWaterCylinder; },
            propertyData,
            QuestionFlowStepOld.HotWaterCylinder,
            entryPoint);
    }
    
    public QuestionFlowStepOld UpdateNumberOfOccupants(
        PropertyData propertyData,
        int? numberOfOccupants,
        QuestionFlowStepOld? entryPoint)
    {
        return UpdatePropertyData(
            p => { p.NumberOfOccupants = numberOfOccupants; },
            propertyData,
            QuestionFlowStepOld.NumberOfOccupants,
            entryPoint);
    }
    
    public QuestionFlowStepOld UpdateHeatingPattern(
        PropertyData propertyData,
        HeatingPattern? heatingPattern,
        int? hoursOfHeatingMorning,
        int? hoursOfHeatingEvening,
        QuestionFlowStepOld? entryPoint)
    {
        return UpdatePropertyData(
            p =>
            {
                p.HeatingPattern = heatingPattern;
                p.HoursOfHeatingMorning = hoursOfHeatingMorning;
                p.HoursOfHeatingEvening = hoursOfHeatingEvening;
            },
            propertyData,
            QuestionFlowStepOld.HeatingPattern,
            entryPoint);
    }
    
    public QuestionFlowStepOld UpdateTemperature(
        PropertyData propertyData,
        decimal? temperature,
        QuestionFlowStepOld? entryPoint)
    {
        return UpdatePropertyData(
            p => { p.Temperature = temperature; },
            propertyData,
            QuestionFlowStepOld.Temperature,
            entryPoint);
    }
    
    private QuestionFlowStepOld UpdatePropertyData(
        Action<PropertyData> update,
        PropertyData propertyData,
        QuestionFlowStepOld currentPage,
        QuestionFlowStepOld? entryPoint = null)
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
        
        var nextStep = questionFlowServiceOld.NextStep(currentPage, propertyData, entryPoint);
            
        // If the user is going back to the answer summary page or the check your unchangeable answers page then they
        // finished editing and we can get rid of the old answers
        if ((entryPoint is not null || propertyData.HasSeenRecommendations) &&
            (nextStep == QuestionFlowStepOld.AnswerSummary ||
             nextStep == QuestionFlowStepOld.CheckYourUnchangeableAnswers))
        {
            propertyData.CommitEdits();
        }
        
        return nextStep;
    }
}
