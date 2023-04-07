using Microsoft.AspNetCore.Mvc;

namespace Api;

[ApiController] // enables automatic validation in the ASP.NET Core pipeline (no need to check for ModelState anymore)
public class ApplicationController : ControllerBase
{
}