using System;
using Microsoft.AspNetCore.Mvc;

namespace HerPublicWebsite.Controllers;

[Route("error")]
public class ErrorController: Controller
{
    [HttpGet]
    [HttpPost]
    public IActionResult HandleException()
    {
        return View("ServiceIssue");
    }
    
    [HttpGet("{code:int}")]
    public IActionResult HandleErrorsWithStatusCode(int code)
    {
        return code switch
        {
            404 => View("PageNotFound"),
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}