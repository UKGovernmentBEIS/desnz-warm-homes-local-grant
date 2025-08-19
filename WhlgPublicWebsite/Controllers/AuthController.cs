using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WhlgPublicWebsite.BusinessLogic.Services.Password;
using WhlgPublicWebsite.Models.Auth;
using WhlgPublicWebsite.Services;

namespace WhlgPublicWebsite.Controllers;

[Route("password")]
public class AuthController(PasswordService passwordService, AuthService authService)
    : Controller
{
    [HttpGet]
    public IActionResult Index_Get([FromQuery] string returnPath)
    {
        if (!authService.AuthIsEnabled())
        {
            return NotFound();
        }

        var viewModel = new AuthViewModel
        {
            ReturnPath = returnPath ?? "/"
        };

        return View("Index", viewModel);
    }

    [HttpPost]
    public IActionResult Index_Post(AuthViewModel viewModel)
    {
        if (!ModelState.IsValid) return Index_Get(viewModel.ReturnPath);

        var hashedPassword = passwordService.HashPassword(viewModel.Password);

        if (!passwordService.HashMatchesConfiguredPassword(hashedPassword))
        {
            ModelState.AddModelError("password", "The password is not correct");
            return Index_Get(viewModel.ReturnPath);
        }

        Response.Cookies.Append(AuthService.AuthCookieName, hashedPassword, new CookieOptions
        {
            Secure = true,
            HttpOnly = true
        });

        var returnPath = IsValidReturnPath(viewModel.ReturnPath) ? viewModel.ReturnPath : "/";

        return Redirect(returnPath);
    }
    
    private static bool IsValidReturnPath(string returnPath)
    {
        // return false if contains \. they can be treated as / in some cases and never used by the site so can be ignored
        if (returnPath.Contains('\\'))
        {
            return false;
        }
        
        // ensure both that the link starts with a slash (is relative) but not starts with //
        // links starting with // are protocol-relative URLs which can be used to redirect to other domains
        // send to index if this is the case
        return returnPath.StartsWith('/') && !returnPath.StartsWith("//");
    }
}