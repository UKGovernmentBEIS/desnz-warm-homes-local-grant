using System.Linq;
using HerPublicWebsite.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace HerPublicWebsite.Filters;

public class SessionExpiryAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.HttpContext.Session.Keys.Any())
        {
            context.Result = new RedirectToActionResult(
                nameof(StaticPagesController.SessionExpired),
                "StaticPages",
                null);
            return;
        }
        base.OnActionExecuting(context);
    }
}
