using Microsoft.AspNetCore.Mvc;
using UserAuthAPI.Models;
using UserAuthAPI.Models.DTO;
using UserAuthAPI.Services;

namespace UserAuthAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class UserAuthController : ControllerBase
{
    readonly UserAuthService _service;

    public UserAuthController(UserAuthService service)
    {
        _service = service;
    }
    [HttpGet("User")]
    public async Task<ActionResult<IEnumerable<UserResponseDTO>>> GetUsersAsync()
    {
        var users = await _service.GetUsersAsync();
        if (users is null)
        {
            return NoContent();
        }

        return Ok(users);
    }
    [HttpGet("UserEmail={email}")]
    public async Task<ActionResult<UserResponseDTO>> GetUserByEmail(string email)
    {
        var user = await _service.GetUserByEmailAsync(email);
        if (user == null)
        {
            return NoContent();
        }

        return Ok(user);
    }
    [HttpPost("User")]
    public async Task<IActionResult> CreateUserAsync([FromBody] UserRequestDTO newUser)
    {
        var checkExistingEmail = await _service.EmailAlreadyExists(newUser.Email);
        var checkExistingUser = await _service.UserAlreadyExists(newUser.UserName);

        if (checkExistingUser)
        {
            return BadRequest("Username already used");
        }
        if (checkExistingEmail)
        {
            return BadRequest("Email already used");
        }

        await _service.CreateUserAsync(newUser.FirstName, newUser.LastName, newUser.UserName, newUser.Password, newUser.Email);
        return StatusCode(201);
    }

    [HttpGet("AuthoriseUser")]
    public async Task<ActionResult<UserResponseDTO>> AutenticateUserAsync(string username, string password)
    {
        var user = await _service.AutenticateUserAsync(username, password);
        if (user == null)
        {
            return BadRequest("Wrong username or password");
        }

        return user;
    }

    [HttpPost("SecurityQandA")]
    public async Task<IActionResult> CreateUserSecurityQandA([FromBody] SecurityRequestQandADTO security)
    {
        await _service.CreateSecurityQandAAsync(security.UserEmail, security.Question, security.Answer);

        return StatusCode(201);
    }

    [HttpPut("ResetPasswordUsingSecurityQuestion")]
    public async Task<ActionResult<SecurityRequestQandADTO>> ResetPasswordUsingSecurityQandAAsync(string email, string question, string answer, string newpassword)
    {
        var result = await _service.ResetPasswordUsingSecurityQandAAsync(email, question, answer, newpassword);

        if (result == null)
        {
            return BadRequest();
        }

        return StatusCode(201);
    }

    [HttpGet("RecoveryPhrase")]

    public async Task<ActionResult<string>> GetRecoveryPhraseAsync(string email)
    {
        var recoveryString = await _service.GetRecoveryPhraseAsync(email);
        return Ok(recoveryString);
    }

    [HttpPut("ResetPasswordUsingRecoveryPhrase")]

    public async Task<ActionResult<RecoveryPhrase>> ResetPasswordWithPhrase(string email, string phrase, string newpassword)
    {
        var result = await _service.ResetPasswordUsingPhraseAsync(email, phrase, newpassword);

        if (result == null)
        {
            return BadRequest();
        }

        return StatusCode(201);
    }

    [HttpGet("OTP")]
    public async Task<ActionResult<int>> GetOTPAsync(string email)
    {
        var otp = await _service.SendOtpToUserEmail(email);
        if (otp == 0)
        {
            return StatusCode(400);
        }
        return StatusCode(201);
    }

    [HttpPut("ResetPasswordWithOTP")]
    public async Task<ActionResult<OTP>> ResetPasswordWithOTP(int otp, string password)
    {
        var otpResult = await _service.ResetPasswordUsingOTPAsync(otp, password);

        if (otpResult == null)
        {
            return BadRequest();
        }

        return StatusCode(201);
    }

}