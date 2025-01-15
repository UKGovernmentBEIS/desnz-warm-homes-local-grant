using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WhlgPublicWebsite.BusinessLogic.Services.Password;
using WhlgPublicWebsite.Models.PasswordAuth;

namespace WhlgPublicWebsite.Controllers;

[Route("password")]
public class PasswordAuthController(PasswordService passwordService) : Controller
{
    [HttpGet]
    public IActionResult Index_Get()
    {
        var viewModel = new PasswordAuthViewModel();
        
        return View("Index", viewModel);
    }
    
    [HttpPost]
    public IActionResult Index_Post(PasswordAuthViewModel viewModel)
    {
        if (!ModelState.IsValid) return Index_Get();
        
        var hashedPassword = passwordService.HashPassword(viewModel.Password);

        if (!passwordService.HashMatchesConfiguredPassword(hashedPassword))
        {
            ModelState.AddModelError("password", "The password is not correct");
            return Index_Get();
        }

        Response.Cookies.Append(PasswordAuthService.PasswordAuthCookieName, hashedPassword, new CookieOptions
        {
            Secure = true,
            HttpOnly = true
        });

        return Redirect("questionnaire");
    }
}