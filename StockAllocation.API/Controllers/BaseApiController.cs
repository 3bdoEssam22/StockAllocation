using Microsoft.AspNetCore.Mvc;
using StockAllocation.Application.Common;

namespace StockAllocation.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseApiController : ControllerBase
    {
        protected ActionResult HandleResponse<T>(GenericResponse<T> response)
        {
            if (response is null)
                return StatusCode(StatusCodes.Status500InternalServerError);

            return response.StatusCode switch
            {
                StatusCodes.Status200OK => Ok(response),
                StatusCodes.Status201Created => StatusCode(StatusCodes.Status201Created, response),
                StatusCodes.Status400BadRequest => BadRequest(response),
                StatusCodes.Status401Unauthorized => Unauthorized(response),
                StatusCodes.Status403Forbidden => StatusCode(StatusCodes.Status403Forbidden, response),
                StatusCodes.Status404NotFound => NotFound(response),
                StatusCodes.Status409Conflict => Conflict(response),
                _ => StatusCode(response.StatusCode, response)
            };
        }
    }
}
