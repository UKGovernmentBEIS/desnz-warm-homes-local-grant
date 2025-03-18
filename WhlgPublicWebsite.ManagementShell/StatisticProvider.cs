using System.Text;
using WhlgPublicWebsite.BusinessLogic;
using WhlgPublicWebsite.BusinessLogic.Services.CsvFileCreator;

namespace WhlgPublicWebsite.ManagementShell;

public interface IStatisticProvider
{
    public string GenerateReferralPerLaPerMonthStatistics();
    public string GenerateReferralPerConsortiumPerMonthStatistics();
}

public class StatisticProvider(IDataAccessProvider dataAccessProvider, ICsvFileCreator csvFileCreator)
    : IStatisticProvider
{
    public string GenerateReferralPerLaPerMonthStatistics()
    {
        var referralRequests = dataAccessProvider.GetReferralRequestsSubmittedAfterHug2Shutdown();
        var referralStatistics = csvFileCreator.CreatePerMonthLocalAuthorityReferralStatistics(referralRequests);
        referralStatistics.Seek(0, SeekOrigin.Begin);
        return Encoding.UTF8.GetString(referralStatistics.ToArray());
    }

    public string GenerateReferralPerConsortiumPerMonthStatistics()
    {
        var referralRequests = dataAccessProvider.GetReferralRequestsSubmittedAfterHug2Shutdown();
        var referralStatistics = csvFileCreator.CreatePerMonthConsortiumReferralStatistics(referralRequests);
        referralStatistics.Seek(0, SeekOrigin.Begin);
        return Encoding.UTF8.GetString(referralStatistics.ToArray());
    }
}