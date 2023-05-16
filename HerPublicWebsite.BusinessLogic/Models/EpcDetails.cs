using HerPublicWebsite.BusinessLogic.Models.Enums;

namespace HerPublicWebsite.BusinessLogic.Models;

public class EpcDetails
{
    public string AddressLine1 { get; set; }
    public string AddressLine2 { get; set; }
    public string AddressLine3 { get; set; }
    public string AddressLine4 { get; set; }
    public string AddressTown { get; set; }
    public string AddressPostcode { get; set; }
    public DateTime? LodgementDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public EpcRating? EpcRating { get; set; }
    public PropertyType? PropertyType { get; set; }
    public HouseType? HouseType { get; set; }
    public BungalowType? BungalowType { get; set; }
    public FlatType? FlatType { get; set; }
}
