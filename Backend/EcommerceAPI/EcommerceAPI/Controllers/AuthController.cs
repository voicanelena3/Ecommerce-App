using BusinessLogic.DTOs;
using BusinessLogic.Services;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceAPI.Controllers
{
    /// <summary>
    /// Handles user authentication operations such as registration and login via HTTP endpoints.
    /// </summary>
    /// <remarks>This controller provides endpoints for registering new users and authenticating existing
    /// users. It relies on an injected authentication service to perform the underlying operations. All endpoints
    /// expect request data in the body of the HTTP request and return appropriate HTTP status codes based on the
    /// outcome of the operation.</remarks>
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }
        /// <summary>
        /// Registers a new user account using the provided registration details.
        /// </summary>
        /// <remarks>This endpoint expects a valid RegisterRequest object in the request body. If the user
        /// already exists or the registration fails, a descriptive error message is returned in the response
        /// body.</remarks>
        /// <param name="request">The registration information for the new user. Must include all required fields and pass model validation.</param>
        /// <returns>An IActionResult that represents the result of the registration operation. Returns 200 OK with the
        /// registration response if successful; otherwise, returns 400 Bad Request with error details.</returns>

        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var response = _authService.Register(request);
            if (response == null)
                return BadRequest(new { message = "Registration failed. User may already exist." });

            return Ok(response);
        }
        /// <summary>
        /// Authenticates a user with the provided credentials and returns an authentication response if successful.
        /// </summary>
        /// <remarks>This endpoint expects a JSON payload representing the user's login credentials. The
        /// response includes authentication details such as tokens if authentication succeeds.</remarks>
        /// <param name="request">The login request containing the user's credentials. Must not be null and must satisfy all validation
        /// requirements.</param>
        /// <returns>An IActionResult containing the authentication response if login is successful; otherwise, a 400 Bad Request
        /// if the request is invalid or a 401 Unauthorized if the credentials are incorrect.</returns>

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var response = _authService.Login(request);
            if (response == null)
                return Unauthorized(new { message = "Invalid email or password." });

            return Ok(response);
        }
    }
}
