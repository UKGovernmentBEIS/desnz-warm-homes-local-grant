using System.Globalization;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using WhlgPublicWebsite.BusinessLogic.Models;
using WhlgPublicWebsite.BusinessLogic.Models.Enums;

namespace WhlgPublicWebsite.BusinessLogic.Services.CsvFileCreator;

public interface ICsvFileCreator
{
    public MemoryStream CreateReferralRequestFileData(IEnumerable<ReferralRequest> referralRequests);
    public MemoryStream CreateReferralRequestOverviewFileData(IEnumerable<ReferralRequest> referralRequests);
    public MemoryStream CreateLocalAuthorityReferralRequestFollowUpFileData(IEnumerable<ReferralRequest> referralRequests);
    public MemoryStream CreateConsortiumReferralRequestFollowUpFileData(IEnumerable<ReferralRequest> referralRequests);
    public MemoryStream CreatePendingReferralRequestFileData(IEnumerable<ReferralRequest> referralRequests);
    public MemoryStream CreatePerMonthLocalAuthorityReferralStatistics(IEnumerable<ReferralRequest> referralRequests);
    public MemoryStream CreatePerMonthConsortiumReferralStatistics(IEnumerable<ReferralRequest> referralRequests);
}

public class CsvFileCreator : ICsvFileCreator
{
    public MemoryStream CreateReferralRequestFileData(IEnumerable<ReferralRequest> referralRequests)
    {
        var rows = referralRequests.Select(rr => new CsvRowReferralRequest(rr));
        return GenerateCsvMemoryStreamFromFileRows(rows);
    }

    public MemoryStream CreateReferralRequestOverviewFileData(IEnumerable<ReferralRequest> referralRequests)
    {
        var rows = referralRequests.Select(rr => new CsvRowReferralCodes(rr));
        return GenerateCsvMemoryStreamFromFileRows(rows);
    }

    public MemoryStream CreateLocalAuthorityReferralRequestFollowUpFileData(IEnumerable<ReferralRequest> referralRequests)
    {
        var rows = referralRequests
            .GroupBy(rr => rr.CustodianCode)
            .Select(groupingByLa => new CsvRowLaDownloadInformation(groupingByLa));
        return GenerateCsvMemoryStreamFromFileRows(rows);
    }

    public MemoryStream CreateConsortiumReferralRequestFollowUpFileData(IEnumerable<ReferralRequest> referralRequests)
    {
        var rows = referralRequests
            .GroupBy(rr => rr.CustodianCode)
            .GroupBy(groupingByLa =>
                LocalAuthorityData.LocalAuthorityDetailsByCustodianCode[groupingByLa.Key].Consortium)
            .Where(groupingByConsortium => groupingByConsortium.Key is not null)
            .Select(groupingByConsortium =>
                {
                    var consortiumReferrals = groupingByConsortium.SelectMany(g => g).ToList();
                    var consortiumStatistics = new ConsortiumStatistics(consortiumReferrals);
                    return new CsvRowConsortiumDownloadInformationRow(groupingByConsortium.Key, consortiumStatistics);
                }
            );
        return GenerateCsvMemoryStreamFromFileRows(rows);
    }

    public MemoryStream CreatePendingReferralRequestFileData(IEnumerable<ReferralRequest> referralRequests)
    {
        var rows = referralRequests
            .Select(rr => new CsvRowPendingReferralRequest(rr));

        return GenerateCsvMemoryStreamFromFileRows(rows);
    }

    public MemoryStream CreatePerMonthLocalAuthorityReferralStatistics(IEnumerable<ReferralRequest> requests)
    {
        var requestsByCustodianCode = requests.GroupBy(r => r.CustodianCode);
        var rows = requestsByCustodianCode.Select(custodianCodeRequests =>
        {
            var localAuthorityDetails = LocalAuthorityData.LocalAuthorityDetailsByCustodianCode[custodianCodeRequests.Key];
            return new CsvRowLaPerMonthStatistics(
                name: localAuthorityDetails.Name,
                consortiumName: localAuthorityDetails.Consortium ?? "N/A",
                referralCount: custodianCodeRequests.Count(),
                firstReferralDate: custodianCodeRequests.Min(x => x.RequestDate)
            );
        });

        return GenerateCsvMemoryStreamFromFileRows(rows);
    }

    public MemoryStream CreatePerMonthConsortiumReferralStatistics(IEnumerable<ReferralRequest> requests)
    {
        var requestsByConsortium = requests.GroupBy(rr =>
                LocalAuthorityData.LocalAuthorityDetailsByCustodianCode[rr.CustodianCode].Consortium)
            .Where(group => group.Key != null);
        var rows = requestsByConsortium.Select(consortiumRequests =>
        {
            return new CsvRowConsortiumPerMonthStatistics(
                name: consortiumRequests.Key,
                referralCount: consortiumRequests.Count(),
                firstReferralDate: consortiumRequests.Min(x => x.RequestDate)
            );
        });
        return GenerateCsvMemoryStreamFromFileRows(rows);
    }

    private class CsvRowConsortiumPerMonthStatistics
    {
        [Index(0)]
        [Name("Consortium Name")]
        public string Name { get; set; }
        
        [Index(1)]
        [Name("Total WH:LG Referrals")]
        public int ReferralCount { get; set; }
        
        [Index(2)]
        [Name("Date of First Referral")]
        public string FirstReferralDate { get; set; }

        [Index(3)] 
        [Name("Months Since First Referral")]
        public int MonthsSinceFirstReferral { get; set; }

        [Index(4)] [Name("Referrals Per Month")]
        public string ReferralsPerMonth { get; set; }
        
        public CsvRowConsortiumPerMonthStatistics(string name, int referralCount, DateTime firstReferralDate)
        {
            Name = name;
            ReferralCount = referralCount;
            FirstReferralDate = firstReferralDate.ToString("dd/MM/yyyy");
            var monthTotal = (DateTime.Now.Year - firstReferralDate.Year) * 12
                + DateTime.Now.Month - firstReferralDate.Month;
            MonthsSinceFirstReferral = monthTotal < 1 ? 1 : monthTotal; 
            ReferralsPerMonth = (ReferralCount / (float)MonthsSinceFirstReferral).ToString("0.##");
        }    
    }

    private class CsvRowLaPerMonthStatistics
    {
        [Index(0)]
        [Name("LA Name")]
        public string Name { get; set; }

        [Index(1)] 
        [Name("Consortium Name")] 
        public string ConsortiumName { get; set; }
        
        [Index(2)]
        [Name("Total WH:LG Referrals")]
        public int ReferralCount { get; set; }
        
        [Index(3)]
        [Name("Date of First Referral")]
        public string FirstReferralDate { get; set; }

        [Index(4)] [Name("Months Since First Referral")]
        public int MonthsSinceFirstReferral { get; set; }

        [Index(5)] [Name("Referrals Per Month")]
        public string ReferralsPerMonth { get; set; }
        
        public CsvRowLaPerMonthStatistics(string name, string consortiumName, int referralCount, DateTime firstReferralDate)
        {
            Name = name;
            ConsortiumName = consortiumName;
            ReferralCount = referralCount;
            FirstReferralDate = firstReferralDate.ToString("dd/MM/yyyy");
            var monthTotal = (DateTime.Now.Year - firstReferralDate.Year) * 12
                + DateTime.Now.Month - firstReferralDate.Month;
            MonthsSinceFirstReferral = monthTotal < 1 ? 1 : monthTotal; 
            ReferralsPerMonth = (ReferralCount / (float)MonthsSinceFirstReferral).ToString("0.##");
        }    
    }


    private class CsvRowReferralCodes
    {
        [Index(0)]
        [Name("Consortium")]
        public string Consortium { get; set; }
        
        [Index(1)]
        [Name("Local Authority")]
        public string LocalAuthority { get; set; }
        
        [Index(2)]
        [Name("Referral Code")]
        public string ReferralCode { get; set; }
        public CsvRowReferralCodes(ReferralRequest request){
            Consortium =  LocalAuthorityData.LocalAuthorityDetailsByCustodianCode[request.CustodianCode].Consortium;
            LocalAuthority =  LocalAuthorityData.LocalAuthorityDetailsByCustodianCode[request.CustodianCode].Name;
            ReferralCode = request.ReferralCode;
        }
    }
    
    private class ConsortiumStatistics
    {
        public bool AllConsortiumReferralsDownloaded { get; set; }
        public int NumberUndownloadedConsortiumReferrals { get; set; }
        public double PercentageUndownloadedConsortiumReferrals { get; set; }
        public bool AllConsortiumReferralsContacted { get; set; }
        public int NumberUncontactedConsortiumReferrals { get; set; }
        public double PercentageUncontactedConsortiumReferrals { get; set; }
        public ConsortiumStatistics(List<ReferralRequest> referralRequests){
            NumberUndownloadedConsortiumReferrals = referralRequests.Count(rr => !rr.ReferralWrittenToCsv);
            AllConsortiumReferralsDownloaded = NumberUndownloadedConsortiumReferrals == 0;
            PercentageUndownloadedConsortiumReferrals = 100 * (double)NumberUndownloadedConsortiumReferrals / referralRequests.Count();
            NumberUncontactedConsortiumReferrals = referralRequests.Count(rr => rr.FollowUp?.WasFollowedUp == false);
            AllConsortiumReferralsContacted = NumberUncontactedConsortiumReferrals == 0;
            PercentageUncontactedConsortiumReferrals = 100 * (double)NumberUncontactedConsortiumReferrals / referralRequests.Count();
        }
    }

    private class CsvRowLaDownloadInformation
    {
        [Index(0)]
        [Name("SLA Report Date")]
        public string ReportDate { get; set; }
        
        [Index(1)]
        [Name("LA")]
        public string LocalAuthority { get; set; }
        
        [Index(2)]
        [Name("LA Number of Referrals Not Downloaded")]
        public int NumberUndownloadedLaReferrals { get; set; }

        [Index(3)]
        [Name("LA Percentage of Referrals Not Downloaded")]
        public double PercentageUndownloadedLaReferrals { get; set; }

        [Index(4)]
        [Name("LA Number of Referrals Not Contacted")]
        public int NumberUncontactedLaReferrals { get; set; }

        [Index(5)]
        [Name("LA Percentage of Referrals Not Contacted")]
        public double PercentageUncontactedLaReferrals { get; set; }
        
        [Index(6)]
        [Name("LA Number of Referrals Responded to email")]
        public int LaNumberOfFollowUpResponses { get; set; }
        
        [Index(7)]
        [Name("LA Percentage of Referrals Responded to email")]
        public double LaPercentageOfFollowUpResponses { get; set; }

        public CsvRowLaDownloadInformation(IGrouping<string,ReferralRequest> requestGroupingByCustodianCode)
        {
            ReportDate = DateTime.Today.ToString("dd-MMM");
            LocalAuthority =  LocalAuthorityData.LocalAuthorityDetailsByCustodianCode[requestGroupingByCustodianCode.Key].Name;
            NumberUndownloadedLaReferrals = requestGroupingByCustodianCode.Count(rr => !rr.ReferralWrittenToCsv);
            PercentageUndownloadedLaReferrals = 100 * (double)NumberUndownloadedLaReferrals/requestGroupingByCustodianCode.Count();
            NumberUncontactedLaReferrals = requestGroupingByCustodianCode.Count(rr => rr.FollowUp?.WasFollowedUp == false);
            PercentageUncontactedLaReferrals = 100 * (double)NumberUncontactedLaReferrals / requestGroupingByCustodianCode.Count();
            LaNumberOfFollowUpResponses = requestGroupingByCustodianCode.Count(rr => rr.FollowUp?.WasFollowedUp != null);
            LaPercentageOfFollowUpResponses = 100 * (double)LaNumberOfFollowUpResponses / requestGroupingByCustodianCode.Sum(rr => rr.FollowUp != null ? 1 : 0 );
        }
    }

    private class CsvRowConsortiumDownloadInformationRow
    {
        [Index(0)]
        [Name("SLA Report Date")]
        public string ReportDate { get; set; }
        
        [Index(1)]
        [Name("Consortium")]
        public string Consortium { get; set; }

        [Index(2)]
        [Name("Consortium All Referrals Downloaded")]
        public bool AllConsortiumReferralsDownloaded { get; set; }
        
        [Index(3)]
        [Name("Consortium Number of Referrals Not Downloaded")]
        public int NumberUndownloadedConsortiumReferrals { get; set; }

        [Index(4)]
        [Name("Consortium Percentage of Referrals Not Downloaded")]
        public double PercentageUndownloadedConsortiumReferrals { get; set; }

        [Index(5)]
        [Name("Consortium All Referrals Contacted")]
        public bool AllConsortiumReferralsContacted { get; set; }
        
        [Index(6)]
        [Name("Consortium Number of Referrals Not Contacted")]
        public int NumberUncontactedConsortiumReferrals { get; set; }

        [Index(7)]
        [Name("Consortium Percentage of Referrals Not Contacted")]
        public double PercentageUncontactedConsortiumReferrals { get; set; }

        public CsvRowConsortiumDownloadInformationRow(string consortiumName, ConsortiumStatistics consortiumData)
        {
            ReportDate = DateTime.Today.ToString("dd-MMM");
            Consortium =  consortiumName;
            AllConsortiumReferralsDownloaded = consortiumData.AllConsortiumReferralsDownloaded;
            NumberUndownloadedConsortiumReferrals = consortiumData.NumberUndownloadedConsortiumReferrals;
            PercentageUndownloadedConsortiumReferrals = consortiumData.PercentageUndownloadedConsortiumReferrals;
            AllConsortiumReferralsContacted = consortiumData.AllConsortiumReferralsContacted;
            NumberUncontactedConsortiumReferrals = consortiumData.NumberUncontactedConsortiumReferrals;
            PercentageUncontactedConsortiumReferrals = consortiumData.PercentageUncontactedConsortiumReferrals; 
        }    
    }

    private class CsvRowReferralRequest
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
        [Name("EPC confirmed by homeowner")]
        public string EpcConfirmed { get; set; }
        
        [Index(13)]
        [Name("EPC Lodgement Date")]
        public string EpcLodgementDate { get; set; }
        
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

        public CsvRowReferralRequest(ReferralRequest request)
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
            EpcConfirmed = request.EpcConfirmation switch
            {
                // Yes not included here as users can't be referred if they've confirmed a high EPC
                EpcConfirmation.No => "Homeowner disagrees with rating",
                EpcConfirmation.Unknown => "Homeowner unsure",
                null => "",
                _ => throw new ArgumentOutOfRangeException("request.EpcConfirmation", "Unrecognised EpcConfirmation value: " + request.EpcConfirmation),
            };
            EpcLodgementDate = request.EpcLodgementDate?.ToString("yyyy-MM-dd HH:mm:ss");
            HouseholdIncome = request.IncomeBand switch
            {
#pragma warning disable CS0618 // Obsolete Income Bands used to preserve backwards-compatibility
                IncomeBand.UnderOrEqualTo31000 => "Below £31k", 
                IncomeBand.GreaterThan31000 => "£31k or above", 
                IncomeBand.UnderOrEqualTo34500 => "Below £34.5k",
                IncomeBand.GreaterThan34500 => "£34.5k or above", 
#pragma warning restore CS0618
                IncomeBand.UnderOrEqualTo36000 => "£36,000 or less",
                IncomeBand.GreaterThan36000 => "More than £36,000",
                _ => throw new ArgumentOutOfRangeException("request.IncomeBand", "Unrecognised IncomeBand value: " + request.IncomeBand)
            };
            EligiblePostcode = request.IsLsoaProperty;
            Tenure = "Owner";
        }
    }
    
    private class CsvRowPendingReferralRequest
    {
        [Index(0)]
        public string Consortium { get; set; }
        
        [Index(1)]
        [Name("Local Authority")]
        public string LocalAuthority { get; set; }
        
        [Index(2)]
        [Name("Referral Date")]
        public string ReferralDate { get; set; }
        
        [Index(3)]
        [Name("Referral Code")]
        public string ReferralCode { get; set; }
        
        [Index(4)]
        public string Name { get; set; }
        
        [Index(5)]
        public string Email { get; set; }
        
        [Index(6)]
        public string Telephone { get; set; }
        
        [Index(7)]
        [Name("Local Authority Status")]
        public string LaStatus { get; set; }

        public CsvRowPendingReferralRequest(ReferralRequest referralRequest)
        {
            var localAuthority = LocalAuthorityData.LocalAuthorityDetailsByCustodianCode[referralRequest.CustodianCode];
            Consortium = localAuthority.Consortium;
            LocalAuthority = localAuthority.Name;
            ReferralDate = referralRequest.RequestDate.ToString("yyyy-MM-dd HH:mm:ss");
            ReferralCode = referralRequest.ReferralCode;
            Name = referralRequest.FullName;
            Email = referralRequest.ContactEmailAddress;
            Telephone = referralRequest.ContactTelephone;
            LaStatus = localAuthority.Status.ToString();
        }
    }

    private MemoryStream GenerateCsvMemoryStreamFromFileRows<T>(IEnumerable<T> rows)
    {
        var csvConfiguration = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            InjectionOptions = InjectionOptions.Strip
        };

        using var writeableMemoryStream = new MemoryStream();
        using var streamWriter = new StreamWriter(writeableMemoryStream, new UTF8Encoding(false));
        using var csvWriter = new CsvWriter(streamWriter, csvConfiguration);
        {
            csvWriter.WriteRecords(rows);
            csvWriter.Flush();
            return new MemoryStream(writeableMemoryStream.GetBuffer(), 0, (int)writeableMemoryStream.Length, false);
        }
    }
}
