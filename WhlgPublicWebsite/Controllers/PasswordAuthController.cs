using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WhlgPublicWebsite.BusinessLogic.Services.Password;
using WhlgPublicWebsite.Models.PasswordAuth;

namespace WhlgPublicWebsite.Controllers;

[Route("password")]
public class PasswordAuthController : Controller
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
        Console.WriteLine(viewModel.Password);
        return View("Index");
    }
}