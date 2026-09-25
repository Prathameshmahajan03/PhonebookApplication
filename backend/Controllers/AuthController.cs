using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhonebookApplication.Models;
using PhonebookApplication.Services;

namespace PhonebookApplication.Controllers
{
    [ApiController]
    [AllowAnonymous]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponse>> Login(
            [FromBody] LoginRequest request)
        {
            LoginResponse? response = await _authService.LoginAsync(request);

            if (response == null)
            {
                return Unauthorized();
            }

            return Ok(response);
        }
    }
}
