namespace HerPublicWebsite.BusinessLogic.Models;

public record Address {
  public string DisplayAddress {get; set;}
  public string AddressLine1 {get; set;}
  public string AddressLine2 {get; set;}
  public string Town {get; set;}
  public string County { get; set; }
  public string Postcode {get; set;}
  public string Uprn { get; set; }
  public int LocalCustodianCode { get; set; }
}
