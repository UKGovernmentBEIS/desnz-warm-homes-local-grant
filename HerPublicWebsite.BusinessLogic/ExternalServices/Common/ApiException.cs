using System.Net;

namespace HerPublicWebsite.BusinessLogic.ExternalServices.Common
{
    public class ApiException : Exception
    {
        public HttpStatusCode StatusCode;
        public HttpContent Content;

        public ApiException(string message, HttpStatusCode statusCode, HttpContent content) : base(message)
        {
            StatusCode = statusCode;
            Content = content;
        }
    }
}
