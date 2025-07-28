using Core.Entities;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class BuggyController : BaseApiController
{
    [HttpGet("unauthorized")]
    public IActionResult GetUnauthorized()
    {
        return Unauthorized("You are not authorized to access this resource.");
    }

    [HttpGet("badrequest")]
    public IActionResult GetBadRequest()
    {
        return BadRequest("This is a bad request.");
    }

    [HttpGet("notfound")]
    public IActionResult GetNotFound()
    {
        return NotFound("The requested resource was not found.");
    }

    [HttpGet("internalerror")]
    public IActionResult GetInternalError()
    {
        throw new Exception("This is an internal server error.");
    }

    [HttpPost("validationerror")]
    public IActionResult GetValidationError(Product product)
    {
        ModelState.AddModelError("Error", "This is a validation error.");
        return ValidationProblem();
    }

}
