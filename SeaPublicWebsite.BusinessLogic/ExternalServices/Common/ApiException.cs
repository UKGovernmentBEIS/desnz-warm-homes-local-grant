using System.Net;

namespace SeaPublicWebsite.BusinessLogic.ExternalServices.Common
{
    public class ApiException : Exception
    {
        public HttpStatusCode StatusCode;

        public ApiException(string message, HttpStatusCode statusCode) : base(message)
        {
            StatusCode = statusCode;
        }
    }
}