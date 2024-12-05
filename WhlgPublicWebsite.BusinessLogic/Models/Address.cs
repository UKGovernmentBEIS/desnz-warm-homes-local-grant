namespace WhlgPublicWebsite.BusinessLogic.Models;

public record Address {
  public string AddressLine1 {get; set;}
  public string AddressLine2 {get; set;}
  public string Town {get; set;}
  public string County { get; set; }
  public string Postcode {get; set;}
  public string Uprn { get; set; }
  public string LocalCustodianCode { get; set; }

  public string DisplayAddress
  {
      get
      {
          var addressParts = new[]
              {
                  AddressLine1,
                  AddressLine2,
                  Town,
                  County,
                  Postcode
              }
              .Where(s => !string.IsNullOrWhiteSpace(s));

          return string.Join(", ", addressParts);
      }
  }
}
