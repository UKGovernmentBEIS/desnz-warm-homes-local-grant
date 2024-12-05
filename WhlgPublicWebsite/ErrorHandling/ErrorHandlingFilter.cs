using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace HerPublicWebsite.ErrorHandling
{
    public class ErrorHandlingFilter : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            if (context.Exception is CustomErrorPageException customErrorPageException)
            {
                context.Result = new ViewResult
                {
                    StatusCode = customErrorPageException.StatusCode,
                    ViewName = customErrorPageException.ViewName,
                    ViewData = new ViewDataDictionary(new EmptyModelMetadataProvider(), context.ModelState)
                    {
                        // For this type of custom error page, we use the exception itself as the model
                        Model = customErrorPageException
                    }

                };
            }
        }

    }
}
