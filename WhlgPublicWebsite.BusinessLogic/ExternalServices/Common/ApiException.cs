using System.Net;

namespace WhlgPublicWebsite.BusinessLogic.ExternalServices.Common
{
    public class ApiException : Exception
    {
        public HttpStatusCode StatusCode;
        public string Content;

        public ApiException(string message, HttpStatusCode statusCode, string content) : base(message)
        {
            StatusCode = statusCode;
            Content = content;
        }
    }
}
