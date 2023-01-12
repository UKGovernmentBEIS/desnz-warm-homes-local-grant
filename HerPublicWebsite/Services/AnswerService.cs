using System;
using System.Threading.Tasks;
using HerPublicWebsite.BusinessLogic;
using HerPublicWebsite.BusinessLogic.Models;
using HerPublicWebsite.BusinessLogic.Models.Enums;
using HerPublicWebsite.DataStores;

namespace HerPublicWebsite.Services;

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
    
    public async Task<QuestionFlowStepOld> UpdateOwnershipStatus(string reference, OwnershipStatus? ownershipStatus)
    {
        return await UpdatePropertyDataAsync(
            (b, p) => b.UpdateOwnershipStatus(p, ownershipStatus),
            reference);
    }
    
    public async Task<QuestionFlowStepOld> UpdateCountry(string reference, Country? country)
    {
        return await UpdatePropertyDataAsync(
            (b, p) => b.UpdateCountry(p, country),
            reference);
    }
    
    public async Task<QuestionFlowStepOld> UpdateSearchForEpc(string reference, SearchForEpc? searchForEpc)
    {
        return await UpdatePropertyDataAsync(
            (b, p) => b.UpdateSearchForEpc(p, searchForEpc),
            reference);
    }
    
    public async Task<QuestionFlowStepOld> SetEpc(string reference, string epcId)
    {
        return await UpdatePropertyDataAsync(
            async (b, p) => await b.SetEpcAsync(p, epcId),
            reference);
    }

    public async Task<QuestionFlowStepOld> ConfirmEpcDetails(string reference, EpcDetailsConfirmed? confirmed)
    {
        return await UpdatePropertyDataAsync(
            (b, p) => b.ConfirmEpcDetails(p, confirmed),
            reference);
    }
    
    public async Task<QuestionFlowStepOld> UpdatePropertyType(
        string reference,
        PropertyType? propertyType,
        QuestionFlowStepOld? entryPoint)
    {
        return await UpdatePropertyDataAsync(
            (b, p) => b.UpdatePropertyType(p, propertyType, entryPoint),
            reference);
    }
    
    public async Task<QuestionFlowStepOld> UpdateHouseType(
        string reference,
        HouseType? houseType,
        QuestionFlowStepOld? entryPoint)
    {
        return await UpdatePropertyDataAsync(
            (b, p) => b.UpdateHouseType(p, houseType, entryPoint),
            reference);
    }
    
    public async Task<QuestionFlowStepOld> UpdateBungalowType(
        string reference,
        BungalowType? bungalowType,
        QuestionFlowStepOld? entryPoint)
    {
        return await UpdatePropertyDataAsync(
            (b, p) => b.UpdateBungalowType(p, bungalowType, entryPoint),
            reference);
    }
    
    public async Task<QuestionFlowStepOld> UpdateFlatType(
        string reference,
        FlatType? flatType,
        QuestionFlowStepOld? entryPoint)
    {
        return await UpdatePropertyDataAsync(
            (b, p) => b.UpdateFlatType(p, flatType, entryPoint),
            reference);
    }
    
    public async Task<QuestionFlowStepOld> UpdateYearBuilt(
        string reference,
        YearBuilt? yearBuilt,
        QuestionFlowStepOld? entryPoint)
    {
        return await UpdatePropertyDataAsync(
            (b, p) => b.UpdateYearBuilt(p, yearBuilt, entryPoint),
            reference);
    }
    
    public async Task<QuestionFlowStepOld> UpdateWallConstruction(
        string reference,
        WallConstruction? wallConstruction,
        QuestionFlowStepOld? entryPoint)
    {
        return await UpdatePropertyDataAsync(
            (b, p) => b.UpdateWallConstruction(p, wallConstruction, entryPoint),
            reference);
    }
    
    public async Task<QuestionFlowStepOld> UpdateCavityWallInsulation(
        string reference,
        CavityWallsInsulated? cavityWallsInsulated,
        QuestionFlowStepOld? entryPoint)
    {
        return await UpdatePropertyDataAsync(
            (b, p) => b.UpdateCavityWallInsulation(p, cavityWallsInsulated, entryPoint),
            reference);
    }
    
    public async Task<QuestionFlowStepOld> UpdateSolidWallInsulation(
        string reference,
        SolidWallsInsulated? solidWallsInsulated,
        QuestionFlowStepOld? entryPoint)
    {
        return await UpdatePropertyDataAsync(
            (b, p) => b.UpdateSolidWallInsulation(p, solidWallsInsulated, entryPoint),
            reference);
    }
    
    public async Task<QuestionFlowStepOld> UpdateFloorConstruction(
        string reference,
        FloorConstruction? floorConstruction,
        QuestionFlowStepOld? entryPoint)
    {
        return await UpdatePropertyDataAsync(
            (b, p) => b.UpdateFloorConstruction(p, floorConstruction, entryPoint),
            reference);
    }
    
    public async Task<QuestionFlowStepOld> UpdateFloorInsulated(
        string reference,
        FloorInsulated? floorInsulated,
        QuestionFlowStepOld? entryPoint)
    {
        return await UpdatePropertyDataAsync(
            (b, p) => b.UpdateFloorInsulated(p, floorInsulated, entryPoint),
            reference);
    }
    
    public async Task<QuestionFlowStepOld> UpdateRoofConstruction(
        string reference,
        RoofConstruction? roofConstruction,
        QuestionFlowStepOld? entryPoint)
    {
        return await UpdatePropertyDataAsync(
            (b, p) => b.UpdateRoofConstruction(p, roofConstruction, entryPoint),
            reference);
    }
    
    public async Task<QuestionFlowStepOld> UpdateLoftSpace(
        string reference,
        LoftSpace? loftSpace,
        QuestionFlowStepOld? entryPoint)
    {
        return await UpdatePropertyDataAsync(
            (b, p) => b.UpdateLoftSpace(p, loftSpace, entryPoint),
            reference);
    }
    
    public async Task<QuestionFlowStepOld> UpdateLoftAccess(
        string reference,
        LoftAccess? loftAccess,
        QuestionFlowStepOld? entryPoint)
    {
        return await UpdatePropertyDataAsync(
            (b, p) => b.UpdateLoftAccess(p, loftAccess, entryPoint),
            reference);
    }
    
    public async Task<QuestionFlowStepOld> UpdateRoofInsulated(
        string reference,
        RoofInsulated? roofInsulated,
        QuestionFlowStepOld? entryPoint)
    {
        return await UpdatePropertyDataAsync(
            (b, p) => b.UpdateRoofInsulated(p, roofInsulated, entryPoint),
            reference);
    }
    
    public async Task<QuestionFlowStepOld> UpdateGlazingType(
        string reference,
        GlazingType? glazingType,
        QuestionFlowStepOld? entryPoint)
    {
        return await UpdatePropertyDataAsync(
            (b, p) => b.UpdateGlazingType(p, glazingType, entryPoint),
            reference);
    }
    
    public async Task<QuestionFlowStepOld> UpdateHasOutdoorSpace(
        string reference,
        HasOutdoorSpace? hasOutdoorSpace,
        QuestionFlowStepOld? entryPoint)
    {
        return await UpdatePropertyDataAsync(
            (b, p) => b.UpdateHasOutdoorSpace(p, hasOutdoorSpace, entryPoint),
            reference);
    }
    
    public async Task<QuestionFlowStepOld> UpdateHeatingType(
        string reference,
        HeatingType? heatingType,
        QuestionFlowStepOld? entryPoint)
    {
        return await UpdatePropertyDataAsync(
            (b, p) => b.UpdateHeatingType(p, heatingType, entryPoint),
            reference);
    }
    
    public async Task<QuestionFlowStepOld> UpdateOtherHeatingType(
        string reference,
        OtherHeatingType? otherHeatingType,
        QuestionFlowStepOld? entryPoint)
    {
        return await UpdatePropertyDataAsync(
            (b, p) => b.UpdateOtherHeatingType(p, otherHeatingType, entryPoint),
            reference);
    }
    
    public async Task<QuestionFlowStepOld> UpdateHasHotWaterCylinder(
        string reference,
        HasHotWaterCylinder? hasHotWaterCylinder,
        QuestionFlowStepOld? entryPoint)
    {
        return await UpdatePropertyDataAsync(
            (b, p) => b.UpdateHasHotWaterCylinder(p, hasHotWaterCylinder, entryPoint),
            reference);
    }
    
    public async Task<QuestionFlowStepOld> UpdateNumberOfOccupants(
        string reference,
        int? numberOfOccupants,
        QuestionFlowStepOld? entryPoint)
    {
        return await UpdatePropertyDataAsync(
            (b, p) => b.UpdateNumberOfOccupants(p, numberOfOccupants, entryPoint),
            reference);
    }
    
    public async Task<QuestionFlowStepOld> UpdateHeatingPattern(
        string reference,
        HeatingPattern? heatingPattern,
        int? hoursOfHeatingMorning,
        int? hoursOfHeatingEvening,
        QuestionFlowStepOld? entryPoint)
    {
        return await UpdatePropertyDataAsync(
            (b, p) => b.UpdateHeatingPattern(p, heatingPattern, hoursOfHeatingMorning, hoursOfHeatingEvening, entryPoint),
            reference);
    }
    
    public async Task<QuestionFlowStepOld> UpdateTemperature(
        string reference,
        decimal? temperature,
        QuestionFlowStepOld? entryPoint)
    {
        return await UpdatePropertyDataAsync(
            (b, p) => b.UpdateTemperature(p, temperature, entryPoint),
            reference);
    }
    
    private async Task<QuestionFlowStepOld> UpdatePropertyDataAsync(
        Func<PropertyDataUpdater, PropertyData, QuestionFlowStepOld> update,
        string reference)
    {
        var propertyData = await propertyDataStore.LoadPropertyDataAsync(reference);

        var nextStep = update(propertyDataUpdater, propertyData);
        
        await propertyDataStore.SavePropertyDataAsync(propertyData);

        return nextStep;
    }
    
    private async Task<QuestionFlowStepOld> UpdatePropertyDataAsync(
        Func<PropertyDataUpdater, PropertyData, Task<QuestionFlowStepOld>> update,
        string reference)
    {
        var propertyData = await propertyDataStore.LoadPropertyDataAsync(reference);

        var nextStep = await update(propertyDataUpdater, propertyData);
        
        await propertyDataStore.SavePropertyDataAsync(propertyData);

        return nextStep;
    }
}