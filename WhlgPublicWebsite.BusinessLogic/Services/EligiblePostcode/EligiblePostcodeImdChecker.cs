namespace WhlgPublicWebsite.BusinessLogic.Services.EligiblePostcode;

public interface IEligiblePostcodeImdFileChecker
{
    bool ShouldUseImd2025();
}

public class EligiblePostcodeImdFileChecker : IEligiblePostcodeImdFileChecker
{
    // TODO DESNZ-2197: Remove this class once we have moved entirely to the IMD2025 postcodes.
    private readonly Func<DateTime> getToday;

    public EligiblePostcodeImdFileChecker() : this(() => DateTime.Today)
    {
    }

    public EligiblePostcodeImdFileChecker(Func<DateTime> getToday)
    {
        this.getToday = getToday;
    }

    public bool ShouldUseImd2025()
    {
        return getToday() >= new DateTime(2026, 3, 1);
    }
}
