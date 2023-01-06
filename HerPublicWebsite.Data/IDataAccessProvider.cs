using HerPublicWebsite.BusinessLogic.Models;

namespace HerPublicWebsite.Data;

public interface IDataAccessProvider
{
    Task AddPropertyDataAsync(PropertyData propertyData);
    Task UpdatePropertyDataAsync(PropertyData propertyData);
    Task<PropertyData> GetPropertyDataAsync(string reference);
    Task<bool> PropertyDataExistsAsync(string reference);
}