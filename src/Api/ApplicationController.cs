using System.Net;
using CSharpFunctionalExtensions;
using DomainModel;
using Microsoft.AspNetCore.Mvc;

namespace Api;

[ApiController] // enables automatic validation in the ASP.NET Core pipeline (no need to check for ModelState anymore)
public class ApplicationController : ControllerBase
{
    // These are wrappers for built-in methods.
    // However not all HTTP codes have such methods. So to be more flexible we introduce our own IActionResult - EnvelopeResult.
    // protected new IActionResult Ok(object result = null)
    // {
    //     return base.Ok(Envelope.Ok(result));
    // }
    //
    // protected IActionResult Error(Error error, string invalidField = null)
    // {
    //     return base.BadRequest(Envelope.Error(error, invalidField));
    // }
    
    protected new IActionResult Ok(object result = null)
    {
        return new EnvelopeResult(Envelope.Ok(result), HttpStatusCode.OK);
    }
    
    protected IActionResult NotFound(Error error, string invalidField = null)
    {
        return new EnvelopeResult(Envelope.Error(error, invalidField), HttpStatusCode.NotFound);
    }
    
    protected IActionResult Error(Error error, string invalidField = null)
    {
        return new EnvelopeResult(Envelope.Error(error, invalidField), HttpStatusCode.BadRequest);
    }

    protected IActionResult FromResult<T>(Result<T, Error> result)
    {
        if (result.IsSuccess)
            return Ok();

        return Error(result.Error);
    }
}
