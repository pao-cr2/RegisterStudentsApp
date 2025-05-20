using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RegisterStudents.API.Dto;
using RegisterStudents.API.Services;

namespace RegisterStudents.API.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginDto)
        {
            var response = await _authService.AuthenticateAsync(loginDto.Email, loginDto.Password);

            if (response == null)
                return Unauthorized(new { message = "Email or password is incorrect" });

            return Ok(response);

        }

    }

}
