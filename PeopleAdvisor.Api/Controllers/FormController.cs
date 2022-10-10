using Microsoft.AspNetCore.Mvc;

namespace PeopleAdvisor.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FormController : ControllerBase
{
    [HttpGet]
    public IActionResult SayHello()
    {
        return Ok("Hello World");
    }
}