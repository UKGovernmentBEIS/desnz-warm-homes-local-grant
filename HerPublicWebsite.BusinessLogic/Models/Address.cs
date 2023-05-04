namespace HerPublicWebsite.BusinessLogic.Models;

public record Address {
  public string DisplayAddress {get; set;}
  public string AddressLine1 {get; set;}
  public string AddressLine2 {get; set;}
  public string AddressLine3 {get; set;}
  public string AddressTown {get; set;}
  public string AddressCounty {get; set;}
  public string AddressPostcode {get; set;}
  public string Uprn { get; set; }
  public int LocalCustodianCode { get; set; }
}
