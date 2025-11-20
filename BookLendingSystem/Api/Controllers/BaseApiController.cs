using Microsoft.AspNetCore.Mvc;

namespace BookLendingSystem.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BaseApiController : ControllerBase
    {
        // property to get the current user's Id
        protected string CurrentUserId => User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
    }
}
