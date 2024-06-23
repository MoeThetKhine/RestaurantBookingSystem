using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RestaurantBookingSystemApi.Resources;

namespace RestaurantBookingSystemApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseController : ControllerBase
    {
        protected IActionResult Created()
        {
            return StatusCode(201,MessageResource.SavingSuccessful);
        }
        protected IActionResult HandleFailure(Exception ex) 
        {
            return StatusCode(500, ex.Message);
        }
    }
}
