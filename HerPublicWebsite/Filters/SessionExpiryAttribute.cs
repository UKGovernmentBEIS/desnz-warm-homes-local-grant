using System.Linq;
using HerPublicWebsite.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace HerPublicWebsite.Filters;

/// <summary>
/// Add the session expiry attribute to a controller or action to redirect to the session expiry page
/// if no session keys are present in the session (either because it has expired or never existed).
///
/// May be disabled at the action level by adding <see cref="ExcludeFromSessionExpiryAttribute"/>
/// </summary>
public class SessionExpiryAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (context.Filters.All(f => f.GetType() != typeof(ExcludeFromSessionExpiryAttribute)) &&
            !context.HttpContext.Session.Keys.Any())
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

public class ExcludeFromSessionExpiryAttribute : ActionFilterAttribute {}
