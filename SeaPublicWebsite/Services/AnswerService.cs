using System;
using System.Threading.Tasks;
using SeaPublicWebsite.BusinessLogic;
using SeaPublicWebsite.BusinessLogic.Models;
using SeaPublicWebsite.BusinessLogic.Models.Enums;
using SeaPublicWebsite.DataStores;

namespace SeaPublicWebsite.Services;

public class AnswerService
{
    private readonly PropertyDataStore propertyDataStore;
    private readonly PropertyDataUpdater propertyDataUpdater;
    
    public AnswerService(
        PropertyDataStore propertyDataStore,
        PropertyDataUpdater propertyDataUpdater)
    {
        this.propertyDataStore = propertyDataStore;
        this.propertyDataUpdater = propertyDataUpdater;
    }
    
    public async Task<QuestionFlowStep> UpdateOwnershipStatus(string reference, OwnershipStatus? ownershipStatus)
    {
        return await UpdatePropertyDataAsync(
            (b, p) => b.UpdateOwnershipStatus(p, ownershipStatus),
            reference);
    }
    
    public async Task<QuestionFlowStep> UpdateCountry(string reference, Country? country)
    {
        return await UpdatePropertyDataAsync(
            (b, p) => b.UpdateCountry(p, country),
            reference);
    }
    
    public async Task<QuestionFlowStep> UpdateSearchForEpc(string reference, SearchForEpc? searchForEpc)
    {
        return await UpdatePropertyDataAsync(
            (b, p) => b.UpdateSearchForEpc(p, searchForEpc),
            reference);
    }
    
    public async Task<QuestionFlowStep> SetEpc(string reference, string epcId)
    {
        return await UpdatePropertyDataAsync(
            async (b, p) => await b.SetEpcAsync(p, epcId),
            reference);
    }

    public async Task<QuestionFlowStep> ConfirmEpcDetails(string reference, EpcDetailsConfirmed? confirmed)
    {
        return await UpdatePropertyDataAsync(
            (b, p) => b.ConfirmEpcDetails(p, confirmed),
            reference);
    }
    
    public async Task<QuestionFlowStep> UpdatePropertyType(
        string reference,
        PropertyType? propertyType,
        QuestionFlowStep? entryPoint)
    {
        return await UpdatePropertyDataAsync(
            (b, p) => b.UpdatePropertyType(p, propertyType, entryPoint),
            reference);
    }
    
    public async Task<QuestionFlowStep> UpdateHouseType(
        string reference,
        HouseType? houseType,
        QuestionFlowStep? entryPoint)
    {
        return await UpdatePropertyDataAsync(
            (b, p) => b.UpdateHouseType(p, houseType, entryPoint),
            reference);
    }
    
    public async Task<QuestionFlowStep> UpdateBungalowType(
        string reference,
        BungalowType? bungalowType,
        QuestionFlowStep? entryPoint)
    {
        return await UpdatePropertyDataAsync(
            (b, p) => b.UpdateBungalowType(p, bungalowType, entryPoint),
            reference);
    }
    
    public async Task<QuestionFlowStep> UpdateFlatType(
        string reference,
        FlatType? flatType,
        QuestionFlowStep? entryPoint)
    {
        return await UpdatePropertyDataAsync(
            (b, p) => b.UpdateFlatType(p, flatType, entryPoint),
            reference);
    }
    
    public async Task<QuestionFlowStep> UpdateYearBuilt(
        string reference,
        YearBuilt? yearBuilt,
        QuestionFlowStep? entryPoint)
    {
        return await UpdatePropertyDataAsync(
            (b, p) => b.UpdateYearBuilt(p, yearBuilt, entryPoint),
            reference);
    }
    
    public async Task<QuestionFlowStep> UpdateWallConstruction(
        string reference,
        WallConstruction? wallConstruction,
        QuestionFlowStep? entryPoint)
    {
        return await UpdatePropertyDataAsync(
            (b, p) => b.UpdateWallConstruction(p, wallConstruction, entryPoint),
            reference);
    }
    
    public async Task<QuestionFlowStep> UpdateCavityWallInsulation(
        string reference,
        CavityWallsInsulated? cavityWallsInsulated,
        QuestionFlowStep? entryPoint)
    {
        return await UpdatePropertyDataAsync(
            (b, p) => b.UpdateCavityWallInsulation(p, cavityWallsInsulated, entryPoint),
            reference);
    }
    
    public async Task<QuestionFlowStep> UpdateSolidWallInsulation(
        string reference,
        SolidWallsInsulated? solidWallsInsulated,
        QuestionFlowStep? entryPoint)
    {
        return await UpdatePropertyDataAsync(
            (b, p) => b.UpdateSolidWallInsulation(p, solidWallsInsulated, entryPoint),
            reference);
    }
    
    public async Task<QuestionFlowStep> UpdateFloorConstruction(
        string reference,
        FloorConstruction? floorConstruction,
        QuestionFlowStep? entryPoint)
    {
        return await UpdatePropertyDataAsync(
            (b, p) => b.UpdateFloorConstruction(p, floorConstruction, entryPoint),
            reference);
    }
    
    public async Task<QuestionFlowStep> UpdateFloorInsulated(
        string reference,
        FloorInsulated? floorInsulated,
        QuestionFlowStep? entryPoint)
    {
        return await UpdatePropertyDataAsync(
            (b, p) => b.UpdateFloorInsulated(p, floorInsulated, entryPoint),
            reference);
    }
    
    public async Task<QuestionFlowStep> UpdateRoofConstruction(
        string reference,
        RoofConstruction? roofConstruction,
        QuestionFlowStep? entryPoint)
    {
        return await UpdatePropertyDataAsync(
            (b, p) => b.UpdateRoofConstruction(p, roofConstruction, entryPoint),
            reference);
    }
    
    public async Task<QuestionFlowStep> UpdateLoftSpace(
        string reference,
        LoftSpace? loftSpace,
        QuestionFlowStep? entryPoint)
    {
        return await UpdatePropertyDataAsync(
            (b, p) => b.UpdateLoftSpace(p, loftSpace, entryPoint),
            reference);
    }
    
    public async Task<QuestionFlowStep> UpdateLoftAccess(
        string reference,
        LoftAccess? loftAccess,
        QuestionFlowStep? entryPoint)
    {
        return await UpdatePropertyDataAsync(
            (b, p) => b.UpdateLoftAccess(p, loftAccess, entryPoint),
            reference);
    }
    
    public async Task<QuestionFlowStep> UpdateRoofInsulated(
        string reference,
        RoofInsulated? roofInsulated,
        QuestionFlowStep? entryPoint)
    {
        return await UpdatePropertyDataAsync(
            (b, p) => b.UpdateRoofInsulated(p, roofInsulated, entryPoint),
            reference);
    }
    
    public async Task<QuestionFlowStep> UpdateGlazingType(
        string reference,
        GlazingType? glazingType,
        QuestionFlowStep? entryPoint)
    {
        return await UpdatePropertyDataAsync(
            (b, p) => b.UpdateGlazingType(p, glazingType, entryPoint),
            reference);
    }
    
    public async Task<QuestionFlowStep> UpdateHasOutdoorSpace(
        string reference,
        HasOutdoorSpace? hasOutdoorSpace,
        QuestionFlowStep? entryPoint)
    {
        return await UpdatePropertyDataAsync(
            (b, p) => b.UpdateHasOutdoorSpace(p, hasOutdoorSpace, entryPoint),
            reference);
    }
    
    public async Task<QuestionFlowStep> UpdateHeatingType(
        string reference,
        HeatingType? heatingType,
        QuestionFlowStep? entryPoint)
    {
        return await UpdatePropertyDataAsync(
            (b, p) => b.UpdateHeatingType(p, heatingType, entryPoint),
            reference);
    }
    
    public async Task<QuestionFlowStep> UpdateOtherHeatingType(
        string reference,
        OtherHeatingType? otherHeatingType,
        QuestionFlowStep? entryPoint)
    {
        return await UpdatePropertyDataAsync(
            (b, p) => b.UpdateOtherHeatingType(p, otherHeatingType, entryPoint),
            reference);
    }
    
    public async Task<QuestionFlowStep> UpdateHasHotWaterCylinder(
        string reference,
        HasHotWaterCylinder? hasHotWaterCylinder,
        QuestionFlowStep? entryPoint)
    {
        return await UpdatePropertyDataAsync(
            (b, p) => b.UpdateHasHotWaterCylinder(p, hasHotWaterCylinder, entryPoint),
            reference);
    }
    
    public async Task<QuestionFlowStep> UpdateNumberOfOccupants(
        string reference,
        int? numberOfOccupants,
        QuestionFlowStep? entryPoint)
    {
        return await UpdatePropertyDataAsync(
            (b, p) => b.UpdateNumberOfOccupants(p, numberOfOccupants, entryPoint),
            reference);
    }
    
    public async Task<QuestionFlowStep> UpdateHeatingPattern(
        string reference,
        HeatingPattern? heatingPattern,
        int? hoursOfHeatingMorning,
        int? hoursOfHeatingEvening,
        QuestionFlowStep? entryPoint)
    {
        return await UpdatePropertyDataAsync(
            (b, p) => b.UpdateHeatingPattern(p, heatingPattern, hoursOfHeatingMorning, hoursOfHeatingEvening, entryPoint),
            reference);
    }
    
    public async Task<QuestionFlowStep> UpdateTemperature(
        string reference,
        decimal? temperature,
        QuestionFlowStep? entryPoint)
    {
        return await UpdatePropertyDataAsync(
            (b, p) => b.UpdateTemperature(p, temperature, entryPoint),
            reference);
    }
    
    private async Task<QuestionFlowStep> UpdatePropertyDataAsync(
        Func<PropertyDataUpdater, PropertyData, QuestionFlowStep> update,
        string reference)
    {
        var propertyData = await propertyDataStore.LoadPropertyDataAsync(reference);

        var nextStep = update(propertyDataUpdater, propertyData);
        
        await propertyDataStore.SavePropertyDataAsync(propertyData);

        return nextStep;
    }
    
    private async Task<QuestionFlowStep> UpdatePropertyDataAsync(
        Func<PropertyDataUpdater, PropertyData, Task<QuestionFlowStep>> update,
        string reference)
    {
        var propertyData = await propertyDataStore.LoadPropertyDataAsync(reference);

        var nextStep = await update(propertyDataUpdater, propertyData);
        
        await propertyDataStore.SavePropertyDataAsync(propertyData);

        return nextStep;
    }
}