using System.Net;

namespace HerPublicWebsite.BusinessLogic.ExternalServices.Common
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