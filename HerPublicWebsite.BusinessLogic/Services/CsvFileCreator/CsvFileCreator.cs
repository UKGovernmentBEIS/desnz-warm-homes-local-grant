using System.Globalization;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using HerPublicWebsite.BusinessLogic.Models;
using HerPublicWebsite.BusinessLogic.Models.Enums;

namespace HerPublicWebsite.BusinessLogic.Services.CsvFileCreator;

public class CsvFileCreator
{
    public MemoryStream CreateFileData(IEnumerable<ReferralRequest> referralRequests)
    {
        var rows = referralRequests.Select(rr => new CsvRow(rr));

        var csvConfiguration = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            InjectionOptions = InjectionOptions.Strip
        };

        using var writeableMemoryStream = new MemoryStream();
        using var streamWriter = new StreamWriter(writeableMemoryStream, Encoding.UTF8);
        using var csvWriter = new CsvWriter(streamWriter, csvConfiguration);
        {
            csvWriter.WriteRecords(rows);
            csvWriter.Flush();
            return new MemoryStream(writeableMemoryStream.GetBuffer(), 0, (int)writeableMemoryStream.Length, false);
        }
    }

    private class CsvRow
    {
        [Index(0)]
        [Name("Referral date")]
        public string ReferralDate { get; set; }
        
        [Index(1)]
        [Name("Referral code")]
        public string ReferralCode { get; set; }
        
        [Index(2)]
        public string Name { get; set; }
        
        [Index(3)]
        public string Email { get; set; }
        
        [Index(4)]
        public string Telephone { get; set; }

        [Index(5)]
        public string Address1 { get; set; }
        
        [Index(6)]
        public string Address2 { get; set; }
        
        [Index(7)]
        public string Town { get; set; }
        
        [Index(8)]
        public string County { get; set; }
        
        [Index(9)]
        public string Postcode { get; set; }
        
        [Index(10)]
        [Name("UPRN")]
        public string Uprn { get; set; }
        
        [Index(11)]
        [Name("EPC Band")]
        public EpcRating EpcBand { get; set; }
        
        [Index(12)]
        [Name("EPC Lodgement Date")]
        public string EpcLodgementDate { get; set; }
        
        [Index(13)]
        [Name("Is off gas grid")]
        [BooleanTrueValues("yes")]
        [BooleanFalseValues("no")]
        public string OffGasGrid { get; set; }
        
        [Index(14)]
        [Name("Household income band")]
        public string HouseholdIncome { get; set; }
        
        [Index(15)]
        [Name("Is eligible postcode")]
        [BooleanTrueValues("yes")]
        [BooleanFalseValues("no")]
        public bool EligiblePostcode { get; set; }
        
        [Index(16)]
        public string Tenure { get; set; }

        public CsvRow(ReferralRequest request)
        {
            ReferralDate = request.RequestDate.ToString("yyyy-MM-dd HH:mm:ss");
            ReferralCode = request.ReferralCode;
            Name = request.FullName;
            Email = request.ContactEmailAddress;
            Telephone = request.ContactTelephone;
            Address1 = request.AddressLine1;
            Address2 = request.AddressLine2;
            Town = request.AddressTown;
            County = request.AddressCounty;
            Postcode = request.AddressPostcode;
            Uprn = request.Uprn;
            EpcBand = request.EpcRating;
            EpcLodgementDate = request.EpcLodgementDate?.ToString("yyyy-MM-dd HH:mm:ss");
            OffGasGrid = request.HasGasBoiler switch
            {
                HasGasBoiler.No => "yes",
                HasGasBoiler.Yes => "no",
                _ => throw new ArgumentOutOfRangeException("request.HasGasBoiler", "Unrecognised HasGasBoiler value: " + request.HasGasBoiler)
            };
            HouseholdIncome = request.IncomeBand switch
            {
                IncomeBand.UnderOrEqualTo31000 => "Below £31k",
                IncomeBand.GreaterThan31000 => "£31k or above",
                _ => throw new ArgumentOutOfRangeException("request.IncomeBand", "Unrecognised IncomeBand value: " + request.IncomeBand)
            };
            EligiblePostcode = request.IsLsoaProperty;
            Tenure = "Owner";
        }
    }
}
