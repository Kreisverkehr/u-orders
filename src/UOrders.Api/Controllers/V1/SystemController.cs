using Microsoft.AspNetCore.Mvc;

namespace UOrders.Api.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class SystemController : ControllerBase
{
    // GET: api/<SystemController>
    [HttpPost("echo")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
    public IActionResult Echo([FromBody] string str)
    {
        return Ok(str);
    }
}
