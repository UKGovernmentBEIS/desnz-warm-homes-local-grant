using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WhlgPublicWebsite.BusinessLogic.Services.Password;
using WhlgPublicWebsite.Models.PasswordAuth;
using WhlgPublicWebsite.Services;

namespace WhlgPublicWebsite.Controllers;

[Route("password")]
public class PasswordAuthController(PasswordService passwordService, PasswordAuthService passwordAuthService)
    : Controller
{
    [HttpGet]
    public IActionResult Index_Get([FromQuery] string returnPath)
    {
        if (!passwordAuthService.PasswordAuthIsEnabled())
        {
            return NotFound();
        }

        var viewModel = new PasswordAuthViewModel
        {
            ReturnPath = returnPath ?? "/"
        };

        return View("Index", viewModel);
    }

    [HttpPost]
    public IActionResult Index_Post(PasswordAuthViewModel viewModel)
    {
        if (!ModelState.IsValid) return Index_Get(viewModel.ReturnPath);
        
        var hashedPassword = passwordService.HashPassword(viewModel.Password);

        if (!passwordService.HashMatchesConfiguredPassword(hashedPassword))
        {
            ModelState.AddModelError("password", "The password is not correct");
            return Index_Get(viewModel.ReturnPath);
        }

        Response.Cookies.Append(PasswordAuthService.PasswordAuthCookieName, hashedPassword, new CookieOptions
        {
            Secure = true,
            HttpOnly = true
        });

        return Redirect(viewModel.ReturnPath);
    }
}