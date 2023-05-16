using System.Globalization;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration.Attributes;
using HerPublicWebsite.BusinessLogic.Models;
using HerPublicWebsite.BusinessLogic.Models.Enums;

namespace HerPublicWebsite.BusinessLogic.Services.CsvFileCreator;

public class CsvFileCreator
{
    public MemoryStream CreateFileData(IEnumerable<ReferralRequest> referralRequests)
    {
        var rows = referralRequests.Select(rr => new CsvRow(rr));
        
        using var writeableMemoryStream = new MemoryStream();
        using var streamWriter = new StreamWriter(writeableMemoryStream, Encoding.UTF8);
        using var csvWriter = new CsvWriter(streamWriter, CultureInfo.InvariantCulture);
        {
            csvWriter.WriteRecords(rows);
            csvWriter.Flush();
            return new MemoryStream(writeableMemoryStream.GetBuffer(), 0, (int)writeableMemoryStream.Length, false);
        }
    }

    private class CsvRow
    {
        [Index(0)]
        public string Name { get; set; }
        
        [Index(1)]
        public string Email { get; set; }
        
        [Index(2)]
        public string Telephone { get; set; }
        
        [Index(3)]
        [Name("Preferred contact method")]
        public ContactPreference ContactPreferrence { get; set; }
        
        [Index(4)]
        public string Address1 { get; set; }
        
        [Index(5)]
        public string Address2 { get; set; }
        
        [Index(6)]
        public string Town { get; set; }
        
        [Index(7)]
        public string County { get; set; }
        
        [Index(8)]
        public string Postcode { get; set; }
        
        [Index(9)]
        public string Uprn { get; set; }
        
        [Index(10)]
        [Name("EPC Band")]
        public EpcRating EpcBand { get; set; }
        
        [Index(11)]
        [Name("Is off gas grid")]
        [BooleanTrueValues("yes")]
        [BooleanFalseValues("no")]
        public string OffGasGrid { get; set; }
        
        [Index(12)]
        [Name("Household income band")]
        public string HouseholdIncome { get; set; }
        
        [Index(13)]
        [Name("Is eligible postcode")]
        [BooleanTrueValues("yes")]
        [BooleanFalseValues("no")]
        public bool EligiblePostcode { get; set; }
        
        [Index(14)]
        public string Tenure { get; set; }
        
        public CsvRow(ReferralRequest request)
        {
            Name = request.ContactDetails.FullName;
            Email = request.ContactDetails.Email;
            Telephone = request.ContactDetails.Telephone;
            ContactPreferrence = request.ContactDetails.ContactPreference;
            Address1 = request.AddressLine1;
            Address2 = request.AddressLine2;
            Town = request.AddressTown;
            County = request.AddressCounty;
            Postcode = request.AddressPostcode;
            Uprn = request.Uprn;
            EpcBand = request.EpcRating;
            OffGasGrid = request.HasGasBoiler switch
            {
                HasGasBoiler.No => "yes",
                HasGasBoiler.Unknown => "unknown",
                HasGasBoiler.Yes => "no",
                _ => throw new ArgumentOutOfRangeException("request.HasGasBoiler", "Unrecognised HasGasBoiler value: " + request.HasGasBoiler)
            };
            HouseholdIncome = request.IncomeBand switch
            {
                IncomeBand.Under31000 => "Below £31k",
                IncomeBand.GreaterOrEqualTo31000 => "£31k or above",
                _ => throw new ArgumentOutOfRangeException("request.IncomeBand", "Unrecognised IncomeBand value: " + request.IncomeBand)
            };
            EligiblePostcode = request.IsLsoaProperty;
            Tenure = "Owner";
        }
    }
}
