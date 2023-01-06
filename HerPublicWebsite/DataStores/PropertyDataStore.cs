using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using HerPublicWebsite.BusinessLogic.Models;
using HerPublicWebsite.Data;
using HerPublicWebsite.ErrorHandling;
using HerPublicWebsite.Helpers;

namespace HerPublicWebsite.DataStores;

public class PropertyDataStore
{
    private readonly IDataAccessProvider dataAccessProvider;
    private readonly ILogger<PropertyDataStore> logger;
    private const int MaxRetries = 10;
    private const int SleepMilliSeconds = 500;

    public PropertyDataStore(IDataAccessProvider dataAccessProvider, ILogger<PropertyDataStore> logger)
    {
        this.dataAccessProvider = dataAccessProvider;
        this.logger = logger;
    }
    
    public async Task<PropertyData> LoadPropertyDataAsync(string reference)
    {
        var data = await dataAccessProvider.GetPropertyDataAsync(reference.ToUpper());
        
        if (data == null)
        {
            throw new PropertyReferenceNotFoundException
            {
                Reference = reference
            };
        }
        
        // Recommendations need to be in a consistent order to allow navigation between them.
        data.PropertyRecommendations.Sort(Comparer<PropertyRecommendation>.Create((p1, p2) => p1.Key.CompareTo(p2.Key)));
        
        return data;
    }

    public async Task<bool> IsReferenceValidAsync(string reference)
    {
        return await dataAccessProvider.PropertyDataExistsAsync(reference);
    }

    public async Task SavePropertyDataAsync(PropertyData propertyData)
    {
        try
        {
            await dataAccessProvider.UpdatePropertyDataAsync(propertyData);
        }
        catch (DbUpdateConcurrencyException e)
        {
            // Most likely reason for this is that the user double-clicked a form submit button so just continue,
            // the previous request should have updated the DB already.
            logger.LogWarning($"DbUpdateConcurrencyException caught. This probably means that a user double-clicked a form submit. {e.Message}");
        }
    }

    public async Task<PropertyData> CreateNewPropertyDataAsync()
    {
        var saveCount = 0;
        var attemptedReferences = new List<string>();

        while (saveCount <= MaxRetries)
        {
            PropertyData propertyData = new()
            {
                Reference = RandomHelper.Generate8CharacterReference()
            };
            attemptedReferences.Add(propertyData.Reference);
            
            try
            {
                await dataAccessProvider.AddPropertyDataAsync(propertyData);
                return propertyData;
            }
            catch (Exception)
            {
                // Just retry
                logger.LogWarning("Failed to create new property data row with reference " + propertyData.Reference);
                await Task.Delay(SleepMilliSeconds);
            }
            saveCount++;
        }

        throw new Exception("Failed to create new property data row. Tried references: " + string.Join(',', attemptedReferences));
    }
}
