using Application;
using Application.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
//using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;



namespace ManjeetFigma.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _auth;
        public AuthController(IAuthService auth) => _auth = auth;

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
        {
            try
            {
                await _auth.RegisterAsync(request);
                return Ok(new { mesaage = "Registered. OTP sent to phone." });
            }
            catch (Exception ex) { return BadRequest(new { error = ex.Message }); }
        }

        [HttpPost("send-otp")]
        public async Task<IActionResult> SendOtp([FromBody] OtpRequest request)
        {
            try { await _auth.SendOtpAsync(request); return Ok(new { message = "Otp sent." }); }
            catch (Exception ex) { return BadRequest(new { error = ex.Message }); }


        }

        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpRequest request)
        {
            try { await _auth.verifyOtpAsync(request); return Ok(new { message = "phone verified" }); }
            catch (Exception ex) { return BadRequest(new { error = ex.Message }); }
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            try
            {
                var token = await _auth.LoginAsync(request);
                return Ok(new { token });
            }
            catch (Exception ex) { return BadRequest(new { error = ex.Message }); }
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> Forgot([FromBody] OtpRequest request)
        {
            try { await _auth.SendOtpAsync(request); return Ok(new { message = "Otp send for reset" }); }
            catch (Exception ex) { return BadRequest(new { error = ex.Message }); }
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto request)
        {
            try
            {
                await _auth.ResetPasswordAsync(
                    new VerifyOtpRequest(request.CountryCode, request.PhoneNumber, request.Code),
                    request.NewPassword
                );

                return Ok(new { message = "Password reset successful" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

    }
}

    

