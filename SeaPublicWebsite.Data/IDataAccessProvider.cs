using SeaPublicWebsite.BusinessLogic.Models;

namespace SeaPublicWebsite.Data;

public interface IDataAccessProvider
{
    Task AddPropertyDataAsync(PropertyData propertyData);
    Task UpdatePropertyDataAsync(PropertyData propertyData);
    Task<PropertyData> GetPropertyDataAsync(string reference);
    Task<bool> PropertyDataExistsAsync(string reference);
}