namespace WhlgPublicWebsite.BusinessLogic.ExternalServices.EpbEpc;

public class EpcApiUnavailableException : Exception
{
    public EpcApiUnavailableException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
